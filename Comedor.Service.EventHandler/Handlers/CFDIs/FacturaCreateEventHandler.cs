using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.Facturas;
using Comedor.Service.EventHandler.Commands.CFDIs.NodosXML;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using Comedor.Domain.DFacturas;
using Comedor.Domain.DContratos;
using Comedor.Domain.DRepositorios;

namespace Comedor.Service.EventHandler.Handlers.CFDIs
{
    public class FacturaCreateEventHandler : IRequestHandler<FacturaCreateCommand, Factura>
    {
        private readonly ApplicationDbContext _context;

        public FacturaCreateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Factura> Handle(FacturaCreateCommand request, CancellationToken cancellationToken)
        {
            string dateNow = DateTime.Now.ToString("yyyyMMddHHmmss");
            Factura factura = new Factura();

            if (await copiaFactura(request.XML))
            {
                Facturas xmlFactura = desglozaXML(request, dateNow);
                await copiaFacturaXML(request.XML, request.Anio, request.Mes, request.Inmueble, xmlFactura.Tipo, xmlFactura.Comprobante.Serie + xmlFactura.Comprobante.Folio);

                if (request.PDF != null)
                {
                    xmlFactura.ArchivoPDF = request.PDF.FileName;
                    await copiaFacturaXML(request.PDF, request.Anio, request.Mes, request.Inmueble, xmlFactura.Tipo, xmlFactura.Comprobante.Serie + xmlFactura.Comprobante.Folio);
                }

                if (await VerificaFacturaExistente(xmlFactura) && await VerificaFacturaServicio(xmlFactura))
                {
                    factura = await InsertaFactura(xmlFactura);
                    factura.EstatusId = 201;
                }
                else if (!await VerificaFacturaExistente(xmlFactura))
                {
                    factura = GetXML(xmlFactura);
                    factura.EstatusId = 205;
                }
                else if (!await VerificaFacturaServicio(xmlFactura))
                {
                    factura = GetXML(xmlFactura);
                    factura.EstatusId = 206;
                }
                else
                {
                    factura = GetXML(xmlFactura);
                    factura.EstatusId = 400;
                }                

                string newPath = Directory.GetCurrentDirectory() + "\\Carga XML\\" + request.XML.FileName;

                File.Delete(newPath);
            }

            return factura;
        }

        public async Task<bool> VerificaFacturaExistente(Facturas factura)
        {
            try
            {
                Factura existFactura = await _context.Facturas.SingleOrDefaultAsync(f => f.Folio == factura.Comprobante.Folio && f.RFC == factura.Emisor.RFC);

                return existFactura != null ? false : true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
        }

        public async Task<bool> VerificaFacturaServicio(Facturas factura)
        {
            try
            {
                Repositorio repositorio = await _context.Repositorios.SingleOrDefaultAsync(f => f.Id == factura.RepositorioId);

                Contrato contrato = await _context.Contratos.SingleOrDefaultAsync(c => c.Id == repositorio.ContratoId);

                return factura.Emisor.RFC.Equals(contrato.RFC) ? true : false;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
           
        }

        public Factura GetXML(Facturas facturas)
        {
            try
            {
                var factura = new Factura
                {
                    RepositorioId = facturas.RepositorioId,
                    Facturacion = facturas.Facturacion,
                    InmuebleId = facturas.InmuebleId,
                    UsuarioId = facturas.UsuarioId,
                    Tipo = facturas.Tipo,
                    RFC = facturas.Emisor.RFC,
                    Nombre = facturas.Emisor.Nombre,
                    Serie = facturas.Comprobante.Serie,
                    Folio = facturas.Comprobante.Folio,
                    UsoCFDI = facturas.Receptor.UsoCFDI,
                    UUID = facturas.TimbreFiscal.UUID,
                    FechaTimbrado = facturas.TimbreFiscal.FechaTimbrado,
                    IVA = facturas.Traslado.Importe,
                    Subtotal = facturas.Comprobante.SubTotal,
                    Total = facturas.Comprobante.Total,
                    ArchivoXML = facturas.Archivo,
                    ArchivoPDF = facturas.ArchivoPDF,
                    FechaCreacion = DateTime.Now
                };

                return factura;

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
        
        public async Task<Factura> InsertaFactura(Facturas facturas)
        {
            try
            {
                var complete = false;

                var factura = new Factura
                {
                    RepositorioId = facturas.RepositorioId,
                    Facturacion = facturas.Facturacion,
                    InmuebleId = facturas.InmuebleId,
                    UsuarioId = facturas.UsuarioId,
                    Tipo = facturas.Tipo,
                    RFC = facturas.Emisor.RFC,
                    Nombre = facturas.Emisor.Nombre,
                    Serie = facturas.Comprobante.Serie,
                    Folio = facturas.Comprobante.Folio,
                    UsoCFDI = facturas.Receptor.UsoCFDI,
                    UUID = facturas.TimbreFiscal.UUID,
                    FechaTimbrado = facturas.TimbreFiscal.FechaTimbrado,
                    IVA = facturas.Traslado.Importe,
                    Subtotal = facturas.Comprobante.SubTotal,
                    Total = facturas.Comprobante.Total,
                    ArchivoXML = facturas.Archivo,
                    ArchivoPDF = facturas.ArchivoPDF,
                    FechaCreacion = DateTime.Now
                };

                await _context.AddAsync(factura);
                await _context.SaveChangesAsync();

                complete = await InsertaConceptosFactura(factura.Id, facturas);

                return complete ? factura : null;

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<bool> InsertaConceptosFactura(int factura, Facturas facturas)
        {
            try
            {
                foreach (var con in facturas.Conceptos)
                {
                    var conceptos = new ConceptosFactura
                    {
                        FacturaId = factura,
                        ClaveProducto = con.ClaveProdServ,
                        ClaveUnidad = con.ClaveUnidad,
                        Cantidad = con.Cantidad,
                        Descripcion = con.Descripcion,
                        Unidad = con.Unidad,
                        PrecioUnitario = con.ValorUnitario,
                        Descuento = con.Descuento,
                        IVA = facturas.Traslado.Importe,
                        Subtotal = con.Importe,
                        FechaCreacion = DateTime.Now
                    };

                    await _context.AddAsync(conceptos);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
        }


        public Facturas desglozaXML(FacturaCreateCommand factura, string newDate)
        {
            Facturas facturas = new Facturas();
            string newPath = "";

            newPath = Directory.GetCurrentDirectory() + "\\Carga XML";

            XmlDocument doc = new XmlDocument();
            doc.Load(newPath + "\\" + factura.XML.FileName);
            try
            {
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
                namespaceManager.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                namespaceManager.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
                namespaceManager.AddNamespace("ext", "http://www.buzone.com.mx/XSD/Addenda/EMEBuzWS");

                XmlNodeList ndComprobante = doc.SelectNodes("//cfdi:Comprobante", namespaceManager);
                if (ndComprobante.Count == 0)
                {
                    namespaceManager.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/4");
                    namespaceManager.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
                    namespaceManager.AddNamespace("ext", "http://www.buzone.com.mx/XSD/Addenda/EMEBuzWS");
                    ndComprobante = doc.SelectNodes("//cfdi:Comprobante", namespaceManager);
                }

                XmlNodeList ndEmisor = doc.SelectNodes("//cfdi:Emisor", namespaceManager);
                XmlNodeList ndCFDIRelacionado = doc.SelectNodes("//cfdi:CfdiRelacionado", namespaceManager);
                XmlNodeList ndConcepto = doc.SelectNodes("//cfdi:Conceptos/cfdi:Concepto", namespaceManager);
                XmlNodeList nDTimbrado = doc.SelectNodes("//cfdi:Complemento/tfd:TimbreFiscalDigital", namespaceManager);
                XmlNodeList nDExtras = doc.SelectNodes("//cfdi:Addenda/ext:ElementosExtra/ext:DatosGenerales", namespaceManager);
                XmlNodeList nDTraslado = doc.SelectNodes("//cfdi:Traslados/cfdi:Traslado", namespaceManager);
                XmlNodeList nDTotales = doc.SelectNodes("//cfdi:Addenda/ext:ElementosExtra/ext:DatosTotales", namespaceManager);
                XmlNodeList nDReceptor = doc.SelectNodes("//cfdi:Receptor", namespaceManager);
                XmlNodeList nDRetenciones = doc.SelectNodes("//cfdi:Retencion", namespaceManager);

                string jsonComprobante = JsonConvert.SerializeXmlNode(ndComprobante[0], Newtonsoft.Json.Formatting.None, true);
                string jsonEmisor = JsonConvert.SerializeXmlNode(ndEmisor[0], Newtonsoft.Json.Formatting.None, true);
                string jsonTraslado = JsonConvert.SerializeXmlNode(nDTraslado[0], Newtonsoft.Json.Formatting.None, true);
                string jsonTimbrado = JsonConvert.SerializeXmlNode(nDTimbrado[0], Newtonsoft.Json.Formatting.None, true);
                string jsonExtra = JsonConvert.SerializeXmlNode(nDExtras[0], Newtonsoft.Json.Formatting.None, true);
                string jsonTotales = JsonConvert.SerializeXmlNode(nDTotales[0], Newtonsoft.Json.Formatting.None, true);
                string jsonReceptor = JsonConvert.SerializeXmlNode(nDReceptor[0], Newtonsoft.Json.Formatting.None, true);
                string jsonRetenciones = JsonConvert.SerializeXmlNode(nDRetenciones[0], Newtonsoft.Json.Formatting.None, true);
                string jsonCFDIRelacionado = "";
                if (ndCFDIRelacionado.Count != 0)
                    jsonCFDIRelacionado = JsonConvert.SerializeXmlNode(ndCFDIRelacionado[0], Newtonsoft.Json.Formatting.None, true);


                List<Concepto> listaConcepto = new List<Concepto>();
                //Concepto concept = null;
                for (int i = 0; i < ndConcepto.Count; i++)
                {
                    string jsonConcepto = JsonConvert.SerializeXmlNode(ndConcepto[i], Newtonsoft.Json.Formatting.None, true);
                    jsonConcepto = jsonConcepto.Replace("@", "");
                    Concepto concept = JsonConvert.DeserializeObject<Concepto>(jsonConcepto);
                    listaConcepto.Add(concept);
                }

                List<Traslado> listaTraslado = new List<Traslado>();
                decimal iva = 0;
                for (int i = 0; i < nDTraslado.Count - 1; i++)
                {
                    string jsonTraslado2 = JsonConvert.SerializeXmlNode(nDTraslado[i], Newtonsoft.Json.Formatting.None, true);
                    jsonTraslado2 = jsonTraslado2.Replace("@", "");
                    Traslado trasl = JsonConvert.DeserializeObject<Traslado>(jsonTraslado2);
                    iva += trasl.Importe;
                }

                jsonComprobante = jsonComprobante.Replace("@", "");
                jsonEmisor = jsonEmisor.Replace("@", "");
                jsonTraslado = jsonTraslado.Replace("@", "");
                jsonTimbrado = jsonTimbrado.Replace("@", "");
                jsonReceptor = jsonReceptor.Replace("@", "");
                jsonCFDIRelacionado = jsonCFDIRelacionado.Replace("@", "");

                Comprobante comp = JsonConvert.DeserializeObject<Comprobante>(jsonComprobante);
                Emisor emisor = JsonConvert.DeserializeObject<Emisor>(jsonEmisor);
                Traslado traslado = JsonConvert.DeserializeObject<Traslado>(jsonTraslado);
                TimbreFiscal tFiscal = JsonConvert.DeserializeObject<TimbreFiscal>(jsonTimbrado);
                Receptor receptor = JsonConvert.DeserializeObject<Receptor>(jsonReceptor);
                CFDIRelacionado cfdiRelacionado = JsonConvert.DeserializeObject<CFDIRelacionado>(jsonCFDIRelacionado);

                traslado.Importe = iva;


                facturas.Comprobante = comp;
                facturas.Emisor = emisor;
                facturas.Traslado = traslado;
                facturas.Conceptos = listaConcepto;
                facturas.TimbreFiscal = tFiscal;
                facturas.Receptor = receptor;
                facturas.CfdiRelacionado = cfdiRelacionado;

                facturas.Archivo = factura.XML.FileName;

                facturas.Tipo = facturas.Receptor.UsoCFDI.Equals("G03") ? "Factura" : "NC";
                facturas.InmuebleId = factura.InmuebleId;
                facturas.UsuarioId = factura.UsuarioId;
                facturas.RepositorioId = factura.RepositorioId;
                facturas.Facturacion = factura.TipoFacturacion;
                facturas.Archivo = factura.XML.FileName;

                return facturas;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<bool> copiaFactura(IFormFile factura)
        {
            string newPath = "";

            newPath = Directory.GetCurrentDirectory() + "\\Carga XML";

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            using (var stream = new FileStream(newPath + "\\" + factura.FileName, FileMode.Create))
            {
                try
                {
                    await factura.CopyToAsync(stream);
                    return true;
                }
                catch (Exception ex)
                {
                    string msg = ex.Message.ToString();
                    return false;
                }
            }
        }

        public async Task<bool> copiaFacturaXML(IFormFile factura, int anio, string mes, string inmueble, string tipo, string folio)
        {
            string newPath = "";
            if (tipo.Equals("NC"))
            {
                newPath = Directory.GetCurrentDirectory() + "\\Repositorio\\" + anio + "\\" + mes + "\\" + inmueble + "\\Notas de Crédito\\" + folio;
            }
            else
            {
                newPath = Directory.GetCurrentDirectory() + "\\Repositorio\\" + anio + "\\" + mes + "\\" + inmueble + "\\Facturas\\" + folio;
            }

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            using (var stream = new FileStream(newPath + "\\" + factura.FileName, FileMode.Create))
            {
                try
                {
                    await factura.CopyToAsync(stream);
                    return true;
                }
                catch (Exception ex)
                {
                    string msg = ex.Message.ToString();
                    return false;
                }
            }
        }
    }
}

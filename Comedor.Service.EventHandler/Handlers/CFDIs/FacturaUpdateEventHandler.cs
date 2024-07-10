using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.Facturas;
using MediatR;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Comedor.Domain.DFacturas;

namespace Comedor.Service.EventHandler.Handlers.CFDIs
{
    public class FacturaUpdateEventHandler : IRequestHandler<FacturaUpdateCommand, Factura>
    {
        private readonly ApplicationDbContext _context;

        public FacturaUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<Factura> Handle(FacturaUpdateCommand request, CancellationToken cancellationToken)
        {
            string dateNow = DateTime.Now.ToString("yyyyMMddHHmmss");
            var file = request.PDF.FileName.Split(".")[0];
            Factura xmlExists = _context.Facturas.SingleOrDefault(f => f.ArchivoXML.Contains(file));
            Factura factura = _context.Facturas.SingleOrDefault(f => f.Id == xmlExists.Id);

            if (xmlExists != null && xmlExists.ArchivoPDF == null)
            {
                await copiaFacturaPDF(request.PDF, request.Anio, request.Mes, request.Inmueble, xmlExists.Tipo, xmlExists.Serie + xmlExists.Folio);

                factura.ArchivoPDF = request.PDF.FileName;
                factura.FechaActualizacion = DateTime.Now;

                await _context.SaveChangesAsync();

                factura.EstatusId = 201;
            }
            else if (request.PDF.FileName.Equals(xmlExists.ArchivoPDF))
            {
                factura.EstatusId = 205;   
            }
            else
            {
                factura.EstatusId = 400;
            }

            return factura;
        }

        public string BuscaPDF(int anio, string mes, string inmueble, string tipo, string folio, string archivo)
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
            
            string pathArchivo = Path.Combine(newPath, archivo);

            if (System.IO.File.Exists(pathArchivo))
            {
                return pathArchivo;
            }
            return "";
        }
        
        public async Task<bool> copiaFacturaPDF(IFormFile factura, int anio, string mes, string inmueble, string tipo, string folio)
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

    using MediatR;
using Comedor.Domain.DCedulaEvaluacion;
using Comedor.Domain.DOficios;
using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.Oficios;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Comedor.Service.EventHandler.Handlers.Oficios
{
    public class OficioCreateEventHandler : IRequestHandler<OficioCreateCommand, Oficio>
    {
        private readonly ApplicationDbContext _context;

        public OficioCreateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Oficio> Handle(OficioCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var path = CargaArchivoExcel(request.Oficio, request.NumeroOficio.Replace("\"", "_").ToString());

                if (TotalHojasExcel(path)){
                    var oficio = new Oficio
                    {
                        ContratoId = request.ContratoId,
                        UsuarioId = request.UsuarioId,
                        Anio = request.Anio,
                        NumeroOficio = request.NumeroOficio.Replace("\"", "_"),
                        FechaTramitado = request.FechaTramitado,
                        FechaCreacion = DateTime.Now
                    };

                    await _context.AddAsync(oficio);
                    await _context.SaveChangesAsync();

                    request.Id = oficio.Id;

                    var detalles = await GetDatosExcel(path, request);

                    return detalles.Count() != 0 ? oficio : null;
                }

                return null;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public bool TotalHojasExcel(string path)
        {
            var constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0;HDR=NO'";
            DataTable datatable = new DataTable();
            int totalHojasExcel = 0;

            constr = string.Format(constr, path);

            using (OleDbConnection excelconn = new OleDbConnection(constr))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    using (OleDbDataAdapter adapterexcel = new OleDbDataAdapter())
                    {

                        excelconn.Open();
                        cmd.Connection = excelconn;
                        DataTable excelschema;
                        excelschema = excelconn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        totalHojasExcel = excelconn.GetSchema("TABLES").AsEnumerable()
                                        .Select(r => r.Field<string>("TABLE_NAME"))
                                        .Where(n => !n.Contains("#"))
                                        .Count();
                        excelconn.Close();
                    }
                }
            }

            return totalHojasExcel == 1 ? true: false;
        }

        public async Task<List<DetalleOficio>> GetDatosExcel(string path, OficioCreateCommand request)
        {
            var constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0;HDR=NO'";
            DataTable datatable = new DataTable();

            constr = string.Format(constr, path);

            using (OleDbConnection excelconn = new OleDbConnection(constr))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    using (OleDbDataAdapter adapterexcel = new OleDbDataAdapter())
                    {

                        excelconn.Open();
                        cmd.Connection = excelconn;
                        DataTable excelschema;
                        excelschema = excelconn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        var sheetname = excelschema.Rows[0]["Table_Name"].ToString();
                        excelconn.Close();

                        excelconn.Open();
                        cmd.CommandText = "SELECT * From ["+sheetname+"A10:P1000]";
                        adapterexcel.SelectCommand = cmd;
                        adapterexcel.Fill(datatable);
                        excelconn.Close();
                    }
                }
            }

            return await GetModelIncidencias(datatable, request);
        }

        public async Task<List<DetalleOficio>> GetModelIncidencias(DataTable excel, OficioCreateCommand oficio)
        {
            DetalleOficio detalle = null;

            foreach (DataRow row in excel.Rows)
            {
                if (row[2] != DBNull.Value)
                {
                    detalle = new DetalleOficio
                    {
                        ServicioId = oficio.ServicioId,
                        OficioId = oficio.Id,
                        FacturaId = GetFacturaId(Convert.ToInt64(row[2])),
                        CedulaId = GetCedula(Convert.ToInt64(row[2])),
                    };
                    await _context.AddAsync(detalle);
                    await _context.SaveChangesAsync();
                }

                if (row[13] != DBNull.Value)
                {
                    detalle = new DetalleOficio
                    {
                        ServicioId = oficio.ServicioId,
                        OficioId = oficio.Id,
                        FacturaId = GetFacturaId(Convert.ToInt64(row[13])),
                        CedulaId = GetCedula(Convert.ToInt64(row[13])),
                    };
                    await _context.AddAsync(detalle);
                    await _context.SaveChangesAsync();
                }
            }

            var detalles = _context.DetalleOficios.Where(dt => dt.OficioId == oficio.Id).ToList();
            
            return detalles;
        }

        public int GetFacturaId(long folio)
        {
            try
            {
                var factura = _context.Facturas.SingleOrDefault(f => f.Folio == folio);

                return factura.Id;
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }
        
        public int GetCedula(long folio)
        {
            try
            {
                var factura = _context.Facturas.Single(f => f.Folio == folio);
                var Repositorio = _context.Repositorios.Single(f => f.Id == factura.RepositorioId);

                var cedula = _context.CedulaEvaluacion.Single(c => Repositorio.MesId == c.MesId && Repositorio.Anio == c.Anio
                                                                && factura.InmuebleId == c.InmuebleId && Repositorio.ContratoId == c.ContratoId);
                
                return cedula.Id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }

        private string CargaArchivoExcel(IFormFile fromFiles, string folio)
        {
            string newPath = Directory.GetCurrentDirectory() + "\\Oficios\\" + folio;
            string dest_path = Path.Combine(newPath, "Oficios");

            if (!Directory.Exists(dest_path))
            {
                Directory.CreateDirectory(dest_path);
            }
            string sourcefile = Path.GetFileName(fromFiles.FileName);
            string path = Path.Combine(dest_path, sourcefile);

            using (FileStream filestream = new FileStream(path, FileMode.Create))
            {
                fromFiles.CopyTo(filestream);
            }
            return path;
        }
    }
}

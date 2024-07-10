using Comedor.Persistence.Database;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using Comedor.Domain.DEntregables;
using Comedor.Service.EventHandler.Commands.Entregables.Update;

namespace Comedor.Service.EventHandler.Handlers.Entregables.Update
{
    public class EntregableUpdateEventHandler : IRequestHandler<EntregableUpdateCommand, Entregable>
    {
        private readonly ApplicationDbContext _context;

        public EntregableUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Entregable> Handle(EntregableUpdateCommand request, CancellationToken cancellationToken)
        {
            Entregable entregable = _context.Entregables.Where(e => e.Id == request.Id && !e.FechaEliminacion.HasValue).FirstOrDefault();
            string newDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            DateTime fechaActualizacion = DateTime.Now;

            if (request.Archivo != null)
            {
                if (entregable.Archivo != null && !entregable.Archivo.Equals(""))
                {
                    eliminaArchivoActual(request.Anio, request.Mes, request.Folio, entregable.Archivo, request.TipoEntregable);
                }

                if (await guardaArchivo(request.Archivo, request.Anio, request.Mes, request.Folio, newDate, request.TipoEntregable))
                {
                    entregable.UsuarioId = request.UsuarioId;
                    entregable.EstatusId = request.EstatusId;
                    entregable.Archivo = newDate + "_" + request.Archivo.FileName;
                    entregable.Observaciones = request.Observaciones;
                    entregable.FechaActualizacion = DateTime.Now;
                }
            }
            else
            {
                entregable.EstatusId = request.EstatusId;
                entregable.Observaciones = request.Observaciones;
                entregable.FechaActualizacion = fechaActualizacion;
            }

            if (request.Validar)
            {
                entregable.Validado = request.Validado;
            }

            await _context.SaveChangesAsync();

            return entregable;
        }

        public async Task<bool> guardaArchivo(IFormFile archivo, int anio, string mes, string folio, string fecha, string tipoEntregable)
        {
            long size = archivo.Length;

            string newPath = Directory.GetCurrentDirectory() + "\\Entregables\\" + anio + "" + "\\" + mes + "\\" + folio+"\\"+tipoEntregable;
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            using (var stream = new FileStream(newPath + "\\" + fecha + "_" + archivo.FileName, FileMode.Create))
            {
                try
                {
                    await archivo.CopyToAsync(stream);
                    return true;
                }
                catch (Exception ex)
                {
                    string msg = ex.Message.ToString();
                    return false;
                }
            }
        }

        public bool eliminaArchivoActual(int anio, string mes, string folio, string archivo, string tipoEntregable)
        {
            string newPath = Directory.GetCurrentDirectory() + "\\Entregables\\" + anio + "" + "\\" + mes + "\\" + folio+"\\"+tipoEntregable;

            newPath += "\\" + archivo;

            try
            {
                File.Delete(newPath);
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

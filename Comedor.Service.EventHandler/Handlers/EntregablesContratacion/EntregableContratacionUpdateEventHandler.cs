using Comedor.Persistence.Database;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using Comedor.Service.EventHandler.Commands.EntregableContratacion;
using System.Text.RegularExpressions;
using Comedor.Domain.DEntregablesContratacion;

namespace Comedor.Service.EventHandler.Handlers.EntregablesContratacion
{
    public class EntregableContratacionUpdateEventHandler : IRequestHandler<EntregableContratacionUpdateCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public EntregableContratacionUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(EntregableContratacionUpdateCommand request, CancellationToken cancellationToken)
        {
            EntregableContratacion entregable = _context.EntregableContratacion.Where(e => e.Id == request.Id).FirstOrDefault();
            string newDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            if (request.Archivo != null)
            {
                if (entregable.Archivo != null && !entregable.Archivo.Equals(""))
                {
                    eliminaArchivoActual(request.Contrato, request.Convenio, request.TipoEntregable, entregable.Archivo);
                }

                if (await guardaArchivo(request.Archivo, request.Contrato, request.Convenio, request.TipoEntregable, newDate))
                {
                    entregable.UsuarioId = request.UsuarioId;
                    entregable.Archivo = newDate + "_" + request.Archivo.FileName;
                    entregable.FechaProgramada = request.FechaProgramada;
                    entregable.FechaEntrega = request.FechaEntrega;
                    entregable.InicioVigencia = request.InicioVigencia;
                    entregable.FinVigencia = request.FinVigencia;
                    entregable.MontoGarantia = request.MontoGarantia;
                    entregable.Penalizable = request.Penalizable;
                    entregable.MontoPenalizacion = request.MontoPenalizacion;
                    entregable.Observaciones = request.Observaciones;
                    entregable.FechaActualizacion = DateTime.Now;
                }
            }
            else
            {
                entregable.UsuarioId = request.UsuarioId;
                entregable.FechaProgramada = request.FechaProgramada;
                entregable.FechaEntrega = request.FechaEntrega;
                entregable.InicioVigencia = request.InicioVigencia;
                entregable.FinVigencia = request.FinVigencia;
                entregable.MontoGarantia = request.MontoGarantia;
                entregable.Penalizable = request.Penalizable;
                entregable.MontoPenalizacion = request.MontoPenalizacion;
                entregable.Observaciones = request.Observaciones;
                entregable.FechaActualizacion = DateTime.Now;
            }

            try
            {
                await _context.SaveChangesAsync();
                return 201;
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return 500;
            }
        }

        public async Task<bool> guardaArchivo(IFormFile archivo, string contrato, string convenio, string tipoEntregable, string fecha)
        {
            long size = archivo.Length;
            string newPath = "";
            var regex = "[^0-9A-Za-z_ ]";
            contrato = Regex.Replace(contrato, regex, "_");
            if (convenio != null && !convenio.Equals(""))
            {
                convenio = Regex.Replace(convenio, regex, "_");
                newPath = Directory.GetCurrentDirectory() + "\\Entregables Contratos\\" + contrato + "\\" + convenio + "\\" + tipoEntregable;
            }
            else
            {
                newPath = Directory.GetCurrentDirectory() + "\\Entregables Contratos\\" + contrato + "\\" + tipoEntregable;
            }

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

        public bool eliminaArchivoActual(string contrato, string convenio, string tipoEntregable, string archivo)
        {
            string newPath = "";
            var regex = "[^0-9A-Za-z_ ]";
            contrato = Regex.Replace(contrato, regex, "_");
            if (convenio != null && !convenio.Equals(""))
            {
                convenio = Regex.Replace(convenio, regex, "_");
                newPath = Directory.GetCurrentDirectory() + "\\Entregables Contratos\\" + contrato + "\\" + convenio + "\\" + tipoEntregable;
            }
            else
            {
                newPath = Directory.GetCurrentDirectory() + "\\Entregables Contratos\\" + contrato + "\\" + tipoEntregable;
            }

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

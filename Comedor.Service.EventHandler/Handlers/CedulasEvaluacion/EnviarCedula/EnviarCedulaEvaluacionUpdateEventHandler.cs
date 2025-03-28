using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using Comedor.Persistence.Database;
using Comedor.Domain.DCedulaEvaluacion;
using Comedor.Domain.DFacturas;
using Comedor.Domain.DIncidencias;
using Comedor.Domain.DCuestionario;
using Comedor.Service.EventHandler.Commands.CedulasEvaluacion;

namespace Comedor.Service.EventHandler.Handlers.CedulasEvaluacion
{
    public class EnviarCedulaEvaluacionUpdateEventHandler : IRequestHandler<EnviarCedulaEvaluacionUpdateCommand, CedulaEvaluacion>
    {
        private readonly ApplicationDbContext _context;

        public EnviarCedulaEvaluacionUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CedulaEvaluacion> Handle(EnviarCedulaEvaluacionUpdateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                CedulaEvaluacion cedula = _context.CedulaEvaluacion.FirstOrDefault(c => c.Id == request.Id);

                if (request.Calcula)
                {
                    List<Factura> facturas = _context.Facturas
                                                                   .Where(f => f.RepositorioId == request.RepositorioId &&
                                                                               f.InmuebleId == cedula.InmuebleId && f.Tipo.Equals("Factura")
                                                                               && f.Facturacion.Equals("Mensual"))
                                                                   .ToList();

                    List<Factura> notasCredito = _context.Facturas
                                                               .Where(f => f.RepositorioId == request.RepositorioId &&
                                                                           f.InmuebleId == cedula.InmuebleId && f.Tipo.Equals("NC")
                                                                           && f.Facturacion.Equals("Mensual"))
                                                               .ToList();

                    List<CuestionarioMensual> cuestionarioMensual = _context.CuestionarioMensual
                                                                .Where(cm => cm.Anio == cedula.Anio && cm.MesId == cedula.MesId && cm.ContratoId == cedula.ContratoId)
                                                                .ToList();

                    var respuestas = await Obtienetotales(request.Id, cuestionarioMensual);
                    var calificacion = await GetCalificacionCedula(request.Id, cuestionarioMensual);

                    cedula.EstatusId = request.EstatusId;
                    if (calificacion < 10)
                    {
                        //string calif = calificacion.ToString().Substring(0, 3);
                        //cedula.Calificacion = Convert.ToDouble(calif);

                        string calif = (Math.Round(calificacion, 1)).ToString();
                        cedula.Calificacion = Convert.ToDouble(calif);
                    }
                    else
                    {
                        cedula.Calificacion = (double)calificacion;
                    }
                    
                    cedula.FechaActualizacion = DateTime.Now;
                    
                    await _context.SaveChangesAsync();

                    foreach (var fac in facturas)
                    {
                        fac.EstatusId = request.EFacturaId;

                        await _context.SaveChangesAsync();
                    }

                    foreach (var fac in notasCredito)
                    {
                        fac.EstatusId = request.ENotaCreditoId;

                        await _context.SaveChangesAsync();
                    }
                }

                return cedula;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }        
        
        private async Task<List<RespuestaEvaluacion>> Obtienetotales(int cedula, List<CuestionarioMensual> cuestionario)
        {
            var incidencias = new List<Incidencia>();

            foreach (var cm in cuestionario)
            {
                incidencias = _context.Incidencias.Where(i => i.Pregunta == cm.Consecutivo && i.CedulaEvaluacionId == cedula
                                                            && !i.FechaEliminacion.HasValue).ToList();

                decimal totalDetalleIncidencias = GetTotalDetalleIncidencias(incidencias);

                var respuesta = _context.Respuestas.SingleOrDefault(r => r.CedulaEvaluacionId == cedula && r.Pregunta == cm.Consecutivo);

                if (!respuesta.Detalles.Equals("N/A") && !respuesta.Detalles.Equals("N/E") && !respuesta.Detalles.Equals("N/R")) {
                    respuesta.Detalles = incidencias.Count() + "";
                }
                respuesta.Penalizable = (incidencias.Sum(i => i.MontoPenalizacion) != 0 ? true : false);
                respuesta.MontoPenalizacion = totalDetalleIncidencias+(incidencias.Sum(i => i.MontoPenalizacion) != 0 ? incidencias.Sum(i => i.MontoPenalizacion) : 0);
                respuesta.FechaActualizacion = DateTime.Now;

                await _context.SaveChangesAsync();
            }

            var respuestas = _context.Respuestas.ToList();

            return respuestas;
        }

        private decimal GetTotalDetalleIncidencias(List<Incidencia> incidencias)
        {
            decimal total = 0;

            foreach (var inc in incidencias)
            {
                var dtIncidencias = _context.DetalleIncidencias.Where(d => d.IncidenciaId == inc.Id).Count();
                
                if (dtIncidencias != 0)
                {
                    total += _context.DetalleIncidencias.Where(d => d.IncidenciaId == inc.Id).Sum(di => di.MontoPenalizacion);
                }
            }

            return total;
        }

        private async Task<decimal> GetCalificacionCedula(int cedula, List<CuestionarioMensual> cuestionario)
        {
            decimal calificacion = 0;
            decimal ponderacion = 0;
            bool calidad = true;
            var incidencias = 0;

            var respuestas = _context.Respuestas.Where(r => r.CedulaEvaluacionId == cedula).ToList();

            foreach (var rs in respuestas)
            {
                var cm = cuestionario.Single(c => c.Consecutivo == rs.Pregunta);
                if (cm.ACLRS == rs.Respuesta && !rs.Detalles.Equals("N/A") && !rs.Detalles.Equals("N/E") && !rs.Detalles.Equals("N/R") && rs.MontoPenalizacion != 0)
                {
                    calidad = false;
                    incidencias = _context.Incidencias.Where(i => i.CedulaEvaluacionId == cedula && i.Pregunta == cm.Consecutivo && !i.FechaEliminacion.HasValue).Count();

                    var listIncidencias = _context.Incidencias.Where(i => i.CedulaEvaluacionId == cedula &&
                                                                      i.Pregunta == cm.Consecutivo &&
                                                                      !i.FechaEliminacion.HasValue).ToList();

                    ponderacion = Convert.ToDecimal(cm.Ponderacion) / Convert.ToDecimal(2);

                    calificacion += ponderacion;

                    rs.Detalles = incidencias+"";
                    rs.Penalizable = true;
                    rs.MontoPenalizacion = (listIncidencias.Count() != 0 ? listIncidencias.Sum(i => i.MontoPenalizacion):0);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    calificacion += Convert.ToDecimal(cm.Ponderacion);
                    rs.Penalizable = false;
                    rs.MontoPenalizacion = 0;
                }
            }

            calificacion = Convert.ToDecimal(calificacion / respuestas.Count());

            return calidad ? calificacion + (decimal)1 : calificacion;
        }
    }
}

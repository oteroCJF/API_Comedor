using Comedor.Domain.DFacturas;
using Comedor.Domain.DIncidencias;
using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.Incidencias;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Comedor.Service.EventHandler.Handlers.Incidencias
{
    public class UpdateIncidenciaEventHandler : IRequestHandler<IncidenciaUpdateCommand, Incidencia>
    {
        private readonly ApplicationDbContext _context;

        public UpdateIncidenciaEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Incidencia> Handle(IncidenciaUpdateCommand request, CancellationToken cancellationToken)
        {

            var incidencia = _context.Incidencias.Where(e => e.Id == request.Id && !e.FechaEliminacion.HasValue).FirstOrDefault();

            var factura = GetFactura(request);

            var montoPenalizacion = GetPenaDeductiva(request, factura);

            incidencia.UsuarioId = request.UsuarioId;
            incidencia.CedulaEvaluacionId = request.CedulaEvaluacionId;
            incidencia.IncidenciaId = request.IncidenciaId;
            incidencia.TipoId = request.TipoId;
            incidencia.Pregunta = request.Pregunta;
            incidencia.FechaIncidencia = request.FechaIncidencia;
            incidencia.FechaProgramada = request.FechaProgramada;
            incidencia.UltimoDia = request.UltimoDia;
            incidencia.FechaRealizada = request.FechaRealizada;
            incidencia.FechaInventario = request.FechaInventario;
            incidencia.FechaNotificacion = request.FechaNotificacion;
            incidencia.FechaAcordadaAdmin = request.FechaAcordadaAdmin;
            incidencia.FechaEntrega = request.FechaEntrega;
            incidencia.HoraInicio = Convert.ToDateTime(request.HoraInicio).TimeOfDay;
            incidencia.HoraReal = Convert.ToDateTime(request.HoraReal).TimeOfDay;
            incidencia.Cantidad = request.Cantidad;
            incidencia.Ponderacion = request.Ponderacion;
            incidencia.Observaciones = request.Observaciones;
            incidencia.Penalizable = montoPenalizacion > 0 ? true : false;
            incidencia.MontoPenalizacion = montoPenalizacion;
            incidencia.FechaActualizacion = DateTime.Now;

            try
            {
                if (request.DTIncidencia.Count() != 0)
                {
                    List<DetalleIncidencia> detalles = await _context.DetalleIncidencias.Where(p => p.IncidenciaId == request.Id).ToListAsync();
                    _context.DetalleIncidencias.RemoveRange(detalles);

                    await _context.SaveChangesAsync();

                    decimal totalDI = 0;
                    foreach (var dt in request.DTIncidencia)
                    {
                        var dtIncidencia = new DetalleIncidencia();
                        dtIncidencia.IncidenciaId = incidencia.Id;
                        dtIncidencia.CIncidenciaId = dt;
                        dtIncidencia.MontoPenalizacion = GetPrecioUnitarioServicio(request, factura) * Convert.ToDecimal(0.01)*incidencia.Cantidad;
                        totalDI += dtIncidencia.MontoPenalizacion;

                        await _context.AddAsync(dtIncidencia);
                        await _context.SaveChangesAsync();
                    }

                    var Uincidencia = _context.Incidencias.Where(e => e.Id == incidencia.Id && !e.FechaEliminacion.HasValue).FirstOrDefault();
                    Uincidencia.MontoPenalizacion = totalDI;
                }
                await _context.SaveChangesAsync();


                return incidencia;
            }
            catch (Exception ex)
            {
                string msg = ex.Message + "\n" + ex.StackTrace + "\n" + ex.InnerException;
                return null;
            }
        }

        public decimal GetPenaDeductiva(IncidenciaUpdateCommand incidencia, Factura factura)
        {
            decimal montoPenalizacion = 0;
            CreateIncidenciaEventHandler creaIncidencia = new CreateIncidenciaEventHandler();

            var cedula = _context.CedulaEvaluacion.Single(ce => ce.Id == incidencia.CedulaEvaluacionId);

            var respuesta = _context.Respuestas.Single(r => r.Pregunta == incidencia.Pregunta && r.CedulaEvaluacionId == incidencia.CedulaEvaluacionId);

            var cuestionario = _context.CuestionarioMensual.Single(cm => cm.Consecutivo == incidencia.Pregunta &&
                                                                         cm.Anio == cedula.Anio &&
                                                                         cm.MesId == cedula.MesId &&
                                                                         cm.ContratoId == cedula.ContratoId);
            if (respuesta.Respuesta == cuestionario.ACLRS)
            {
                if (cuestionario.Formula.Contains("CDAS"))
                {
                    montoPenalizacion = GetCostoDiaAnteriorServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.Cantidad;
                }
                else if (cuestionario.Formula.Contains("CUS"))
                {
                    montoPenalizacion = GetPrecioUnitarioServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.Cantidad;
                }
                else if (cuestionario.Formula.Contains("CDS"))
                {
                    montoPenalizacion = GetCostoDiaServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje);
                    if (cuestionario.Formula.Contains("NMR"))
                    {
                        montoPenalizacion = montoPenalizacion * GetMinutosRetraso(incidencia);
                    }
                    else if (cuestionario.Formula.Contains("NMFR"))
                    {
                        montoPenalizacion = montoPenalizacion * incidencia.Cantidad;
                    }
                }
                else if (cuestionario.Formula.Contains("ENSERESA"))
                {
                    var fechaEntrega = incidencia.FechaEntrega;
                    if (!incidencia.EntregaEnseres)
                    {
                        fechaEntrega = GetUltimoDiaHabilMes(incidencia.FechaLimite);
                    }
                    var fechaLimite = incidencia.FechaLimite;
                    TimeSpan diffDate = fechaEntrega - fechaLimite;
                    //   var diasAtraso = diffDate.Days;
                    var diasAtraso = creaIncidencia.CalcularDiasHabiles(fechaLimite, fechaEntrega);

                    montoPenalizacion = GetCostoFechaInventario(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje);
                    montoPenalizacion = diasAtraso <= 0 ? montoPenalizacion : montoPenalizacion * diasAtraso;
                }
                else if (cuestionario.Formula.Contains("ENSERESB"))
                {
                    var fechaEntrega = incidencia.FechaEntrega;
                    if (!incidencia.EntregaEnseres)
                    {
                        fechaEntrega = GetUltimoDiaHabilMes(incidencia.FechaLimite);
                    }
                    var fechaLimite = incidencia.FechaLimite;
                    TimeSpan diffDate = fechaEntrega - fechaLimite;
                    //var diasAtraso = diffDate.Days;
                    var diasAtraso = creaIncidencia.CalcularDiasHabiles(fechaLimite, fechaEntrega);
                    

                    montoPenalizacion = GetCostoFechaInventario(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.Cantidad;
                    montoPenalizacion = diasAtraso <= 0 ? montoPenalizacion : montoPenalizacion * diasAtraso;
                }
                else if (cuestionario.Formula.Contains("ENSERESC"))
                {
                    var fechaEntrega = incidencia.FechaEntrega;
                    if (!incidencia.EntregaEnseres)
                    {
                        fechaEntrega = GetUltimoDiaHabilMes(incidencia.FechaLimite);
                    }
                    var fechaAcordada = incidencia.FechaAcordadaAdmin;
                    TimeSpan diffDate = fechaEntrega - fechaAcordada;
                    var diasAtraso = diffDate.Days;

                    montoPenalizacion = GetCostoFechaInventario(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.Cantidad;
                    montoPenalizacion = diasAtraso <= 0 ? montoPenalizacion : montoPenalizacion * diasAtraso;
                }
                else if (cuestionario.Formula.Contains("CFS"))
                {
                    var ultimoDia = incidencia.UltimoDia;
                    var fechaProgramada = incidencia.FechaProgramada;
                    TimeSpan diffDate = ultimoDia - fechaProgramada;
                    var diasAtraso = diffDate.Days;

                    montoPenalizacion = GetCostoSemanal(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * diasAtraso;
                }
                else if (cuestionario.Formula.Contains("CTPS"))
                {
                    montoPenalizacion = GetCostoDiaServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.Ponderacion * incidencia.Cantidad;
                }
            }

            return montoPenalizacion;
        }


        public Factura GetFactura(IncidenciaUpdateCommand incidencia)
        {
            var cedula = _context.CedulaEvaluacion.Single(ce => ce.Id == incidencia.CedulaEvaluacionId);
            var repositorio = _context.Repositorios.Single(r => r.Anio == cedula.Anio && r.ContratoId == cedula.ContratoId
                                                                && r.MesId == cedula.MesId);

            var facturas = new Factura();

            if (incidencia.Pregunta == 25 || incidencia.Pregunta == 26 || incidencia.Pregunta == 27)
            {
                facturas = _context.Facturas.Single(f => f.RepositorioId == (repositorio.Id - 1) &&
                                                            cedula.InmuebleId == f.InmuebleId && f.Tipo.Equals("Factura") && f.FechaEliminacion == null);
            }
            else
            {
                facturas = _context.Facturas.Single(f => f.RepositorioId == repositorio.Id &&
                                                                cedula.InmuebleId == f.InmuebleId && f.Tipo.Equals("Factura") && f.FechaEliminacion == null);
            }
            return facturas;
        }

        public decimal GetPrecioUnitarioServicio(IncidenciaUpdateCommand incidencia, Factura factura)
        {
            var conceptosFactura = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id).First();

            return conceptosFactura.PrecioUnitario;
        }

        public decimal GetCostoDiaServicio(IncidenciaUpdateCommand incidencia, Factura factura)
        {
            var conceptosFactura = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id && c.FechaServicio == incidencia.FechaIncidencia).Sum(f => f.Subtotal);

            return conceptosFactura;
        }

        public decimal GetCostoDiaAnteriorServicio(IncidenciaUpdateCommand incidencia, Factura factura)
        {
            var diaAnterior = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id && c.FechaServicio < incidencia.FechaIncidencia)
                                .OrderByDescending(cf => cf.FechaServicio).FirstOrDefault();
            decimal conceptosFactura = 0;

            if (diaAnterior != null)
            {
                conceptosFactura = diaAnterior.Subtotal;
            }
            else
            {
                conceptosFactura = _context.ConceptosFactura.Single(c => c.FacturaId == factura.Id && c.FechaServicio == incidencia.FechaIncidencia).Subtotal;
            }

            return conceptosFactura;
        }

        public decimal GetCostoFechaInventario(IncidenciaUpdateCommand incidencia, Factura factura)
        {
            var conceptosFactura = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id && c.FechaServicio == incidencia.FechaInventario).Sum(f => f.Subtotal);

            return conceptosFactura;
        }

        public int GetMinutosRetraso(IncidenciaUpdateCommand incidencia)
        {
            var horaInicio = Convert.ToDateTime(incidencia.HoraInicio);
            var horaReal = Convert.ToDateTime(incidencia.HoraReal);

            var diffMinutes = horaReal.Minute - horaInicio.Minute;
            var minutos = 0;

            if (diffMinutes > 15)
            {
                minutos = diffMinutes - 15;
            }

            return minutos;
        }

        public decimal GetCostoSemanal(IncidenciaUpdateCommand incidencia, Factura factura)
        {
            var fechaInicio = GetFechaInicialCostoSemanal(incidencia, factura);
            var fechaFinal = GetFechaFinalCostoSemanal(factura, fechaInicio);
            var costoSemanal = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id && c.FechaServicio >= fechaInicio && c.FechaServicio <= fechaFinal).Sum(f => f.Subtotal);

            return costoSemanal;
        }

        public DateTime GetFechaInicialCostoSemanal(IncidenciaUpdateCommand incidencia, Factura factura)
        {
            var fechaInicial = new DateTime();
            var fechas = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id);
            bool inhabil = false;
            foreach (var fc in fechas)
            {
                DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(fc.FechaServicio);
                if (day == DayOfWeek.Monday)
                {
                    inhabil = false;
                    fechaInicial = fc.FechaServicio;
                }


                if (fc.FechaServicio == incidencia.UltimoDia)
                {
                    break;
                }
            }
            return fechaInicial;
        }

        public DateTime GetFechaFinalCostoSemanal(Factura factura, DateTime fechaInicial)
        {
            var fechaFinal = new DateTime();
            var fechas = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id && c.FechaServicio >= fechaInicial);
            bool inhabil = false;
            foreach (var fc in fechas)
            {
                DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(fc.FechaServicio);
                if (day == DayOfWeek.Friday)
                {
                    fechaInicial = fc.FechaServicio;
                    break;
                }
            }
            return fechaInicial;
        }

        public DateTime GetUltimoDiaHabilMes(DateTime fechaInicial)
        {
            DateTime inicioMes = new DateTime(fechaInicial.Year, fechaInicial.Month, 1);
            DateTime lastDayMonth = inicioMes.AddMonths(1).AddDays(-1);
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(lastDayMonth);

            if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
            {
                while (true)
                {
                    lastDayMonth = lastDayMonth.AddDays(-1);
                    day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(lastDayMonth);
                    if (day != DayOfWeek.Saturday && day != DayOfWeek.Sunday)
                    {
                        break;
                    }
                }
            }

            return lastDayMonth;
        }
    }
}

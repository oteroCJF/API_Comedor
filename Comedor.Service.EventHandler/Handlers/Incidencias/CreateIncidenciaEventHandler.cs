using Comedor.Domain.DFacturas;
using Comedor.Domain.DIncidencias;
using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.Incidencias;
using MediatR;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Comedor.Service.EventHandler.Handlers.Incidencias
{
    public class CreateIncidenciaEventHandler : IRequestHandler<IncidenciaCreateCommand, Incidencia>
    {
        private readonly ApplicationDbContext _context;

        public CreateIncidenciaEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Incidencia> Handle(IncidenciaCreateCommand request, CancellationToken cancellationToken)
        {
            var factura = GetFactura(request);

            var montoPenalizacion = GetPenaDeductiva(request, factura);

            var incidencia = new Incidencia
            {
                UsuarioId = request.UsuarioId,
                CedulaEvaluacionId = request.CedulaEvaluacionId,
                IncidenciaId = request.IncidenciaId,
                TipoId = request.TipoId,
                Pregunta = request.Pregunta,
                FechaIncidencia = request.FechaIncidencia,
                FechaProgramada= request.FechaProgramada,
                UltimoDia = request.UltimoDia,
                FechaRealizada = request.FechaRealizada,
                FechaInventario = request.FechaInventario,
                FechaNotificacion = request.FechaNotificacion,
                FechaAcordadaAdmin = request.FechaAcordadaAdmin,
                FechaLimite = request.FechaLimite,
                FechaEntrega = request.FechaEntrega,
                EntregaEnseres = request.EntregaEnseres,
                HoraInicio = Convert.ToDateTime(request.HoraInicio).TimeOfDay,
                HoraReal = Convert.ToDateTime(request.HoraReal).TimeOfDay,
                Ponderacion = request.Ponderacion,
                Cantidad = request.Cantidad,
                Observaciones = request.Observaciones,
                Penalizable = montoPenalizacion > 0 ? true : false,
                MontoPenalizacion = montoPenalizacion,
                FechaCreacion = DateTime.Now
            };

            try
            {
                await _context.AddAsync(incidencia);
                await _context.SaveChangesAsync();

                if (request.DTIncidencia.Count() != 0)
                {
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

                    await _context.SaveChangesAsync();
                }

                return incidencia;
            }
            catch (Exception ex)
            {
                string msg = ex.Message + "\n" + ex.StackTrace + "\n" + ex.InnerException;
                return null;
            }
        }

        public decimal GetPenaDeductiva(IncidenciaCreateCommand incidencia, Factura factura)
        {
            decimal montoPenalizacion = 0;
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
                    montoPenalizacion = GetCostoDiaAnteriorServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje)*incidencia.Cantidad;
                }
                else if (cuestionario.Formula.Contains("CUS"))
                {
                    montoPenalizacion = GetPrecioUnitarioServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje)*incidencia.Cantidad;
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
                    var fechaLimite = incidencia.FechaLimite;
                    TimeSpan diffDate = fechaEntrega - fechaLimite;
                    var diasAtraso = diffDate.Days;

                    montoPenalizacion = GetCostoFechaInventario(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje);
                    montoPenalizacion = diasAtraso <= 0 ? montoPenalizacion : montoPenalizacion * diasAtraso;
                }
                else if (cuestionario.Formula.Contains("ENSERESB"))
                {
                    var fechaEntrega = incidencia.FechaEntrega;
                    var fechaLimite = incidencia.FechaLimite;
                    TimeSpan diffDate = fechaEntrega - fechaLimite;
                    var diasAtraso = diffDate.Days;

                    montoPenalizacion = GetCostoFechaInventario(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.Cantidad;
                    montoPenalizacion = diasAtraso <= 0 ? montoPenalizacion : montoPenalizacion * diasAtraso;
                }
                else if (cuestionario.Formula.Contains("ENSERESC"))
                {
                    var fechaEntrega = incidencia.FechaEntrega;
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
                    var montoFactura = GetCostoSemanal(incidencia, factura);
                    var porcentaje = Convert.ToDecimal(cuestionario.Porcentaje);
                    montoPenalizacion =  (montoFactura *  porcentaje)* diasAtraso;
                }
                else if (cuestionario.Formula.Contains("CTPS"))
                {
                    montoPenalizacion = GetCostoDiaServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.Ponderacion * incidencia.Cantidad;
                }
            }

            return montoPenalizacion;
        }

        public Factura GetFactura(IncidenciaCreateCommand incidencia)
        {
            var cedula = _context.CedulaEvaluacion.Single(ce => ce.Id == incidencia.CedulaEvaluacionId);
            var repositorio = _context.Repositorios.Single(r => r.Anio == cedula.Anio && r.ContratoId == cedula.ContratoId
                                                                && r.MesId == cedula.MesId);
            var facturas = _context.Facturas.Single(f => f.RepositorioId == repositorio.Id &&
                                                            cedula.InmuebleId == f.InmuebleId && f.Tipo.Equals("Factura") && !f.FechaEliminacion.HasValue);

            return facturas;
        }

        public decimal GetPrecioUnitarioServicio(IncidenciaCreateCommand incidencia, Factura factura)
        {
            var conceptosFactura = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id).First();

            return conceptosFactura.PrecioUnitario;
        }
        
        public decimal GetCostoDiaServicio(IncidenciaCreateCommand incidencia, Factura factura)
        {
            var conceptosFactura = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id && c.FechaServicio == incidencia.FechaIncidencia).Sum(f => f.Subtotal);

            return conceptosFactura;
        }

        public decimal GetCostoDiaAnteriorServicio(IncidenciaCreateCommand incidencia, Factura factura)
        {
            var diaAnterior = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id && c.FechaServicio < incidencia.FechaIncidencia).OrderByDescending(o =>o.FechaServicio).First();
            decimal conceptosFactura = 0;

            if(diaAnterior != null)
            {
                conceptosFactura = diaAnterior.Subtotal;
            }
            else
            {
                conceptosFactura = _context.ConceptosFactura.Single(c => c.FacturaId == factura.Id && c.FechaServicio == incidencia.FechaIncidencia).Subtotal;
            }

            return conceptosFactura;
        }

        public decimal GetCostoFechaInventario(IncidenciaCreateCommand incidencia, Factura factura)
        {
            var conceptosFactura = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id && c.FechaServicio == incidencia.FechaInventario).Sum(f => f.Subtotal);

            return conceptosFactura;
        }

        public int GetMinutosRetraso(IncidenciaCreateCommand incidencia)
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

        public decimal GetCostoSemanal(IncidenciaCreateCommand incidencia, Factura factura)
        {
            var fechaInicio = GetFechaInicialCostoSemanal(incidencia, factura);
            var fechaFinal = GetFechaFinalCostoSemanal(factura, fechaInicio);
            var costoSemanal = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id && c.FechaServicio >= fechaInicio && c.FechaServicio <= fechaFinal).Sum(f => f.Subtotal);

            return costoSemanal;
        }

        public DateTime GetFechaInicialCostoSemanal(IncidenciaCreateCommand incidencia, Factura factura)
        {
            var fechaInicial = new DateTime();
            var fechas = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id);
            bool inhabil = false;
            foreach(var fc in fechas)
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

    }
}

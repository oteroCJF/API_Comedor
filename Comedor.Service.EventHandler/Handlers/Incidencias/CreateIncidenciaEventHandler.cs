﻿using Comedor.Domain.DFacturas;
using Comedor.Domain.DIncidencias;
using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.Incidencias;
using MediatR;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;

namespace Comedor.Service.EventHandler.Handlers.Incidencias
{
    public class CreateIncidenciaEventHandler : IRequestHandler<IncidenciaCreateCommand, Incidencia>
    {
        private readonly ApplicationDbContext _context;

        public CreateIncidenciaEventHandler() { }
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
                FechaProgramada = request.FechaProgramada,
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
                        dtIncidencia.MontoPenalizacion = GetPrecioUnitarioServicio(request, factura) * Convert.ToDecimal(0.01) * incidencia.Cantidad;
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
                    montoPenalizacion = GetCostoDiaAnteriorServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.Cantidad;
                }
                else if (cuestionario.Formula.Contains("CUS"))
                {
                    //AGREGAR MINUTOS DE RETRASO PARA EL CALCULO DE LA PENALIZACIÓN DESPUÉS DE LAS DOS HORAS
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
                    var diasAtraso = CalcularDiasHabiles(fechaLimite, fechaEntrega);

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
                    var diasAtraso = CalcularDiasHabiles(fechaLimite, fechaEntrega);

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
                    var montoFactura = GetCostoSemanal(incidencia, factura);
                    var porcentaje = Convert.ToDecimal(cuestionario.Porcentaje);
                    montoPenalizacion = (montoFactura * porcentaje) * diasAtraso;
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
            var facturas = new Factura();

            if (incidencia.Pregunta == 25 || incidencia.Pregunta == 26 || incidencia.Pregunta == 27)
            {
                facturas = _context.Facturas.Single(f => (f.RepositorioId) == (repositorio.Id - 1) &&
                                                            cedula.InmuebleId == f.InmuebleId && f.Tipo.Equals("Factura") && f.FechaEliminacion == null);
            }
            else
            {
                facturas = _context.Facturas.Single(f => f.RepositorioId == repositorio.Id &&
                                                    cedula.InmuebleId == f.InmuebleId &&
                                                    f.Tipo.Equals("Factura") && f.FechaEliminacion == null);
            }
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
            var diaAnterior = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id && c.FechaServicio < incidencia.FechaIncidencia).OrderByDescending(o => o.FechaServicio).First();
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

        //METODOS PARA CALCULAR LOS DÍAS INHABILES (TEMPORAL) PARA EL 2024
        public  int CalcularDiasHabiles(DateTime fechaInicio, DateTime fechaFin)
        {
            int diasHabiles = 0;


            for (; true;)
            {
                fechaInicio = fechaInicio.AddDays(1);

                if (!EsFinDeSemana(fechaInicio) && !EsInhabil(fechaInicio))
                {
                    diasHabiles++;
                }

                if (fechaInicio == fechaFin)
                {
                    break;
                }
            }

            return diasHabiles;
        }

        
        private bool EsFinDeSemana(DateTime fecha)
        {
            return fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday;
        }

        private bool EsInhabil(DateTime fecha)
        {
            List<DateTime> diasInhabiles = new List<DateTime>()
            {
                  new DateTime(2024, 01, 01),
                  new DateTime(2024, 02, 05),
                  new DateTime(2024, 03, 18),
                  new DateTime(2024, 03, 27),
                  new DateTime(2024, 03, 28),
                  new DateTime(2024, 03, 29),
                  new DateTime(2024, 05, 01),
                  new DateTime(2024, 07, 16),
                  new DateTime(2024, 07, 17),
                  new DateTime(2024, 07, 18),
                  new DateTime(2024, 07, 19),
                  new DateTime(2024, 07, 22),
                  new DateTime(2024, 07, 23),
                  new DateTime(2024, 07, 24),
                  new DateTime(2024, 07, 25),
                  new DateTime(2024, 07, 26),
                  new DateTime(2024, 07, 29),
                  new DateTime(2024, 07, 30),
                  new DateTime(2024, 07, 31),
                               new DateTime(2024, 09, 16),
                               new DateTime(2024, 10, 01),
                               new DateTime(2024, 11, 01),
                               new DateTime(2024, 11, 18),
                               new DateTime(2024, 12, 16),
                               new DateTime(2024, 12, 17),
                               new DateTime(2024, 12, 18),
                               new DateTime(2024, 12, 19),
                               new DateTime(2024, 12, 20),
                               new DateTime(2024, 12, 23),
                               new DateTime(2024, 12, 24),
                               new DateTime(2024, 12, 25),
                               new DateTime(2024, 12, 26),
                               new DateTime(2024, 12, 27),
                               new DateTime(2024, 12, 30),
                               new DateTime(2024, 12, 31)
            };
            if (diasInhabiles.Contains(fecha))
            {
                Debug.WriteLine("ENCONTRÉ UN DÍA INHABIL:" + fecha);
            }
            return diasInhabiles.Contains(fecha);
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

        //  METODO PARA CALCULAR EL NUMERO DE DÍAS HABILES ENTRE DOS FECHAS

        //public async Task<JsonResult> CalculaDiasHabilesEntreDosFechas(string fechaLim, string fechaEnt)
        //{
        //    int cuentaDiasHabiles = 0;
        //    var fechaLimite = Convert.ToDateTime(fechaLim);
        //    var fechaEntrega = Convert.ToDateTime(fechaEnt);

        //    List<DateTime> diasInhabiles = new List<DateTime>()
        //    {
        //         new DateTime(2024, 01, 01),
        //          new DateTime(2024, 02, 05),
        //          new DateTime(2024, 03, 18),
        //          new DateTime(2024, 05, 01),
        //          new DateTime(2024, 07, 16),
        //          new DateTime(2024, 07, 17),
        //          new DateTime(2024, 07, 18),
        //          new DateTime(2024, 07, 19),
        //          new DateTime(2024, 07, 23),
        //          new DateTime(2024, 07, 24),
        //          new DateTime(2024, 07, 25),
        //          new DateTime(2024, 07, 26),
        //          new DateTime(2024, 07, 22),
        //          new DateTime(2024, 07, 29),
        //          new DateTime(2024, 07, 30),
        //          new DateTime(2024, 07, 31),
        //                     new DateTime(2024, 09, 16),
        //                      new DateTime(2024, 11, 18),
        //                       new DateTime(2024, 12, 16),
        //                       new DateTime(2024, 12, 17),
        //                       new DateTime(2024, 12, 18),
        //                       new DateTime(2024, 12, 19),
        //                       new DateTime(2024, 12, 20),
        //                       new DateTime(2024, 12, 23),
        //                       new DateTime(2024, 12, 24),
        //                       new DateTime(2024, 12, 25),
        //                       new DateTime(2024, 12, 26),
        //                       new DateTime(2024, 12, 27),
        //                       new DateTime(2024, 12, 30),
        //                       new DateTime(2024, 12, 31)
        //    };

        //    for (; true;)
        //    {
        //        fechaLimite = fechaLimite.AddDays(1);
        //        DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(fechaLimite);

        //        if (!await.EsDiaInhabil(fechaLimite.Year, fechaLimite.ToString("yyyy-MM-ddTHH:mm:ss")) && day != DayOfWeek.Saturday && DayOfWeek.Sunday != day)
        //        {
        //            cuentaDiasHabiles++;
        //        }

        //        if (fechaLimite == fechaEntrega)
        //        {
        //            break;
        //        }
        //    }

        //    return new JsonResult(cuentaDiasHabiles);
        //}
    }
}

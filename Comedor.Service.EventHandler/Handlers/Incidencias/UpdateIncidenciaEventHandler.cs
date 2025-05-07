using Comedor.Domain.DFacturas;
using Comedor.Domain.DIncidencias;
using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.Incidencias;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            if (cedula.ContratoId >= 3)
            {
                if (respuesta.Respuesta == cuestionario.ACLRS)
                {
                    //PREGUNTA 2 - NUEVO CONTRATO
                    if (cuestionario.Formula.Contains("CDIA*PP*NDA"))
                    {
                        montoPenalizacion = GetCostoDiaAnteriorServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje);
                    }
                    // PREGUNTA 25 - NUEVO CONTRATO 
                    else if (cuestionario.Formula.Contains("CFDIA*PD"))
                    {
                        var costoDiaAnterior = GetCostoDiaAnteriorServicio(incidencia, factura);
                        montoPenalizacion = costoDiaAnterior * Convert.ToDecimal(cuestionario.Porcentaje);

                    }
                    else if (cuestionario.Formula.Contains("CFD"))
                    {
                        if (cuestionario.Formula == "CFD*PP*NDA")
                        {
                            //PREGUNTA 3, 17,  - NUEVO CONTRATO 2025

                            if (respuesta.Pregunta != 23 && respuesta.Pregunta != 26 && respuesta.Pregunta != 29 && respuesta.Pregunta != 30)
                            {
                                //PREGUNTA 3, 17,  - NUEVO CONTRATO 2025
                                if (respuesta.Pregunta == 3)
                                {
                                    var diasAtraso = (incidencia.FechaEntrega - incidencia.FechaLimite).Days;
                                    incidencia.FechaIncidencia = incidencia.FechaProgramada;
                                    montoPenalizacion = GetCostoDiaServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * diasAtraso;

                                }
                                //PREGUNTA 17 - NUEVO CONTRATO 2025
                                else
                                {
                                    var diasAtraso = (incidencia.FechaEntrega - incidencia.FechaProgramada).Days;
                                    incidencia.FechaIncidencia = incidencia.FechaProgramada;
                                    montoPenalizacion = GetCostoDiaServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * diasAtraso;
                                }
                            }
                            //PREGUNTA 29, 30 - NUEVO CONTRATO 2025
                            else if (respuesta.Pregunta == 29 || respuesta.Pregunta == 30)
                            {
                                incidencia.FechaIncidencia = incidencia.FechaLimite;
                                var diasNatAtraso = (incidencia.FechaEntrega - incidencia.FechaLimite).Days;
                                montoPenalizacion = GetCostoDiaServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * diasNatAtraso;
                            }
                            //PREGUNTA 23, 26 - NUEVO CONTRATO 2025
                            else
                            {
                                montoPenalizacion = GetCostoDiaServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje);
                            }

                        }
                        //PREGUNTA 5,6,7,8,9,10,11,12,13,14,15,24,27,28 - NUEVO CONTRATO 2025
                        else if (cuestionario.Formula == "CFD*PD")
                        {
                            montoPenalizacion = GetCostoDiaServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje);
                        }
                        //PREGUNTA 18 - NUEVO CONTRATO 2025
                        else if (cuestionario.Formula == "CFD*PD*NMFR")
                        {
                            montoPenalizacion = GetCostoDiaServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.Cantidad;
                        }
                        //PREGUNTA 19 - NUEVO CONTRATO 2025
                        else if (cuestionario.Formula == "CFD*PD*NPRFR")
                        {
                            montoPenalizacion = GetCostoDiaServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.Cantidad;
                        }
                        //PREGUNTA 20 - NUEVO CONTRATO 2025
                        else if (cuestionario.Formula == "CFD*PD*NDEF")
                        {
                            montoPenalizacion = GetCostoFechaInventario(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * CalcularDiasHabiles(incidencia.FechaLimite, incidencia.FechaEntrega);
                        }
                        //PREGUNTA 21,22 - NUEVO CONTRATO 2025
                        else if (cuestionario.Formula == "CFD*CEF*PD*NDEF")
                        {
                            incidencia.FechaIncidencia = incidencia.FechaNotificacion;
                            var diasHabilesAtraso = CalcularDiasHabiles(incidencia.FechaLimite, incidencia.FechaEntrega);
                            montoPenalizacion = GetCostoDiaServicio(incidencia, factura) * incidencia.Cantidad * Convert.ToDecimal(cuestionario.Porcentaje) * diasHabilesAtraso;
                        }
                    }
                    //PREGUNTA 4 - NUEVO CONTRATO 2025
                    else if (cuestionario.Formula.Contains("CPU*PD*NCI"))
                    {
                        montoPenalizacion = GetPrecioUnitarioServicio(incidencia, factura) * (Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.DTIncidencia.Count());
                    }
                    //PREGUNTA 16 - NUEVO CONTRATO 2025
                    else if (cuestionario.Formula.Contains("CFS*PP*NDA"))
                    {
                        var diasAtraso = (incidencia.FechaEntrega - incidencia.FechaLimite).Days;
                        incidencia.FechaIncidencia = incidencia.FechaLimite;
                        montoPenalizacion = GetCostoSemanalNuevoContrato(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * diasAtraso;
                    }
                    //PREGUNTA 31 - NUEVO CONTRATO
                    else if (cuestionario.Formula.Contains("CQDA*PP*NDA"))
                    {
                        var diasAtraso = (incidencia.FechaEntrega - incidencia.FechaIncidencia).Days;
                        montoPenalizacion = GetCostoDiaServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * diasAtraso;
                    }
                    //PREGUNTA 32 - NUEVO CONTRATO
                    else if (cuestionario.Formula.Contains("CFMI*PP*NDA"))
                    {
                        var diasAtraso = (incidencia.FechaEntrega - incidencia.FechaIncidencia).Days;
                        montoPenalizacion = GetCostoDiaServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * diasAtraso;
                    }


                }

                return montoPenalizacion;

            }
            else
            {

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
            }

            return montoPenalizacion;
        }

        //    if (respuesta.Respuesta == cuestionario.ACLRS)
        //    {
        //        if (cuestionario.Formula.Contains("CDAS"))
        //        {
        //            montoPenalizacion = GetCostoDiaAnteriorServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.Cantidad;
        //        }
        //        else if (cuestionario.Formula.Contains("CUS"))
        //        {
        //            montoPenalizacion = GetPrecioUnitarioServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.Cantidad;
        //        }
        //        else if (cuestionario.Formula.Contains("CDS"))
        //        {
        //            montoPenalizacion = GetCostoDiaServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje);
        //            if (cuestionario.Formula.Contains("NMR"))
        //            {
        //                montoPenalizacion = montoPenalizacion * GetMinutosRetraso(incidencia);
        //            }
        //            else if (cuestionario.Formula.Contains("NMFR"))
        //            {
        //                montoPenalizacion = montoPenalizacion * incidencia.Cantidad;
        //            }
        //        }
        //        else if (cuestionario.Formula.Contains("ENSERESA"))
        //        {
        //            var fechaEntrega = incidencia.FechaEntrega;
        //            if (!incidencia.EntregaEnseres)
        //            {
        //                fechaEntrega = GetUltimoDiaHabilMes(incidencia.FechaLimite);
        //            }
        //            var fechaLimite = incidencia.FechaLimite;
        //            TimeSpan diffDate = fechaEntrega - fechaLimite;
        //            //   var diasAtraso = diffDate.Days;
        //            var diasAtraso = creaIncidencia.CalcularDiasHabiles(fechaLimite, fechaEntrega);

        //            montoPenalizacion = GetCostoFechaInventario(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje);
        //            montoPenalizacion = diasAtraso <= 0 ? montoPenalizacion : montoPenalizacion * diasAtraso;
        //        }
        //        else if (cuestionario.Formula.Contains("ENSERESB"))
        //        {
        //            var fechaEntrega = incidencia.FechaEntrega;
        //            if (!incidencia.EntregaEnseres)
        //            {
        //                fechaEntrega = GetUltimoDiaHabilMes(incidencia.FechaLimite);
        //            }
        //            var fechaLimite = incidencia.FechaLimite;
        //            TimeSpan diffDate = fechaEntrega - fechaLimite;
        //            //var diasAtraso = diffDate.Days;
        //            var diasAtraso = creaIncidencia.CalcularDiasHabiles(fechaLimite, fechaEntrega);


        //            montoPenalizacion = GetCostoFechaInventario(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.Cantidad;
        //            montoPenalizacion = diasAtraso <= 0 ? montoPenalizacion : montoPenalizacion * diasAtraso;
        //        }
        //        else if (cuestionario.Formula.Contains("ENSERESC"))
        //        {
        //            var fechaEntrega = incidencia.FechaEntrega;
        //            if (!incidencia.EntregaEnseres)
        //            {
        //                fechaEntrega = GetUltimoDiaHabilMes(incidencia.FechaLimite);
        //            }
        //            var fechaAcordada = incidencia.FechaAcordadaAdmin;
        //            TimeSpan diffDate = fechaEntrega - fechaAcordada;
        //            var diasAtraso = diffDate.Days;

        //            montoPenalizacion = GetCostoFechaInventario(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.Cantidad;
        //            montoPenalizacion = diasAtraso <= 0 ? montoPenalizacion : montoPenalizacion * diasAtraso;
        //        }
        //        else if (cuestionario.Formula.Contains("CFS"))
        //        {
        //            var ultimoDia = incidencia.UltimoDia;
        //            var fechaProgramada = incidencia.FechaProgramada;
        //            TimeSpan diffDate = ultimoDia - fechaProgramada;
        //            var diasAtraso = diffDate.Days;

        //            montoPenalizacion = GetCostoSemanal(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * diasAtraso;
        //        }
        //        else if (cuestionario.Formula.Contains("CTPS"))
        //        {
        //            montoPenalizacion = GetCostoDiaServicio(incidencia, factura) * Convert.ToDecimal(cuestionario.Porcentaje) * incidencia.Ponderacion * incidencia.Cantidad;
        //        }
        //    }

        //    return montoPenalizacion;
        //}


        public Factura GetFactura(IncidenciaUpdateCommand incidencia)
        {
            var cedula = _context.CedulaEvaluacion.Single(ce => ce.Id == incidencia.CedulaEvaluacionId);
            var repositorio = _context.Repositorios.Single(r => r.Anio == cedula.Anio && r.ContratoId == cedula.ContratoId
                                                                && r.MesId == cedula.MesId);

            var facturas = new Factura();
            if (cedula.ContratoId == 2)
            {
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
            else
            {
                facturas = _context.Facturas.Single(f => f.RepositorioId == repositorio.Id &&
                                                    cedula.InmuebleId == f.InmuebleId &&
                                                    f.Tipo.Equals("Factura") && f.FechaEliminacion == null);
            }
            return facturas;
        }

        //    if (incidencia.Pregunta == 25 || incidencia.Pregunta == 26 || incidencia.Pregunta == 27)
        //    {
        //        facturas = _context.Facturas.Single(f => f.RepositorioId == (repositorio.Id - 1) &&
        //                                                    cedula.InmuebleId == f.InmuebleId && f.Tipo.Equals("Factura") && f.FechaEliminacion == null);
        //    }
        //    else
        //    {
        //        facturas = _context.Facturas.Single(f => f.RepositorioId == repositorio.Id &&
        //                                                        cedula.InmuebleId == f.InmuebleId && f.Tipo.Equals("Factura") && f.FechaEliminacion == null);
        //    }
        //    return facturas;
        //}


        //Método para comparar el mes de la Fecha de Incidencia ingresada con el Mes
        public bool ComparaMeses(IncidenciaCreateCommand incidencia)
        {
            bool aux;
            var cedula = _context.CedulaEvaluacion.Single(ce => ce.Id == incidencia.CedulaEvaluacionId);

            if (incidencia.FechaIncidencia.Month != cedula.MesId)
            {
                aux = false;
            }
            else
            {
                aux = true;
            }

            return aux;
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

        //METODOS PARA CALCULAR LOS DÍAS INHABILES (TEMPORAL) PARA EL 2024
        public int CalcularDiasHabiles(DateTime fechaInicio, DateTime fechaFin)
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
                               new DateTime(2024, 12, 31),

                //DIAS INHABILES DEL 2025
                  new DateTime(2025, 01, 01),
                  new DateTime(2025, 02, 03),
                  new DateTime(2025, 03, 17),
                  new DateTime(2025, 04, 16),
                  new DateTime(2025, 04, 17),
                  new DateTime(2025, 04, 18),
                  new DateTime(2025, 05, 01),
                  new DateTime(2025, 05, 02),
                  new DateTime(2025, 05, 05),
                  new DateTime(2025, 07, 16),
                  new DateTime(2025, 07, 17),
                  new DateTime(2025, 07, 18),
                  new DateTime(2025, 07, 21),
                  new DateTime(2025, 07, 22),
                  new DateTime(2025, 07, 23),
                  new DateTime(2025, 07, 24),
                  new DateTime(2025, 07, 25),
                  new DateTime(2025, 07, 28),
                  new DateTime(2025, 07, 29),
                  new DateTime(2025, 07, 30),
                  new DateTime(2025, 07, 31),
                               new DateTime(2025, 09, 16),
                               new DateTime(2025, 11, 17),
                               new DateTime(2025, 12, 16),
                               new DateTime(2025, 12, 17),
                               new DateTime(2025, 12, 18),
                               new DateTime(2025, 12, 19),
                               new DateTime(2025, 12, 20),
                               new DateTime(2025, 12, 23),
                               new DateTime(2025, 12, 24),
                               new DateTime(2025, 12, 25),
                               new DateTime(2025, 12, 26),
                               new DateTime(2025, 12, 27),
                               new DateTime(2025, 12, 30),
                               new DateTime(2025, 12, 31)
            };
            if (diasInhabiles.Contains(fecha))
            {
                Debug.WriteLine("ENCONTRÉ UN DÍA INHABIL:" + fecha);
            }
            return diasInhabiles.Contains(fecha);
        }

        public decimal GetCostoSemanal(IncidenciaUpdateCommand incidencia, Factura factura)
        {
            var fechaInicio = GetFechaInicialCostoSemanal(incidencia, factura);
            var fechaFinal = GetFechaFinalCostoSemanal(factura, fechaInicio);
            var costoSemanal = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id && c.FechaServicio >= fechaInicio && c.FechaServicio <= fechaFinal).Sum(f => f.Subtotal);

            return costoSemanal;
        }

        public decimal GetCostoSemanalNuevoContrato(IncidenciaUpdateCommand incidencia, Factura factura)
        {
            var fechaInicio = GetFechaInicialCostoSemanalNuevoContrato(incidencia, factura);
            var fechaFinal = GetFechaFinalCostoSemanalNuevoContrato(factura, fechaInicio, incidencia);
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

        public DateTime GetFechaFinalCostoSemanalNuevoContrato(Factura factura, DateTime fechaInicial, IncidenciaUpdateCommand incidencia)
        {
            var fechaFinal = new DateTime();
            var fechas = _context.ConceptosFactura.Where(c => c.FacturaId == factura.Id && c.FechaServicio >= incidencia.FechaLimite);
            bool inhabil = false;
            foreach (var fc in fechas)
            {
                DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(fc.FechaServicio);
                if (day == DayOfWeek.Friday)
                {
                    fechaFinal = fc.FechaServicio;
                    break;
                }
            }
            return fechaFinal;
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

        public DateTime GetFechaInicialCostoSemanalNuevoContrato(IncidenciaUpdateCommand incidencia, Factura factura)
        {
            var fechaInicial = new DateTime();
            var fechas = _context.ConceptosFactura
                 .Where(c => c.FacturaId == factura.Id && c.FechaServicio <= incidencia.FechaLimite)
                 .OrderByDescending(c => c.FechaServicio) // Ordenar para encontrar el lunes más cercano hacia atrás
                 .ToList();

            bool inhabil = false;


            foreach (var fc in fechas)
            {
                DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(fc.FechaServicio);

                if (day == DayOfWeek.Monday)
                {
                    fechaInicial = fc.FechaServicio;
                    break; // Encontramos un lunes, podemos detenernos
                }
                else
                {
                    // Si la fecha de servicio actual es la fecha límite y no es lunes,
                    // disminuimos un día y verificamos si es lunes.
                    DateTime fechaAnterior = fc.FechaServicio.AddDays(-1);
                }
            }

            return fechaInicial;
        }
    }
}

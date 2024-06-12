using MediatR;
using Comedor.Domain.DCedulaEvaluacion;
using Comedor.Domain.DFacturas;
using Comedor.Domain.DHistorial;
using Comedor.Domain.DOficios;
using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.Oficios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Comedor.Service.EventHandler.Handlers.Oficios
{
    public class EDGPPTOficioEventHandler : IRequestHandler<EDGPPTOficioCommand, Oficio>
    {
        private readonly ApplicationDbContext _context;

        public EDGPPTOficioEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Oficio> Handle(EDGPPTOficioCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.FechaCreacion = DateTime.Now;
                var oficio = _context.Oficios.Single(o => o.Id == request.Id);
                List<Factura> facturas = await EFacturasDGPPT(request);
                List<CedulaEvaluacion> cedulas = await ECedulasDGPPT(request);

                //var dtOficio = await EliminaDetalleOficio(request.Id);

                if (cedulas != null && facturas != null)
                {
                    oficio.EstatusId = request.ESucesivoId;
                    oficio.FechaActualizacion = request.FechaCreacion;

                    await _context.SaveChangesAsync();
                }
                else
                {
                    oficio = null;
                }

                return oficio;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<List<Factura>> EFacturasDGPPT(EDGPPTOficioCommand request)
        {
            try
            {
                var facturasId = _context.DetalleOficios.Where(dt => dt.OficioId == request.Id).Select(dt => dt.FacturaId).ToList();
                var facturas = _context.Facturas.Where(f => facturasId.Contains(f.Id)).ToList();

                foreach (var fc in facturas)
                {
                    fc.EstatusId = request.EFacturaId;
                    fc.FechaActualizacion = request.FechaCreacion;
                    await _context.SaveChangesAsync();
                }

                return facturas;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<List<CedulaEvaluacion>> ECedulasDGPPT(EDGPPTOficioCommand request)
        {
            try
            {
                var Oficio = _context.Oficios.Single(o => o.Id == request.Id);
                var cedulasId = _context.DetalleOficios.Where(dt => dt.OficioId == request.Id).Select(dt => dt.CedulaId).ToList();
                var cedulas = _context.CedulaEvaluacion.Where(c => cedulasId.Contains(c.Id)).ToList();

                foreach (var c in cedulas)
                {
                    c.EstatusId = request.ECedulaId;
                    c.FechaActualizacion= request.FechaCreacion;

                    LogCedula logCedula = new LogCedula
                    { 
                        CedulaEvaluacionId = c.Id,
                        UsuarioId = request.UsuarioId,
                        EstatusId = request.ECedulaId,
                        Observaciones = "Se envía a <b>DGPPT</b> la cédula de evaluación el día <b>" + Convert.ToDateTime(Oficio.FechaTramitado).ToString("dd/MM/yyyy") + "</b> mediante el oficio " +
                        "<b>" + Oficio.NumeroOficio + "</b>.",
                        FechaCreacion = request.FechaCreacion
                    };

                    await _context.AddAsync(logCedula);
                    await _context.SaveChangesAsync();
                }

                return cedulas;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}

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
    public class PagarOficioEventHandler : IRequestHandler<PagarOficioCommand, Oficio>
    {
        private readonly ApplicationDbContext _context;

        public PagarOficioEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Oficio> Handle(PagarOficioCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.FechaCreacion = DateTime.Now;
                var oficio = _context.Oficios.Single(o => o.Id == request.Id);
                List<Factura> facturas = await PagarFacturas(request);
                List<CedulaEvaluacion> cedulas = await PagarCedulas(request);

                //var dtOficio = await EliminaDetalleOficio(request.Id);

                if (cedulas != null && facturas != null)
                {
                    oficio.EstatusId = request.ESucesivoId;
                    oficio.FechaPagado = request.FechaPago;
                    oficio.FechaActualizacion = request.FechaCreacion;

                    await _context.SaveChangesAsync();
                }

                return oficio;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<List<Factura>> PagarFacturas(PagarOficioCommand request)
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

        public async Task<List<CedulaEvaluacion>> PagarCedulas(PagarOficioCommand request)
        {
            try
            {
                var Oficio = _context.Oficios.Single(o => o.Id == request.Id);
                var cedulasId = _context.DetalleOficios.Where(dt => dt.OficioId == request.Id).Select(dt => dt.CedulaId).ToList();
                var cedulas = _context.CedulaEvaluacion.Where(c => cedulasId.Contains(c.Id)).ToList();

                foreach (var c in cedulas)
                {
                    c.EstatusId = request.ECedulaId;
                    c.FechaActualizacion = request.FechaCreacion;

                    LogCedula logCedula = new LogCedula
                    {
                        CedulaEvaluacionId = c.Id,
                        UsuarioId = request.UsuarioId,
                        EstatusId = request.ECedulaId,
                        Observaciones = "Se relizo el pago de la(s) facturas de la cédula de evaluación el día <b>"+request.FechaPago.ToString("dd/MM/yyyy") + "</b> mediante el oficio " +
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

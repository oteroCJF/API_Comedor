using MediatR;
using Comedor.Domain.DCedulaEvaluacion;
using Comedor.Domain.DFacturas;
using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.CedulasEvaluacion.ActualizacionCedula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Comedor.Service.EventHandler.Handlers.CedulasEvaluacion
{
    public class CedulaEvaluacionUpdateEventHandler : IRequestHandler<CedulaEvaluacionUpdateCommand, CedulaEvaluacion>
    {
        private readonly ApplicationDbContext _context;

        public CedulaEvaluacionUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CedulaEvaluacion> Handle(CedulaEvaluacionUpdateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var cedula = _context.CedulaEvaluacion.Single(c => c.Id == request.Id);

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

                cedula.EstatusId = request.EstatusId;
                cedula.Bloqueada = request.Bloqueada;
                cedula.FechaActualizacion = request.FechaActualizacion;

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

                return cedula;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}

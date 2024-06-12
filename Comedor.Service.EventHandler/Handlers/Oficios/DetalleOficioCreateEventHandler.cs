using MediatR;
using Comedor.Domain.DOficios;
using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.Oficios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Comedor.Service.EventHandler.Handlers.Oficios
{
    public class DetalleOficioCreateEventHandler : IRequestHandler<DetalleOficioCreateCommand, DetalleOficio>
    {
        private readonly ApplicationDbContext _context;

        public DetalleOficioCreateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DetalleOficio> Handle(DetalleOficioCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var detalle = new DetalleOficio
                {
                    ServicioId = request.ServicioId,
                    OficioId = request.OficioId,
                    CedulaId = request.CedulaId,
                    FacturaId = request.FacturaId
                };

                await _context.AddAsync(detalle);
                await _context.SaveChangesAsync();

                return detalle;
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}

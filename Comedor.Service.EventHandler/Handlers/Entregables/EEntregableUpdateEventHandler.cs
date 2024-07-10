using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.Entregables;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using Comedor.Domain.DEntregables;

namespace Comedor.Service.EventHandler.Handlers.Entregables
{
    public class EEntregableUpdateEventHandler : IRequestHandler<EEntregableUpdateCommand, Entregable>
    {
        private readonly ApplicationDbContext _context;

        public EEntregableUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Entregable> Handle(EEntregableUpdateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Entregable entregable = _context.Entregables.Where(e => e.Id == request.Id && !e.FechaEliminacion.HasValue).FirstOrDefault();

                entregable.EstatusId = request.EstatusId;
                entregable.Observaciones = request.Observaciones;
                entregable.FechaActualizacion = request.FechaActualizacion;
                if (!Convert.ToDateTime(request.FechaEliminacion).ToString("dd/MM/yyyy").Equals("01/01/0001"))
                {
                    entregable.FechaEliminacion = request.FechaEliminacion;
                }

                await _context.SaveChangesAsync();

                return entregable;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}

using Comedor.Persistence.Database;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using Comedor.Domain.DEntregables;
using Comedor.Service.EventHandler.Commands.Entregables.Update;
using Comedor.Service.EventHandler.Commands.Entregables.Create;

namespace Comedor.Service.EventHandler.Handlers.Entregables.Create
{
    public class EntregableCreateEventHandler : IRequestHandler<EntregableCreateCommand, Entregable>
    {
        private readonly ApplicationDbContext _context;

        public EntregableCreateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Entregable> Handle(EntregableCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                DateTime fechaCreacion = DateTime.Now;

                var entregable = new Entregable
                {
                    CedulaEvaluacionId = request.CedulaEvaluacionId,
                    UsuarioId = request.UsuarioId,
                    EntregableId = request.EntregableId,
                    Observaciones = request.Observaciones,
                    FechaCreacion = fechaCreacion
                };

                await _context.AddAsync(entregable);
                await _context.SaveChangesAsync();

                return entregable;
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}

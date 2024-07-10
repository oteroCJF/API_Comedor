using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.Incidencias;
using Comedor.Service.EventHandler.Commands.LogCedulas;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Comedor.Domain.DHistorial;
using Comedor.Service.EventHandler.Commands.LogOficios;
using Comedor.Domain.DHistorialOficios;

namespace Comedor.Service.EventHandler.Handlers.LogOficios
{
    public class LogOficiosCreateEventHandler : IRequestHandler<LogOficiosCreateCommand, LogOficio>
    {
        private readonly ApplicationDbContext _context;

        public LogOficiosCreateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LogOficio> Handle(LogOficiosCreateCommand logs, CancellationToken cancellationToken)
        {
            try
            {
                var log = new LogOficio
                {
                    OficioId = logs.OficioId,
                    UsuarioId = logs.UsuarioId,
                    EstatusId = logs.EstatusId,
                    Observaciones = logs.Observaciones,
                    FechaCreacion = DateTime.Now
                };

                await _context.AddAsync(log);
                await _context.SaveChangesAsync();

                return log;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}

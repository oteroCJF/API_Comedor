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

namespace Comedor.Service.EventHandler.Handlers.LogCedulas
{
    public class LogCedulaCreateEventHandler : IRequestHandler<LogCedulasCreateCommand, LogCedula>
    {
        private readonly ApplicationDbContext _context;

        public LogCedulaCreateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LogCedula> Handle(LogCedulasCreateCommand logs, CancellationToken cancellationToken)
        {
            try
            {
                var log = new LogCedula
                {
                    CedulaEvaluacionId = logs.CedulaEvaluacionId,
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

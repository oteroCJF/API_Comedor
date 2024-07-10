using Comedor.Domain.DIncidencias;
using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.Incidencias;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Comedor.Service.EventHandler.Handlers.Incidencias
{
    public class DeleteIncidenciaEventHandler : IRequestHandler<IncidenciaDeleteCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public DeleteIncidenciaEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(IncidenciaDeleteCommand request, CancellationToken cancellationToken)
        {

            var incidencia = _context.Incidencias.Where(e => e.Id == request.Id && !e.FechaEliminacion.HasValue).FirstOrDefault();
            try
            {
                incidencia.FechaEliminacion = DateTime.Now;
                await _context.SaveChangesAsync();
                return incidencia.Id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message + "\n" + ex.StackTrace + "\n" + ex.InnerException;
                return -1;
            }
        }
    }
}

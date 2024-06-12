using MediatR;
using Comedor.Domain.DFacturas;
using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.CFDIs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Comedor.Service.EventHandler.Handlers.CFDIs
{
    public class HistorialMFDeleteEventHandler : IRequestHandler<HistorialMFDeleteCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public HistorialMFDeleteEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(HistorialMFDeleteCommand request, CancellationToken cancellationToken)
        {
            List<HistorialMF> historial = await _context.HistorialMF.Where(mf => mf.RepositorioId == request.RepositorioId).ToListAsync();

            _context.HistorialMF.RemoveRange(historial);

            try
            {
                await _context.SaveChangesAsync();

                return historial.Count;
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return 500;
            }
        }
    }
}

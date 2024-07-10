using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.Contratos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Comedor.Domain.DContratos;

namespace Comedor.Service.EventHandler.Handlers.Contratos
{
    public class ContratoDeleteEventHandler : IRequestHandler<ContratoDeleteCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public ContratoDeleteEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(ContratoDeleteCommand request, CancellationToken cancellationToken)
        {
            Contrato contrato = await _context.Contratos.SingleOrDefaultAsync(c => c.Id == request.Id);

            contrato.FechaEliminacion = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return 201;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 500;
            }
        }
    }
}

using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.ServiciosContrato;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Comedor.Service.EventHandler.Handlers.ServiciosContrato
{
    public class ServicioContratoDeleteEventHandler : IRequestHandler<ServicioContratoDeleteCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public ServicioContratoDeleteEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(ServicioContratoDeleteCommand request, CancellationToken cancellationToken)
        {
            var scontrato = _context.ServicioContrato.Single(sc => sc.ContratoId == request.ContratoId && sc.ServicioId == request.ServicioId);

            _context.ServicioContrato.RemoveRange(scontrato);

            try
            {
                await _context.SaveChangesAsync();

                return 201;
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return 500;
            }
        }
    }
}

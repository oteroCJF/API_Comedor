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
using Comedor.Domain.DContratos;

namespace Comedor.Service.EventHandler.Handlers.ServiciosContrato
{
    public class ServicioContratoCreateEventHandler : IRequestHandler<ServicioContratoCreateCommand, ServicioContrato>
    {
        private readonly ApplicationDbContext _context;

        public ServicioContratoCreateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServicioContrato> Handle(ServicioContratoCreateCommand request, CancellationToken cancellationToken)
        {
            var exists = _context.ServicioContrato.SingleOrDefault(sc => sc.ContratoId == request.ContratoId && sc.ServicioId == request.ServicioId);

            if (exists != null )
            {
                return null;
            }
            else
            {
                try
                {
                    var scontrato = new ServicioContrato
                    {
                        ContratoId = request.ContratoId,
                        ServicioId = request.ServicioId,
                        PrecioUnitario = request.PrecioUnitario,
                        IVA = request.IVA,
                        Total = request.IVA+request.PrecioUnitario,
                        PorcentajeImpuesto = request.PorcentajeImpuesto,
                    };

                    await _context.AddAsync(scontrato);
                    await _context.SaveChangesAsync();

                    return scontrato;
                }
                catch (Exception ex)
                {
                    string msg = ex.ToString();
                    return null;
                }
            }
        }
    }
}

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
    public class ServicioContratoUpdateEventHandler : IRequestHandler<ServicioContratoUpdateCommand, ServicioContrato>
    {
        private readonly ApplicationDbContext _context;

        public ServicioContratoUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServicioContrato> Handle(ServicioContratoUpdateCommand request, CancellationToken cancellationToken)
        {
            var scontrato = _context.ServicioContrato.SingleOrDefault(sc => sc.Id == request.Id);


            scontrato.ContratoId = request.ContratoId;
            scontrato.ServicioId = request.ServicioId;
            scontrato.PrecioUnitario = request.PrecioUnitario;
            scontrato.IVA = request.IVA;
            scontrato.Total = request.PrecioUnitario+request.IVA;
            scontrato.PorcentajeImpuesto = request.PorcentajeImpuesto;

            try
            {
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

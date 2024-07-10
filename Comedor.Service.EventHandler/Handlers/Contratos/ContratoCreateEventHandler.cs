using Comedor.Persistence.Database;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using Comedor.Service.EventHandler.Commands.Contratos;
using Comedor.Domain.DContratos;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Comedor.Service.EventHandler.Handlers.Contratos
{
    public class ContratoCreateEventHandler : IRequestHandler<ContratoCreateCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public ContratoCreateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(ContratoCreateCommand request, CancellationToken cancellationToken)
        {
            List<Contrato> contratos = await _context.Contratos.ToListAsync();

            if (request.Activo)
            {
                foreach (var cn in contratos)
                {
                    cn.Activo = false;
                    await _context.SaveChangesAsync();
                }
            }

            var contrato = new Contrato
            {
                UsuarioId = request.UsuarioId,
                NoContrato = request.NoContrato,
                Empresa = request.Empresa,
                Representante = request.Representante,
                RFC = request.RFC,
                Direccion = request.Direccion,
                MontoMin = request.MontoMin,
                MontoMax = request.MontoMax,
                VolumetriaMin = request.VolumetriaMin,
                VolumetriaMax = request.VolumetriaMax,
                InicioVigencia = request.InicioVigencia,
                FechaFirmaContrato = request.FechaFirmaContrato,
                FechaRecepcion = request.FechaRecepcion,
                FinVigencia = request.FinVigencia,
                Activo = request.Activo,
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now
            };

            try
            {
                await _context.AddAsync(contrato);
                await _context.SaveChangesAsync();
                return 201;
            }
            catch (Exception ex)
            {
                string msg = ex.Message + "\n" + ex.StackTrace + "\n" + ex.InnerException;
                if (ex.InnerException.Message.Contains("trigger"))
                {
                    return 202;
                }
                else
                {
                    return 500;
                }
            }
        }
    }
}

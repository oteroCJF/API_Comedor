using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.Contratos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using Comedor.Service.EventHandler.Commands.Convenios;
using Comedor.Domain.DContratos;

namespace Comedor.Service.EventHandler.Handlers.Convenios
{
    public class ConvenioCreateEventHandler : IRequestHandler<ConvenioCreateCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public ConvenioCreateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(ConvenioCreateCommand request, CancellationToken cancellationToken)
        {
            var convenio = new Convenio
            {
                UsuarioId = request.UsuarioId,
                ContratoId = request.ContratoId,
                NoConvenio = request.NoConvenio,
                MontoMin = request.MontoMin,
                MontoMax = request.MontoMax,
                VolumetriaMin = request.VolumetriaMin,
                VolumetriaMax = request.VolumetriaMax,
                InicioVigencia = request.InicioVigencia,
                FechaFirmaConvenio = request.FechaFirmaConvenio,
                FechaRecepcion = request.FechaRecepcion,
                Observaciones = request.Observaciones,
                FinVigencia = request.FinVigencia,
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now
            };

            try
            {
                await _context.AddAsync(convenio);
                await _context.SaveChangesAsync();

                var ConvenioId = convenio.Id;

                foreach (var rb in request.rubrosConvenio)
                {
                    var rubrosConvenio = new RubroConvenio
                    {
                        ConvenioId = ConvenioId,
                        RubroId = rb.RubroId,
                    };

                    await _context.AddAsync(rubrosConvenio);
                    await _context.SaveChangesAsync();
                }

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

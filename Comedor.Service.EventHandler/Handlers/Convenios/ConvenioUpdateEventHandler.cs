using Comedor.Persistence.Database;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using Comedor.Service.EventHandler.Commands.Convenios;
using Microsoft.EntityFrameworkCore;
using Comedor.Domain.DContratos;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Comedor.Service.EventHandler.Handlers.Convenios
{
    public class ConvenioUpdateEventHandler : IRequestHandler<ConvenioUpdateCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public ConvenioUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(ConvenioUpdateCommand request, CancellationToken cancellationToken)
        {
            if (await EliminaRubros(request.Id))
            {
                Convenio convenio = await _context.Convenio.SingleOrDefaultAsync(c => c.Id == request.Id);

                convenio.UsuarioId = request.UsuarioId;
                convenio.NoConvenio = request.NoConvenio;
                convenio.MontoMin = request.MontoMin;
                convenio.MontoMax = request.MontoMax;
                convenio.VolumetriaMin = request.VolumetriaMin;
                convenio.VolumetriaMax = request.VolumetriaMax;
                convenio.InicioVigencia = request.InicioVigencia;
                convenio.FinVigencia = request.FinVigencia;
                convenio.FechaFirmaConvenio = request.FechaFirmaConvenio;
                convenio.Observaciones = request.Observaciones;
                convenio.FechaRecepcion = request.FechaRecepcion;
                convenio.FechaActualizacion = DateTime.Now;

                try
                {
                    await _context.SaveChangesAsync();

                    foreach (var rb in request.Rubros)
                    {
                        var rubrosConvenio = new RubroConvenio
                        {
                            ConvenioId = request.Id,
                            RubroId = rb.RubroId,
                        };

                        await _context.AddAsync(rubrosConvenio);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    return 500;
                }
            }
            return 201;
        }

        public async Task<bool> EliminaRubros(int convenio)
        {
            List<RubroConvenio> rubros = await _context.RubroConvenio.Where(r => r.ConvenioId == convenio).ToListAsync();
            _context.RubroConvenio.RemoveRange(rubros);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message + "\n" + ex.StackTrace;
                return false;
            }
        }
    }
}

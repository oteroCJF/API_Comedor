using Comedor.Persistence.Database;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Comedor.Domain.DRepositorios;
using Comedor.Service.EventHandler.Commands.Repositorios;

namespace Comedor.Service.EventHandler.Handlers.HRepositorio
{
    public class RepositorioUpdateEventHandler : IRequestHandler<RepositorioUpdateCommand, Repositorio>
    {
        private readonly ApplicationDbContext _context;

        public RepositorioUpdateEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Repositorio> Handle(RepositorioUpdateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var Repositorio = await _context.Repositorios.SingleOrDefaultAsync(f => f.Id == request.Id);

                Repositorio.ContratoId = request.ContratoId;
                Repositorio.Anio = request.Anio;
                Repositorio.MesId = request.MesId;
                Repositorio.UsuarioId = request.UsuarioId;
                Repositorio.EstatusId = request.EstatusId;

                await _context.SaveChangesAsync();

                return Repositorio;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}

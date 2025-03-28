using Comedor.Persistence.Database;
using Comedor.Service.Queries.DTOs.Entregables;
using Comedor.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using Service.Common.Collection;
using Service.Common.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comedor.Service.Queries.Queries.Entregables
{
    public interface IEntregableQueryService
    {
        Task<List<EntregableDto>> GetAllEntregablesAsync();
        Task<List<EntregableDto>> GetEntregablesByCedula(int cedula);
        Task<EntregableDto> GetEntregableById(int entregable);
        Task<List<EntregableEstatusDto>> GetEntregablesByEstatus(int estatus);
        Task<DataCollection<EntregableDto>> GetEntregablesValidados();
    }

    public class EntregableQueryService : IEntregableQueryService
    {
        private readonly ApplicationDbContext _context;

        public EntregableQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EntregableDto>> GetAllEntregablesAsync()
        {
            try
            {
                var entregables = await _context.Entregables.Where(e => !e.FechaEliminacion.HasValue).ToListAsync();

                return entregables.MapTo<List<EntregableDto>>();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<List<EntregableDto>> GetEntregablesByCedula(int cedula)
        {
            try
            {
                var entregables = await _context.Entregables.Where(x => x.CedulaEvaluacionId == cedula && !x.FechaEliminacion.HasValue).OrderBy(e => e.EntregableId).ToListAsync();

                return entregables.MapTo<List<EntregableDto>>();
            }
            catch (Exception ex)
            { 
                string message = ex.Message;
                return null;
            }
        }

        public async Task<EntregableDto> GetEntregableById(int entregable)
        {
            var entregables = await _context.Entregables.SingleOrDefaultAsync(x => x.Id == entregable);

            return entregables.MapTo<EntregableDto>();
        }

        public async Task<List<EntregableEstatusDto>> GetEntregablesByEstatus(int estatus)
        {
            var entregables = await _context.EntregablesEstatus.Where(x => x.EstatusId == estatus).ToListAsync();

            return entregables.MapTo<List<EntregableEstatusDto>>();
        }
        
        public async Task<DataCollection<EntregableDto>> GetEntregablesValidados()
        {
            var entregables = await _context.Entregables
                //.Where(e => e.Validado.HasValue && !e.FechaEliminacion.HasValue)
                .Where(e => e.Validado.HasValue && !e.FechaEliminacion.HasValue && new[] { 6, 1, 7 }.Contains(e.EntregableId))
                .Select(e=> new EntregableDto { 
                    Id = e.Id,
                    CedulaEvaluacionId = e.CedulaEvaluacionId,
                    EntregableId = e.EntregableId,
                    EstatusId= e.EstatusId,
                    Validado = e.Validado
                })
                .GetPagedAsync(1,10000);

            return entregables.MapTo<DataCollection<EntregableDto>>();
        }
    }
}

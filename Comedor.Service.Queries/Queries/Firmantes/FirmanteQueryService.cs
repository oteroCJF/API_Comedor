using Comedor.Persistence.Database;
using Comedor.Service.Queries.DTOs.Firmantes;
using Comedor.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comedor.Service.Queries.Queries.Firmantes
{
    public interface IFirmantesQueryService
    {
        Task<List<FirmanteDto>> GetAllFirmantesAsync();
        Task<List<FirmanteDto>> GetFirmantesByInmueble(int inmueble);
        Task<FirmanteDto> GetFirmanteById(int firmante);
    }
    public class FirmanteQueryService : IFirmantesQueryService
    {
        private readonly ApplicationDbContext _context;

        public FirmanteQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FirmanteDto>> GetAllFirmantesAsync()
        {
            var firmantes = await _context.Firmantes.ToListAsync();

            return firmantes.MapTo<List<FirmanteDto>>();
        }

        public async Task<List<FirmanteDto>> GetFirmantesByInmueble(int inmueble)
        {
            var firmantes = await _context.Firmantes.Where(f => f.InmuebleId == inmueble).ToListAsync();

            return firmantes.MapTo<List<FirmanteDto>>();
        }

        public async Task<FirmanteDto> GetFirmanteById(int firmante)
        {
            var firmantes = await _context.Firmantes.SingleAsync(f => f.Id == firmante);

            return firmantes.MapTo<FirmanteDto>();
        }
    }
}

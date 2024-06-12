using Comedor.Service.Queries.DTOs.Oficios;
using Comedor.Persistence.Database;
using Comedor.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comedor.Service.Queries.Queries.Oficios
{
    public interface IOficioQueryService
    {
        public Task<List<OficioDto>> GetAllOficiosAsync();
        public Task<List<OficioDto>> GetOficiosByAnio(int anio);
        public Task<List<DetalleOficioDto>> GetDetalleOficio(int oficio);
        public Task<OficioDto> GetOficioById(int id);
    }

    public class OficioQueryService : IOficioQueryService
    {
        private readonly ApplicationDbContext _context;

        public OficioQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OficioDto>> GetAllOficiosAsync()
        {
            var oficios = await _context.Oficios.ToListAsync();

            return oficios.MapTo<List<OficioDto>>();
        }

        public async Task<List<OficioDto>> GetOficiosByAnio(int anio)
        {
            var oficios = await _context.Oficios.Where(o => o.Anio == anio).ToListAsync();

            return oficios.MapTo<List<OficioDto>>();
        }

        public async Task<List<DetalleOficioDto>> GetDetalleOficio(int oficio)
        {
            var oficios = await _context.DetalleOficios.Where(o => o.OficioId == oficio).ToListAsync();

            return oficios.MapTo<List<DetalleOficioDto>>();
        }

        public async Task<OficioDto> GetOficioById(int id)
        {
            try
            {
                var oficio = await _context.Oficios.SingleOrDefaultAsync(o => o.Id == id);

                return oficio.MapTo<OficioDto>();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}

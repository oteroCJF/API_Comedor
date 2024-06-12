using Comedor.Domain.DCuestionario;
using Comedor.Domain.DFacturas;
using Comedor.Persistence.Database;
using Comedor.Service.Queries.DTOs.CedulaEvaluacion;
using Comedor.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using Service.Common.Collection;
using Service.Common.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Comedor.Service.Queries.Queries.CedulasEvaluacion
{
    public interface ICedulaQueryService
    {
        Task<List<CedulaEvaluacionDto>> GetAllCedulasAsync();
        Task<DataCollection<CedulaEvaluacionDto>> GetCedulaEvaluacionByAnio(int anio);
        Task<CedulaEvaluacionDto> GetCedulaById(int cedula);
    }

    public class CedulaQueryService : ICedulaQueryService
    {
        private readonly ApplicationDbContext _context;

        public CedulaQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CedulaEvaluacionDto>> GetAllCedulasAsync()
        {
            try
            {
                var collection = await _context.CedulaEvaluacion.OrderByDescending(x => x.Id)
                                                            .OrderBy(c => c.MesId)
                                                            .ToListAsync();

                return collection.MapTo<List<CedulaEvaluacionDto>>();
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<DataCollection<CedulaEvaluacionDto>> GetCedulaEvaluacionByAnio(int anio)
        {
            int page = 1;
            var totalCedulas = await _context.CedulaEvaluacion.Where(x => x.Anio == anio && !x.FechaEliminacion.HasValue).CountAsync();

            var cedulas = await _context.CedulaEvaluacion
                .Where(x => x.Anio == anio && !x.FechaEliminacion.HasValue)
                .OrderBy(c => c.MesId)
                .GetPagedAsync(page, totalCedulas);

            return cedulas.MapTo<DataCollection<CedulaEvaluacionDto>>();
        }

        public async Task<CedulaEvaluacionDto> GetCedulaById(int cedula)
        {
            return (await _context.CedulaEvaluacion.SingleOrDefaultAsync(x => x.Id == cedula)).MapTo<CedulaEvaluacionDto>();
        }
    }
}

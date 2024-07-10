using Comedor.Persistence.Database;
using Comedor.Service.Queries.DTOs.EntregablesContratacion;
using Comedor.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comedor.Service.Queries.Queries.EntregablesContratacion
{
    public interface IEntregableContratacionQueryService
    {
        Task<List<EntregableContratacionDto>> GetEntregableContratacionByContrato(int contrato);
        Task<List<EntregableContratacionDto>> GetEntregableContratacionByContratoConvenio(int contrato, int convenio);
        Task<EntregableContratacionDto> GetEntregableById(int entregable);
    }

    public class EntregableContratacionQueryService : IEntregableContratacionQueryService
    {
        private readonly ApplicationDbContext _context;

        public EntregableContratacionQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EntregableContratacionDto>> GetEntregableContratacionByContrato(int contrato)
        {
            var EntregableContratacion = await _context.EntregableContratacion.Where(x => x.ContratoId == contrato &&
                                                                                    !x.FechaEliminacion.HasValue &&
                                                                                    !x.ConvenioId.HasValue).ToListAsync();

            return EntregableContratacion.MapTo<List<EntregableContratacionDto>>();
        }

        public async Task<List<EntregableContratacionDto>> GetEntregableContratacionByContratoConvenio(int contrato, int convenio)
        {
            var EntregableContratacion = await _context.EntregableContratacion.Where(x => x.ContratoId == contrato &&
                                                                                     x.ConvenioId == convenio &&
                                                                                    !x.FechaEliminacion.HasValue).ToListAsync();

            return EntregableContratacion.MapTo<List<EntregableContratacionDto>>();
        }

        public async Task<EntregableContratacionDto> GetEntregableById(int entregable)
        {
            var EntregableContratacion = await _context.EntregableContratacion.SingleOrDefaultAsync(x => x.Id == entregable);

            return EntregableContratacion.MapTo<EntregableContratacionDto>();
        }
    }
}

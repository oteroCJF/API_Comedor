using Comedor.Persistence.Database;
using Comedor.Service.Queries.DTOs.Contratos;
using Comedor.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comedor.Service.Queries.Queries.Convenios
{
    public interface IConvenioQueryService
    {
        Task<List<ConvenioDto>> GetConveniosByContratoAsync(int contrato);
        Task<ConvenioDto> GetConvenioByIdAsync(int convenio);
        Task<List<RubroConvenioDto>> GetRubrosConvenioAsync(int convenio);
    }

    public class ConvenioQueryService : IConvenioQueryService
    {
        private readonly ApplicationDbContext _context;

        public ConvenioQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ConvenioDto>> GetConveniosByContratoAsync(int contrato)
        {
            try
            {
                var collection = await _context.Convenio.Where(c => c.ContratoId == contrato).OrderBy(x => x.Id).ToListAsync();

                return collection.MapTo<List<ConvenioDto>>();
            }
            catch(Exception ex) 
            { 
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<ConvenioDto> GetConvenioByIdAsync(int convenio)
        {
            return (await _context.Convenio.SingleAsync(x => x.Id == convenio)).MapTo<ConvenioDto>();
        }

        public async Task<List<RubroConvenioDto>> GetRubrosConvenioAsync(int convenio)
        {
            var rubros = await _context.RubroConvenio.Where(c => c.ConvenioId == convenio).ToListAsync();
            return rubros.MapTo<List<RubroConvenioDto>>();
        }
    }
}

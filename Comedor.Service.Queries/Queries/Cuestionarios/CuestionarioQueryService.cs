using Comedor.Persistence.Database;
using Comedor.Service.Queries.DTOs.Cuestionario;
using Comedor.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Comedor.Service.Queries.Queries.Cuestionarios
{
    public interface ICuestionarioQueryService
    {
        Task<List<CuestionarioDto>> GetAllPreguntasAsync();
        Task<List<CuestionarioMensualDto>> GetCuestionarioMensualAsync(int anio, int mes, int contrato, int servicio);
        Task<CuestionarioDto> GetPreguntaByIdAsync(int pregunta);
        Task<List<int>> GetPreguntasDeductiva(int anio, int mes, int contrato, int servicio);

    }

    public class CuestionarioQueryService : ICuestionarioQueryService
    {
        private readonly ApplicationDbContext _context;

        public CuestionarioQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CuestionarioDto>> GetAllPreguntasAsync()
        {
            var preguntas = await _context.Cuestionarios.ToListAsync();

            return preguntas.MapTo<List<CuestionarioDto>>();
        }

        public async Task<List<CuestionarioMensualDto>> GetCuestionarioMensualAsync(int anio, int mes, int contrato, int servicio)
        {
            var preguntas = await _context.CuestionarioMensual.Where(x => x.Anio == anio && x.MesId == mes && x.ContratoId == contrato && x.ServicioId == servicio)
                .OrderBy(c => c.Consecutivo).ToListAsync();

            return preguntas.MapTo<List<CuestionarioMensualDto>>();
        }

        public async Task<CuestionarioDto> GetPreguntaByIdAsync(int pregunta)
        {
            return (await _context.Cuestionarios.SingleAsync(x => x.Id == pregunta)).MapTo<CuestionarioDto>();
        }

        public async Task<List<int>> GetPreguntasDeductiva(int anio, int mes, int contrato, int servicio)
        {
            var cuestionarioMensual = await _context.CuestionarioMensual.Where(x => x.Anio == anio && x.MesId == mes && x.ContratoId == contrato && x.ServicioId == servicio)
                                        .ToListAsync();

            List<int> preguntas = cuestionarioMensual.Where(cm => cm.Tipo.Equals("Deductiva")).Select(cm => cm.Consecutivo).ToList();

            return preguntas;
        }
    }
}

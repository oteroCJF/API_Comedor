using Comedor.Domain.DContratos;
using Comedor.Persistence.Database;
using Comedor.Service.Queries.DTOs.CedulaEvaluacion;
using Comedor.Service.Queries.DTOs.Cuestionario;
using Comedor.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comedor.Service.Queries.Queries.Respuestas
{
    public interface IRespuestaQueryService
    {
        Task<List<RespuestaDto>> GetAllRespuestasByAnioAsync(int anio);
        Task<List<RespuestaDto>> GetRespuestasByCedulaAsync(int cedula);
        Task<decimal> GetTotalPenasDeductivas(int cedula);
        Task<bool> VerificaDeductivas(int cedulaId);
        bool VerificaDeductivass(int anio, int mes, int contrato, int cedula);
    }

    public class RespuestaQueryService : IRespuestaQueryService
    {
        private readonly ApplicationDbContext _context;

        public RespuestaQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RespuestaDto>> GetAllRespuestasByAnioAsync(int anio)
        {
            try
            {
                var cedulas = await _context.CedulaEvaluacion.Where(c => c.Anio == anio).Select(c => c.Id).ToListAsync();
                var respuestas = await _context.Respuestas.Where(r => cedulas.Contains(r.CedulaEvaluacionId))
                                .ToListAsync();
                return respuestas.MapTo<List<RespuestaDto>>();

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<List<RespuestaDto>> GetRespuestasByCedulaAsync(int cedula)
        {
            try
            {
                var respuestas = await _context.Respuestas.Where(r => r.CedulaEvaluacionId == cedula).OrderBy(r => r.Pregunta).ToListAsync();
                return respuestas.MapTo<List<RespuestaDto>>();

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<decimal> GetTotalPenasDeductivas(int cedula)
        {
            try
            {
                var totalPD = await _context.Respuestas.Where(r => r.CedulaEvaluacionId == cedula).Select(r => r.MontoPenalizacion).SumAsync();
                var tot = await _context.Respuestas.Where(r => r.CedulaEvaluacionId == cedula).ToListAsync();

                return Convert.ToDecimal(totalPD);

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }

        public async Task<bool> VerificaDeductivas(int cedulaId)
        {
            var cedula = await _context.CedulaEvaluacion.SingleOrDefaultAsync(c => c.Id == cedulaId);

            var cuestionarioMensual = await _context.CuestionarioMensual.Where(x => x.Anio == cedula.Anio && 
                                                                                    x.MesId == cedula.MesId && 
                                                                                    x.ContratoId == cedula.ContratoId && 
                                                                                    x.Tipo.Equals("Deductiva"))
                .Select(c => c.Consecutivo).ToListAsync();

            var deductivas = await _context.Respuestas.Where(r => cuestionarioMensual.Contains(r.Pregunta) && r.CedulaEvaluacionId == cedula.Id).ToListAsync();

            return deductivas.Sum(d => d.MontoPenalizacion) > 0 ? true : false;
        }
        
        public bool VerificaDeductivass(int anio, int mes, int contrato, int cedula)
        {
            var cuestionarioMensual = _context.CuestionarioMensual.Where(x => x.Anio == anio && 
                                                                                    x.MesId == mes && 
                                                                                    x.ContratoId == contrato && 
                                                                                    x.Tipo.Equals("Deductiva"))
                .Select(c => c.Consecutivo).ToList();

            var deductivas = _context.Respuestas.Where(r => cuestionarioMensual.Contains(r.Pregunta) && r.CedulaEvaluacionId == cedula).ToList();

            return deductivas.Sum(d => d.MontoPenalizacion) > 0 ? true : false;
        }

    }
}

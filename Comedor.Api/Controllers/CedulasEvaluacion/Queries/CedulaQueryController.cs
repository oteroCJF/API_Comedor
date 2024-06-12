using Comedor.Service.Queries.DTOs.CedulaEvaluacion;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Comedor.Service.Queries.Queries.CedulasEvaluacion;
using System.Linq;
using Comedor.Service.Queries.Queries.Respuestas;
using Service.Common.Collection;

namespace Comedor.Api.Controllers.CedulaEvaluacion.Queries
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/cedulaEvaluacion")]
    public class CedulaQueryController : ControllerBase
    {
        private readonly ICedulaQueryService _cedula;
        private readonly IRespuestaQueryService _respuestas;        

        public CedulaQueryController(ICedulaQueryService cedula, IRespuestaQueryService respuestas)
        {
            _cedula = cedula;
            _respuestas = respuestas;
        }

        [HttpGet]
        public async Task<List<CedulaEvaluacionDto>> GetAllCedulasEvaluacion()
        {
            return await _cedula.GetAllCedulasAsync();
        }

        [Route("getCedulasByAnio/{anio}")]
        [HttpGet]
        public async Task<DataCollection<CedulaEvaluacionDto>> GetCedulasEvaluacionByAnio(int anio)
        {
            var result = await _cedula.GetCedulaEvaluacionByAnio(anio);

            var cedulas = result.Items.Select(c => new CedulaEvaluacionDto
            {
                Id = c.Id,
                ContratoId = c.ContratoId,
                Anio = c.Anio,
                MesId = c.MesId,
                InmuebleId = c.InmuebleId,
                ServicioId = c.ServicioId,
                Folio = c.Folio,
                EstatusId = c.EstatusId,
                Calificacion = c.Calificacion,
                FechaInicial = c.FechaInicial,
                FechaFinal = c.FechaFinal,
                FechaCreacion = c.FechaCreacion,
                FechaActualizacion = c.FechaActualizacion,
                FechaEliminacion = c.FechaEliminacion,
            }).OrderBy(c => c.Id);

            result.Items = cedulas;

            return result;
        }

        [Route("getCedulaById/{id}")]
        [HttpGet]
        public async Task<CedulaEvaluacionDto> GetCedulaById(int id)
        {
            var cedula = await _cedula.GetCedulaById(id);

            return cedula != null ? cedula : new CedulaEvaluacionDto();
        }

        [Route("getTotalPD/{cedula}")]
        [HttpGet]
        public async Task<decimal> GetTotalPDAsync(int cedula)
        {
            return await _respuestas.GetTotalPenasDeductivas(cedula);
        }
    }
}

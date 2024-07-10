using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Comedor.Service.Queries.DTOs.Cuestionario;
using Comedor.Service.Queries.Queries.Cuestionarios;

namespace Comedor.Api.Controllers.Cuestionarios.Queries
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/cuestionarios")]

    public class CuestionarioQueryController : ControllerBase
    {
        private readonly ICuestionarioQueryService _cuestionario;
        //private readonly IMediator mediator;

        public CuestionarioQueryController(ICuestionarioQueryService cuestionario)
        {
            _cuestionario = cuestionario;
        }

        [HttpGet]
        public async Task<List<CuestionarioDto>> GetAllPreguntas()
        {
            return await _cuestionario.GetAllPreguntasAsync();
        }

        [Route("{anio}/{mes}/{contrato}/{servicio}")]
        [HttpGet]
        public async Task<List<CuestionarioMensualDto>> GetCuestionarioMensualId(int anio, int mes, int contrato, int servicio)
        {
            return await _cuestionario.GetCuestionarioMensualAsync(anio, mes, contrato, servicio);
        }

        [Route("getPreguntaById/{pregunta}")]
        [HttpGet]
        public async Task<CuestionarioDto> GetPreguntaById(int pregunta)
        {
            return await _cuestionario.GetPreguntaByIdAsync(pregunta);
        }
        
        [Route("getPreguntasDeductiva/{anio}/{mes}/{contrato}/{servicio}")]
        [HttpGet]
        public async Task<List<int>> GetPreguntasDeductiva(int anio, int mes, int contrato, int servicio)
        {
            return await _cuestionario.GetPreguntasDeductiva(anio, mes, contrato, servicio);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Comedor.Service.Queries.DTOs.CedulaEvaluacion;
using Comedor.Service.EventHandler.Commands.Respuestas;
using MediatR;
using System;
using Comedor.Service.Queries.Queries.Respuestas;

namespace Comedor.Api.Controllers.Respuestas.Queries
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/respuestasEvaluacion")]
    public class RespuestasController : ControllerBase
    {
        private readonly IRespuestaQueryService _respuestas;
        private readonly IMediator _mediator;

        public RespuestasController(IRespuestaQueryService respuestas, IMediator mediator)
        {
            _respuestas = respuestas;
            _mediator = mediator;
        }

        [Route("{cedula}")]
        [HttpGet]
        public async Task<List<RespuestaDto>> GetCedulaEvaluacionByCedulaAnioMes(int cedula)
        {
            var respuestas = await _respuestas.GetRespuestasByCedulaAsync(cedula);

            return respuestas;
        }

        [Route("getRespuestasByAnio/{anio}")]
        [HttpGet]
        public async Task<List<RespuestaDto>> GetAllRespuestasByAnioAsync(int anio)
        {
            var respuestas = await _respuestas.GetAllRespuestasByAnioAsync(anio);

            return respuestas;
        }
        
        [Route("verificaDeductivas/{cedulaId}")]
        [HttpGet]
        public async Task<bool> VerificaDeductivas(int cedulaId)
        {
            var respuestas = await _respuestas.VerificaDeductivas(cedulaId);

            return respuestas;
        }
    }
}

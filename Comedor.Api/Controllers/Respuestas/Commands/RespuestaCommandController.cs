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

namespace Comedor.Api.Controllers.Respuestas.Commands
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/respuestasEvaluacion")]
    public class RespuestaCommandController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RespuestaCommandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("updateRespuestasByCedula")]
        [HttpPut]
        public async Task<IActionResult> UpdateRespuestasByCedula([FromBody] List<RespuestasUpdateCommand> respuestas)
        {
            try
            {
                foreach (var rs in respuestas)
                {
                    await _mediator.Send(rs);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return BadRequest();
            }

        }
    }
}

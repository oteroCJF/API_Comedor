using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Comedor.Service.EventHandler.Commands.LogCedulas;
using Comedor.Service.EventHandler.Commands.CedulasEvaluacion.ActualizacionCedula;
using Comedor.Service.EventHandler.Commands.CedulasEvaluacion;

namespace Comedor.Api.Controllers.CedulasEvaluacion.Commands
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/cedulaEvaluacion")]
    public class CedulaCommandController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CedulaCommandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("enviarCedula")]
        [HttpPut]
        public async Task<IActionResult> UpdateCedula([FromBody] EnviarCedulaEvaluacionUpdateCommand request)
        {
            var cedula = await _mediator.Send(request);

            if (cedula != null)
            {
                var log = new LogCedulasCreateCommand
                {
                    UsuarioId = request.UsuarioId,
                    CedulaEvaluacionId = cedula.Id,
                    EstatusId = request.EstatusId,
                    Observaciones = request.Observaciones
                };

                var logs = await _mediator.Send(log);

                if (logs != null)
                {
                    return Ok(cedula);
                }
            }
            return Ok(cedula);
        }

        [Route("updateCedula")]
        [HttpPut]
        public async Task<IActionResult> CedulaSolicitudRechazo([FromBody] CedulaEvaluacionUpdateCommand request)
        {
            var cedula = await _mediator.Send(request);

            if (cedula != null)
            {
                var log = new LogCedulasCreateCommand
                {
                    UsuarioId = request.UsuarioId,
                    CedulaEvaluacionId = cedula.Id,
                    EstatusId = cedula.EstatusId,
                    Observaciones = request.Observaciones
                };

                var logs = await _mediator.Send(log);

                if (logs != null)
                {
                    return Ok(cedula);
                }
            }
            return Ok(cedula);
        }

    }
}

using Comedor.Service.EventHandler.Commands.ServiciosContrato;
using Comedor.Service.Queries.DTOs.Contratos;
using Comedor.Service.Queries.Queries.ServiciosContrato;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comedor.Api.Controllers.ServiciosContrato.Commands
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/servicioContrato")]
    public class SContratoCommandController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SContratoCommandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("createSContrato")]
        [HttpPost]
        public async Task<IActionResult> CreateContrato([FromBody] ServicioContratoCreateCommand request)
        {
            var contrato = await _mediator.Send(request);
            return Ok(contrato);
        }

        [Route("updateSContrato")]
        [HttpPut]
        public async Task<IActionResult> UpdateContrato([FromBody] ServicioContratoUpdateCommand request)
        {
            var contrato = await _mediator.Send(request);
            return Ok(contrato);
        }

        [Route("deleteSContrato")]
        [HttpPut]
        public async Task<IActionResult> DeleteContrato([FromBody] ServicioContratoDeleteCommand request)
        {
            var contrato = await _mediator.Send(request);
            return Ok(contrato);
        }
    }
}

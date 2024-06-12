using Comedor.Service.EventHandler.Commands.Contratos;
using Comedor.Service.Queries.DTOs.Contratos;
using Comedor.Service.Queries.Queries.Contratos;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comedor.Api.Controllers.Contratos.Commands
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/contratos")]
    public class ContratoCommandController : ControllerBase
    {
        private readonly IContratosQueryService _contratos;
        private readonly IMediator _mediator;

        public ContratoCommandController(IContratosQueryService contratos, IMediator mediator)
        {
            _contratos = contratos;
            _mediator = mediator;
        }

        [Route("createContrato")]
        [HttpPost]
        public async Task<IActionResult> CreateContrato([FromBody] ContratoCreateCommand contrato)
        {
            int success = await _mediator.Send(contrato);
            return Ok(success);
        }

        [Route("updateContrato")]
        [HttpPut]
        public async Task<IActionResult> UpdateContrato([FromBody] ContratoUpdateCommand contrato)
        {
            int success = await _mediator.Send(contrato);
            return Ok(success);
        }

        [Route("deleteContrato")]
        [HttpPut]
        public async Task<IActionResult> DeleteContrato([FromBody] ContratoDeleteCommand contrato)
        {
            int success = await _mediator.Send(contrato);
            return Ok(success);
        }
    }
}

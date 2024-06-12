using Comedor.Service.EventHandler.Commands.Contratos;
using Comedor.Service.EventHandler.Commands.Convenios;
using Comedor.Service.Queries.DTOs.Contratos;
using Comedor.Service.Queries.Queries.Convenios;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comedor.Api.Controllers.Convenios.Commands
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/convenios")]
    public class ConvenioCommandController : ControllerBase
    {
        private readonly IConvenioQueryService _convenio;
        private readonly IMediator _mediator;

        public ConvenioCommandController(IConvenioQueryService convenio, IMediator mediator)
        {
            _convenio = convenio;
            _mediator = mediator;
        }

        [Route("createConvenio")]
        [HttpPost]
        public async Task<IActionResult> CreateConvenio([FromBody] ConvenioCreateCommand contrato)
        {
            int success = await _mediator.Send(contrato);
            return Ok(success);
        }

        [Route("updateConvenio")]
        [HttpPut]
        public async Task<IActionResult> UpdateConvenio([FromBody] ConvenioUpdateCommand contrato)
        {
            int success = await _mediator.Send(contrato);
            return Ok(success);
        }

        [Route("deleteConvenio")]
        [HttpPut]
        public async Task<IActionResult> DeleteConvenio([FromBody] ConvenioDeleteCommand contrato)
        {
            int success = await _mediator.Send(contrato);
            return Ok(success);
        }

    }
}

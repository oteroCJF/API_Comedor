using Comedor.Service.EventHandler.Commands.LogEntregables;
using Comedor.Service.Queries.DTOs.LogEntregables;
using Comedor.Service.Queries.Queries.LogEntregables;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comedor.Api.Controllers.LogEntregables.Commands
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/logEntregables")]
    public class LEntregableCommandController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LEntregableCommandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("createHistorial")]
        public async Task<IActionResult> CreateHistorial([FromBody] LogEntregablesCreateCommand historial)
        {
            await _mediator.Send(historial);
            return Ok();
        }
    }
}

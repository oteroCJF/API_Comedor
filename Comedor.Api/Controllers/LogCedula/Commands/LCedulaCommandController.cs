using Comedor.Service.EventHandler.Commands.LogCedulas;
using Comedor.Service.Queries.DTOs.LogCedulas;
using Comedor.Service.Queries.Queries.LogCedulas;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comedor.Api.Controllers.LogCedula.Commands
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/logCedulas")]
    public class LCedulaCommandController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LCedulaCommandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("createHistorial")]
        public async Task<IActionResult> CreateHistorial([FromBody] LogCedulasCreateCommand historial)
        {
            await _mediator.Publish(historial);
            return Ok();
        }
    }
}

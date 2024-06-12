using Comedor.Service.EventHandler.Commands.EntregableContratacion;
using Comedor.Service.Queries.DTOs.EntregablesContratacion;
using Comedor.Service.Queries.Queries.EntregablesContratacion;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Comedor.Api.Controllers.EntregablesContratacion.Commands
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/entregablesContratacion")]
    public class EntregableCCommandController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EntregableCCommandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Consumes("multipart/form-data")]
        [Route("updateEntregableContratacion")]
        [HttpPut]
        public async Task<IActionResult> UpdateEntregable([FromForm] EntregableContratacionUpdateCommand entregable)
        {
            int status = await _mediator.Send(entregable);
            return Ok(status);
        }

    }
}

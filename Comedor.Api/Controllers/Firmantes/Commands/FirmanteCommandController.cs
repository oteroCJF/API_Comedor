using Comedor.Service.EventHandler.Commands.Firmantes;
using Comedor.Service.Queries.DTOs.Firmantes;
using Comedor.Service.Queries.Queries.Firmantes;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comedor.Api.Controllers.Firmantes.Commands
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/firmantes")]
    public class FirmanteCommandController : ControllerBase
    {
        private readonly IFirmantesQueryService _firmantes;
        private readonly IMediator _mediator;

        public FirmanteCommandController(IFirmantesQueryService firmantes, IMediator mediator)
        {
            _firmantes = firmantes;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("createFirmantes")]
        public async Task<IActionResult> CreateFirmantes([FromBody] FirmantesCreateCommand firmantes)
        {
            var firmante = await _mediator.Send(firmantes);
            return Ok(firmante);
        }

        [HttpPut]
        [Route("updateFirmantes")]
        public async Task<IActionResult> UpdateFirmantes([FromBody] FirmantesUpdateCommand firmantes)
        {
            var firmante = await _mediator.Send(firmantes);
            return Ok(firmante);
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Comedor.Service.EventHandler.Commands.Repositorios;

namespace Comedor.Api.Controllers.Repositorios.Commands
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/repositorios")]
    public class RepositorioCommandController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RepositorioCommandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("createRepositorio")]
        [HttpPost]
        public async Task<IActionResult> CreateRepositorio([FromBody] RepositorioCreateCommand request)
        {
            var status = await _mediator.Send(request);
            return Ok(status);
        }

        [Route("updateRepositorio")]
        [HttpPut]
        public async Task<IActionResult> UpdateFacturacion([FromBody] RepositorioUpdateCommand request)
        {
            var status = await _mediator.Send(request);
            return Ok(status);
        }
    }
}

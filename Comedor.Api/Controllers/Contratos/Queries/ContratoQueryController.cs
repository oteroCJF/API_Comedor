using Comedor.Service.EventHandler.Commands.Contratos;
using Comedor.Service.Queries.DTOs.Contratos;
using Comedor.Service.Queries.Queries.Contratos;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comedor.Api.Controllers.Contratos.Queries
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/contratos")]
    public class ContratoQueryController : ControllerBase
    {
        private readonly IContratosQueryService _contratos;
        private readonly IMediator _mediator;

        public ContratoQueryController(IContratosQueryService contratos, IMediator mediator)
        {
            _contratos = contratos;
            _mediator = mediator;
        }

        [Route("getContratos")]
        [HttpGet]
        public async Task<List<ContratoDto>> GetAllContratos()
        {
            return await _contratos.GetAllContratosAsync();
        }

        [Route("getContratoById/{id}")]
        [HttpGet]
        public async Task<ContratoDto> GetContratoById(int id)
        {
            return await _contratos.GetContratoByIdAsync(id);
        }

    }
}

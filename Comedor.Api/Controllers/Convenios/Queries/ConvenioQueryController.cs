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

namespace Comedor.Api.Controllers.Convenios.Queries
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/convenios")]
    public class ConvenioQueryController : ControllerBase
    {
        private readonly IConvenioQueryService _convenio;
        private readonly IMediator _mediator;

        public ConvenioQueryController(IConvenioQueryService convenio, IMediator mediator)
        {
            _convenio = convenio;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("getConveniosByContrato/{contrato}")]
        public async Task<List<ConvenioDto>> getConveniosByContrato(int contrato)
        {
            List<ConvenioDto> convenios = await _convenio.GetConveniosByContratoAsync(contrato);
            return convenios;
        }

        [HttpGet]
        [Route("getConvenioById/{convenio}")]
        public async Task<ConvenioDto> getConveniosById(int convenio)
        {
            ConvenioDto convenios = await _convenio.GetConvenioByIdAsync(convenio);
            return convenios;
        }

        [HttpGet]
        [Route("getRubrosByConvenio/{convenio}")]
        public async Task<List<RubroConvenioDto>> GetRubrosConvenio(int convenio)
        {
            List<RubroConvenioDto> convenios = await _convenio.GetRubrosConvenioAsync(convenio);
            return convenios;
        }
    }
}

using Comedor.Service.EventHandler.Commands.Firmantes;
using Comedor.Service.Queries.DTOs.Firmantes;
using Comedor.Service.Queries.Queries.Firmantes;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comedor.Api.Controllers.Firmantes.Queries
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/firmantes")]
    public class FirmanteQueryController : ControllerBase
    {
        private readonly IFirmantesQueryService _firmantes;
        private readonly IMediator _mediator;

        public FirmanteQueryController(IFirmantesQueryService firmantes, IMediator mediator)
        {
            _firmantes = firmantes;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<List<FirmanteDto>> GetAllFirmantesAsync()
        {
            var firmantes = await _firmantes.GetAllFirmantesAsync();
            return firmantes;
        }

        [HttpGet]
        [Route("getFirmantesByInmueble/{inmueble}")]
        public async Task<List<FirmanteDto>> GetFirmantesByInmueble(int inmueble)
        {
            var firmantes = await _firmantes.GetFirmantesByInmueble(inmueble);
            return firmantes;
        }

        [HttpGet]
        [Route("getFirmanteById/{firmante}")]
        public async Task<FirmanteDto> GetFirmanteById(int firmante)
        {
            var firmantes = await _firmantes.GetFirmanteById(firmante);
            return firmantes;
        }
    }
}

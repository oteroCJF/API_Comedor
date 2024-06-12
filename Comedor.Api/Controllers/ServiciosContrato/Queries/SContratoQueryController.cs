using Comedor.Service.EventHandler.Commands.ServiciosContrato;
using Comedor.Service.Queries.DTOs.Contratos;
using Comedor.Service.Queries.Queries.ServiciosContrato;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comedor.Api.Controllers.ServicioContrato.Queries
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/servicioContrato")]
    public class SContratoQueryController : ControllerBase
    {
        private readonly IServicioContratoQueryService _scontrato;

        public SContratoQueryController(IServicioContratoQueryService scontrato)
        {
            _scontrato = scontrato;
        }

        [Route("getServiciosContrato/{contrato}")]
        [HttpGet]
        public async Task<List<ServicioContratoDto>> GetServiciosByContrato(int contrato)
        {
            return await _scontrato.GetServicioContratoListAsync(contrato);
        }
    }
}

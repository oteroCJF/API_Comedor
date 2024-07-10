using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Comedor.Service.Queries.Queries.Repositorios;
using Comedor.Service.Queries.DTOs.Repositorio;

namespace Comedor.Api.Controllers.Repositorios.Queries
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/repositorios")]
    public class RepositorioCommandController : ControllerBase
    {
        private readonly IRepositorioQueryService _repositorios;

        public RepositorioCommandController(IRepositorioQueryService facturacion)
        {
            _repositorios = facturacion;
        }

        [HttpGet("{anio}")]
        public async Task<List<RepositorioDto>> GetFacturaciones(int anio)
        {
            return await _repositorios.GetAllRepositoriosAsync(anio);
        }

        [HttpGet("getRepositorioById/{repositorio}")]
        public async Task<RepositorioDto> GetFacturacionById(int repositorio)
        {
            return await _repositorios.GetRepositorioByIdAsync(repositorio);
        }
        
        [HttpGet("getRepositorioByAMC/{anio}/{mes}/{contrato}")]
        public async Task<RepositorioDto> GetFacturacionByCAM(int contrato, int anio, int mes)
        {
            return await _repositorios.GetRepositorioByCAM(contrato, anio, mes);
        }
    }
}

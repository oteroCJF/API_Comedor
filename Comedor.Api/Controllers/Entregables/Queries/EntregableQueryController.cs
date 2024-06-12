using Comedor.Service.EventHandler.Commands.Entregables;
using Comedor.Service.Queries.DTOs.Entregables;
using Comedor.Service.Queries.Queries.Entregables;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Comedor.Service.EventHandler.Commands.LogEntregables;
using System;
using Comedor.Service.EventHandler.Commands.Entregables;
using Service.Common.Collection;

namespace Comedor.Api.Controllers.Entregables.Queries
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/entregables")]
    public class EntregableQueryController : ControllerBase
    {
        private readonly IEntregableQueryService _entregables;
        private readonly IHostingEnvironment _environment;
        private readonly IMediator _mediator;

        public EntregableQueryController(IEntregableQueryService entregables, IMediator mediator, IHostingEnvironment environment)
        {
            _entregables = entregables;
            _mediator = mediator;
            _environment = environment;
        }

        [HttpGet]
        public async Task<List<EntregableDto>> GetAllEntregables()
        {
            var configuracion = await _entregables.GetAllEntregablesAsync();

            return configuracion;
        }

        
        [Route("getEntregablesByCedula/{cedula}")]
        [HttpGet]
        public async Task<List<EntregableDto>> GetEntregablesByCedula(int cedula)
        {
            var entregables = await _entregables.GetEntregablesByCedula(cedula);

            return entregables;
        }

        
        [Route("getEntregableById/{entregable}")]
        [HttpGet]
        public async Task<EntregableDto> GetEntregableById(int entregable)
        {
            var entregables = await _entregables.GetEntregableById(entregable);

            return entregables;
        }

        
        [Route("getEntregablesValidados")]
        [HttpGet]
        public async Task<DataCollection<EntregableDto>> GetEntregablesValidados()
        {
            var entregables = await _entregables.GetEntregablesValidados();

            return entregables;
        }
        
        [Route("getEntregablesByEstatus/{estatus}")]
        [HttpGet]
        public async Task<List<EntregableEstatusDto>> GetEntregablesByEstatus(int estatus)
        {
            var entregables = await _entregables.GetEntregablesByEstatus(estatus);

            return entregables;
        }


        [Route("visualizarEntregable/{anio}/{mes}/{folio}/{archivo}/{tipo}")]
        [HttpGet]
        public async Task<string> VisualizarEntregable(int anio, string mes, string folio, string archivo, string tipo)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Entregables\\" + anio + "" + "\\" + mes + "\\" + folio + "\\" + tipo;
            string webRootPath = _environment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string pathArchivo = Path.Combine(newPath, archivo);

            if (System.IO.File.Exists(pathArchivo))
            {
                return pathArchivo;
            }

            return "";
        }

        
        [Route("getPathEntregables")]
        [HttpGet]
        public async Task<string> GetPathEntregables()
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Entregables";

            return folderName;
        }
    }
}

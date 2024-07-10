using MediatR;
using Comedor.Domain.DEntregables;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.EventHandler.Commands.Entregables.Update
{
    public class EntregableUpdateCommand : IRequest<Entregable>
    {
        public int Id { get; set; }
        public string? UsuarioId { get; set; }
        public IFormFile Archivo { get; set; }
        public int EstatusId { get; set; }
        public bool Validado { get; set; }
        public bool Validar { get; set; }
        public string Observaciones { get; set; }

        //Parametros para las carpetas
        public int Anio { get;set; }
        public string Mes { get;set; }
        public string Folio { get;set; }
        public string TipoEntregable { get;set; }
    }
}

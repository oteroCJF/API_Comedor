using MediatR;
using Comedor.Domain.DOficios;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.EventHandler.Commands.Oficios
{
    public class OficioCreateCommand : IRequest<Oficio>
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public int ContratoId { get; set; }
        public int ServicioId { get; set; }
        public int Anio { get; set; }
        public string NumeroOficio { get; set; }
        public DateTime FechaTramitado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public IFormFile Oficio { get; set; }
    }
}

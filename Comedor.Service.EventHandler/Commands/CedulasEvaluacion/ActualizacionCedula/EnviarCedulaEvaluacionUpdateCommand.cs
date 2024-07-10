using MediatR;
using Comedor.Domain.DCedulaEvaluacion;
using System;
using System.Collections.Generic;

namespace Comedor.Service.EventHandler.Commands.CedulasEvaluacion
{
    public class EnviarCedulaEvaluacionUpdateCommand : IRequest<CedulaEvaluacion>
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public int EstatusId { get; set; }
        public int RepositorioId { get; set; }
        public int EFacturaId { get; set; }
        public int ENotaCreditoId { get; set; }
        public bool Calcula { get; set; }
        public string Estatus { get; set; }
        public decimal UMA { get; set; }
        public string Observaciones { get; set; }

        public virtual List<ServicioContratoDto> Penalizacion { get; set; }
        public virtual List<CTIndemnizacionDto> Indemnizaciones { get; set; } 
    }
}

using MediatR;
using Comedor.Domain.DCedulaEvaluacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.EventHandler.Commands.Respuestas
{
    public class RespuestasUpdateCommand : IRequest<RespuestaEvaluacion>
    {
        public int CedulaEvaluacionId { get; set; }
        public int Pregunta { get; set; }
        public bool Respuesta { get; set; }
        public string Detalles{ get; set; }
        public bool Penalizable{ get; set; } 
        public decimal MontoPenalizacion { get; set; } 
        public DateTime FechaActualizacion{ get; set; }

        public string UsuarioId { get; set; }
    }
}

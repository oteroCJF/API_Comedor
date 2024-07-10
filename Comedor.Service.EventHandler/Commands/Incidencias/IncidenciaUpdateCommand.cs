using MediatR;
using Comedor.Domain.DIncidencias;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Comedor.Service.EventHandler.Commands.Incidencias
{
    public class IncidenciaUpdateCommand : IRequest<Incidencia>
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public int CedulaEvaluacionId { get; set; }
        public int IncidenciaId { get; set; }
        public int TipoId { get; set; }
        public int Pregunta { get; set; }
        public DateTime FechaIncidencia { get; set; }
        public DateTime FechaProgramada { get; set; }
        public DateTime UltimoDia { get; set; }
        public DateTime FechaRealizada { get; set; }
        public DateTime FechaInventario { get; set; }
        public DateTime FechaNotificacion { get; set; }
        public DateTime FechaAcordadaAdmin { get; set; }
        public DateTime FechaEntrega { get; set; }
        public DateTime FechaLimite { get; set; }
        public bool EntregaEnseres { get; set; }
        public string HoraInicio { get; set; }
        public string HoraReal { get; set; }
        public int Ponderacion { get; set; }
        public int Cantidad { get; set; }
        public string Observaciones { get; set; }
        public bool Penalizable { get; set; }
        public decimal MontoPenalizacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public List<int> DTIncidencia { get; set; } = new List<int>();
    }
}

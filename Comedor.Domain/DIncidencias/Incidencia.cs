﻿using System;

namespace Comedor.Domain.DIncidencias
{
    public class Incidencia
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
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraReal { get; set; }
        public bool EntregaEnseres { get; set; }
        public int Ponderacion { get; set; }
        public int Cantidad { get; set; }
        public string Observaciones { get; set; }
        public bool Penalizable { get; set; }
        public decimal MontoPenalizacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }

    }
}

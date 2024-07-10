using System;

namespace Comedor.Service.Queries.DTOs.Incidencias
{
    public class ConfiguracionIncidenciaDto
    {
        public int Id { get; set; }
        public int Pregunta { get; set; }
        public bool Respuesta { get; set; }
        public bool Obligatorio{ get; set; }
        public bool MateriaPrima { get; set; }
        public bool Abasto { get; set; }
        public bool FechaIncidencia { get; set; }
        public bool FechaProgramada { get; set; }
        public bool UltimoDia { get; set; }
        public bool FechaRealizada { get; set; }
        public bool FechaInventario { get; set; }
        public bool FechaNotificacion { get; set; }
        public bool FechaAcordadaAdmin { get; set; }
        public bool FechaEntrega { get; set; }
        public bool FechaLimite { get; set; }
        public bool HoraInicio { get; set; }
        public bool HoraReal { get; set; }
        public bool EntregaEnseres { get; set; }
        public bool Ponderacion { get; set; }
        public bool Cantidad { get; set; }
        public bool Observaciones { get; set; }
        public string? RespuestaCedula { get; set; }
        public string? Ayuda { get; set; }
    }
}

using System;

namespace Comedor.Service.Queries.DTOs.CedulaEvaluacion
{
    public class CedulaEvaluacionDto
    {
        public int Id { get; set; }
        public int ContratoId { get; set; }
        public int InmuebleId { get; set; }
        public int MesId { get; set; }
        public int EstatusId { get; set; }
        public int ServicioId { get; set; }
        public bool Bloqueada { get; set; }
        public int Anio { get; set; }
        public string UsuarioId { get; set; }
        public string Folio { get; set; }
        public double Calificacion { get; set; }
        public decimal Penalizacion { get; set; }
        public bool RequiereNC { get; set; }
        public Nullable<DateTime> FechaInicial { get; set; }
        public Nullable<DateTime> FechaFinal { get; set; }
        public Nullable<decimal> Factura { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
    }
}

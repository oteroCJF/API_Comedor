using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comedor.Domain.DCedulaEvaluacion
{
    public class CedulaEvaluacion
    {
        public int Id { get; set; }
        public int ContratoId { get; set; }
        public int InmuebleId { get; set; }
        public string UsuarioId { get; set; }
        public int MesId { get; set; }
        public int EstatusId { get; set; }
        public int ServicioId { get; set; }
        public int Anio { get; set; }
        public bool Bloqueada { get; set; }
        public string Folio { get; set; }
        [Column(TypeName = "decimal(5, 1)")]
        public double Calificacion { get; set; }
        public decimal Penalizacion { get; set; }
        public Nullable<DateTime> FechaInicial { get; set; }
        public Nullable<DateTime> FechaFinal { get; set; }
        public Nullable<DateTime> FechaCreacion { get; set; }
        public Nullable<DateTime> FechaActualizacion { get; set; }
        public Nullable<DateTime> FechaEliminacion { get; set; }
    }
}

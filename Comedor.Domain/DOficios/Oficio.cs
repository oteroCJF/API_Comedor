using System;

namespace Comedor.Domain.DOficios
{
    public class Oficio
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public int ContratoId { get; set; }
        public int EstatusId { get; set; }
        public Nullable<int> Anio { get; set; }
        public string NumeroOficio { get; set; }
        public Nullable<DateTime> FechaTramitado { get; set; }
        public Nullable<DateTime> FechaPagado { get; set; }
        public Nullable<DateTime> FechaCreacion { get; set; }
        public Nullable<DateTime> FechaActualizacion { get; set; }
        public Nullable<DateTime> FechaEliminacion { get; set; }
    }
}

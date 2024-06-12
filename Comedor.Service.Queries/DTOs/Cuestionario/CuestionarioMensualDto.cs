using System;

namespace Comedor.Service.Queries.DTOs.Cuestionario
{
    public class CuestionarioMensualDto
    {
        public int CuestionarioId { get; set; }
        public int Consecutivo { get; set; }
        public int ServicioId { get; set; }
        public int CategoriaId { get; set; }
        public int Anio { get; set; }
        public int MesId { get; set; }
        public System.Nullable<int> Ponderacion { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string? Formula { get; set; } = string.Empty;
        public Nullable<decimal> Porcentaje { get; set; }
        public System.Nullable<bool> ACLRS { get; set; }
    }
}

namespace Comedor.Service.Queries.DTOs.Contratos
{
    public class ServicioContratoDto
    {
        public int Id { get; set; }
        public int ContratoId { get; set; }
        public int ServicioId { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public decimal PorcentajeImpuesto { get; set; }
    }
}

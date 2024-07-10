using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.EventHandler.Commands.EntregableContratacion
{
    public class EntregableContratacionUpdateCommand : IRequest<int>
    {
        public int Id { get; set; }
        public string? UsuarioId { get; set; } = string.Empty;
        public int ContratoId { get; set; }
        public int ConvenioId { get; set; }
        public int VariableId { get; set; }
        public IFormFile Archivo { get; set; }
        public Nullable<DateTime> FechaProgramada { get; set; }
        public Nullable<DateTime> FechaEntrega { get; set; }
        public Nullable<DateTime> InicioVigencia { get; set; }
        public Nullable<DateTime> FinVigencia { get; set; }
        public Nullable<decimal> MontoGarantia { get; set; }
        public Nullable<bool> Penalizable { get; set; }
        public Nullable<decimal> MontoPenalizacion { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public System.Nullable<DateTime> FechaCreacion { get; set; }
        public System.Nullable<DateTime> FechaActualizacion { get; set; }

        public string Contrato { get; set; }
        public string? Convenio { get; set; } = string.Empty;
        public string TipoEntregable { get; set; }

    }
}

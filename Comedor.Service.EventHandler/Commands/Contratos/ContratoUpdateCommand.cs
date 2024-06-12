using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.EventHandler.Commands.Contratos
{
    public class ContratoUpdateCommand : IRequest<int>
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public string NoContrato { get; set; }
        public string Empresa { get; set; }
        public string RFC { get; set; }
        public string Direccion { get; set; }
        public decimal MontoMin { get; set; }
        public decimal MontoMax { get; set; }
        public int VolumetriaMin { get; set; }
        public int VolumetriaMax { get; set; }
        public string Representante { get; set; }
        public DateTime InicioVigencia { get; set; }
        public DateTime FinVigencia { get; set; }
        public DateTime FechaFirmaContrato { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}

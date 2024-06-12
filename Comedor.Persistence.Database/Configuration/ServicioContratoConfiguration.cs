using Comedor.Domain.DContratos;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comedor.Persistence.Database.Configuration
{
    public class ServicioContratoConfiguration
    {
        public ServicioContratoConfiguration(EntityTypeBuilder<ServicioContrato> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
        }
    }
}

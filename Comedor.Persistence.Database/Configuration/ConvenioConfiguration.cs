using Comedor.Domain.DContratos;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comedor.Persistence.Database.Configuration
{
    public class ConvenioConfiguration
    {
        public ConvenioConfiguration(EntityTypeBuilder<Convenio> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
        }
    }
}

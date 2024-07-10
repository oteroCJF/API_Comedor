using Comedor.Domain.DContratos;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comedor.Persistence.Database.Configuration
{
    public class RubroConvenioConfiguration
    {
        public RubroConvenioConfiguration(EntityTypeBuilder<RubroConvenio> entityBuilder)
        {
            entityBuilder.HasKey(x => new { x.RubroId, x.ConvenioId });
        }
    }
}

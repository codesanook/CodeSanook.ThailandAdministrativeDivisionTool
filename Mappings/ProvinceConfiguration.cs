using Codesanook.ThailandAdministrativeDivisionTool.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codesanook.ThailandAdministrativeDivisionTool.Mappings
{
    public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
    {
        public void Configure(EntityTypeBuilder<Province> builder)
        {
            builder.ToTable(nameof(Province));
            builder.HasKey(x => x.Id);

            builder
                .Property(x => x.Code)
                .IsRequired();

            builder
                .Property(x => x.NameInThai)
                .IsRequired();

            builder
                .Property(x => x.NameInEnglish)
                .IsRequired();
        }
    }
}

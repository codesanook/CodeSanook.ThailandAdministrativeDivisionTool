using Codesanook.ThailandAdministrativeDivisionTool.Models;
using OrchardCore.Data.Migration;

namespace Codesanook.ThailandAdministrativeDivisionTool
{
    public class Migrations : DataMigration
    {
        public int Create()
        {
            // To get migration data
            // SELECT Content
            // FROM [Document]
            // WHERE [Type] = 'OrchardCore.Data.Migration.Records.DataMigrationRecord, OrchardCore.Data'
            SchemaBuilder.CreateTable(nameof(Province), table => table
                .Column<int>(nameof(Province.Id), col => col.PrimaryKey().Identity())
                .Column<string>(nameof(Province.Code), col => col.NotNull().WithLength(255))
                .Column<string>(nameof(Province.NameInThai), col => col.NotNull().WithLength(255))
                .Column<string>(nameof(Province.NameInEnglish), col => col.NotNull().WithLength(255))
            );

            return 1;
        }
    }
}

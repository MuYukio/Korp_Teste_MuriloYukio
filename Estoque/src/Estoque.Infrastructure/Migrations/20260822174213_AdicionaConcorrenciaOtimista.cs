using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estoque.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaConcorrenciaOtimista : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Sem operação: "xmin" já existe nativamente em toda tabela do
            // PostgreSQL como coluna de sistema. Esta migration apenas
            // registra, no histórico do EF Core, que o modelo passou a
            // mapear essa coluna como token de concorrência otimista
            // (ver Estoque.Infrastructure/Persistence/Configurations/ProdutoConfiguration.cs).
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Sem operação, pelo mesmo motivo do Up().
        }
    }
}
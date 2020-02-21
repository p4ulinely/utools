using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace utools.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    cnpj = table.Column<string>(nullable: false),
                    tipo = table.Column<string>(nullable: true),
                    abertura = table.Column<string>(nullable: true),
                    nome = table.Column<string>(nullable: true),
                    fantasia = table.Column<string>(nullable: true),
                    porte = table.Column<string>(nullable: true),
                    natureza_juridica = table.Column<string>(nullable: true),
                    logradouro = table.Column<string>(nullable: true),
                    numero = table.Column<int>(nullable: false),
                    complemento = table.Column<string>(nullable: true),
                    cep = table.Column<string>(nullable: true),
                    bairro = table.Column<string>(nullable: true),
                    municipio = table.Column<string>(nullable: true),
                    uf = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    telefone = table.Column<string>(nullable: true),
                    efr = table.Column<string>(nullable: true),
                    situacao = table.Column<string>(nullable: true),
                    data_situacao = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.cnpj);
                });

            migrationBuilder.CreateTable(
                name: "Cnaes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(nullable: true),
                    text = table.Column<string>(nullable: true),
                    Empresacnpj = table.Column<string>(nullable: true),
                    Empresacnpj1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cnaes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cnaes_Empresas_Empresacnpj",
                        column: x => x.Empresacnpj,
                        principalTable: "Empresas",
                        principalColumn: "cnpj",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cnaes_Empresas_Empresacnpj1",
                        column: x => x.Empresacnpj1,
                        principalTable: "Empresas",
                        principalColumn: "cnpj",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cnaes_Empresacnpj",
                table: "Cnaes",
                column: "Empresacnpj");

            migrationBuilder.CreateIndex(
                name: "IX_Cnaes_Empresacnpj1",
                table: "Cnaes",
                column: "Empresacnpj1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cnaes");

            migrationBuilder.DropTable(
                name: "Empresas");
        }
    }
}

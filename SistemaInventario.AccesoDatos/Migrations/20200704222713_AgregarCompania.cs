using Microsoft.EntityFrameworkCore.Migrations;

namespace SistemaInventario.AccesoDatos.Migrations
{
    public partial class AgregarCompania : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Compania",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(maxLength: 80, nullable: false),
                    Descripcion = table.Column<string>(maxLength: 200, nullable: false),
                    Pais = table.Column<string>(maxLength: 60, nullable: false),
                    Ciudad = table.Column<string>(maxLength: 60, nullable: false),
                    Direccion = table.Column<string>(maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(maxLength: 40, nullable: false),
                    BodegaVentaId = table.Column<int>(nullable: false),
                    LogoUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compania", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compania_Bodegas_BodegaVentaId",
                        column: x => x.BodegaVentaId,
                        principalTable: "Bodegas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Compania_BodegaVentaId",
                table: "Compania",
                column: "BodegaVentaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Compania");
        }
    }
}

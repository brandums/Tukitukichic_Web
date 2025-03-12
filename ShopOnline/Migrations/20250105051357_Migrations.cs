using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopOnline.Migrations
{
    /// <inheritdoc />
    public partial class Migrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Opiniones",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Calificacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data4 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opiniones", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "PrincipalStructs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Canal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubstructsData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpinionesData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data15 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data16 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data17 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data18 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrincipalStructs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Substruct",
                columns: table => new
                {
                    Date = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Precio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BreveDescripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Talla = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubCategoria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Images = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extra1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extra2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extra3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extra4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extra5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extra6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extra7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extra8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TiempoOferta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ventas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VentasBase = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LikesBase = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Substruct", x => x.Date);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CI = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Opiniones");

            migrationBuilder.DropTable(
                name: "PrincipalStructs");

            migrationBuilder.DropTable(
                name: "Substruct");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

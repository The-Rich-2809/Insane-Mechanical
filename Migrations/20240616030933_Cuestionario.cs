using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insane_Mechanical.Migrations
{
    /// <inheritdoc />
    public partial class Cuestionario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Preguntas",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preguntas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Opciones",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsCorrecta = table.Column<bool>(type: "bit", nullable: false),
                    idPregunta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opciones", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Opciones_Preguntas_idPregunta",
                        column: x => x.idPregunta,
                        principalTable: "Preguntas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RespuestaUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idPregunta = table.Column<int>(type: "int", nullable: false),
                    Respuesta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    idUsuario = table.Column<int>(type: "int", nullable: false),
                    idOpcion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespuestaUsuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RespuestaUsuario_Opciones_idOpcion",
                        column: x => x.idOpcion,
                        principalTable: "Opciones",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RespuestaUsuario_Preguntas_idPregunta",
                        column: x => x.idPregunta,
                        principalTable: "Preguntas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RespuestaUsuario_Usuario_idUsuario",
                        column: x => x.idUsuario,
                        principalTable: "Usuario",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Opciones_idPregunta",
                table: "Opciones",
                column: "idPregunta");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestaUsuario_idOpcion",
                table: "RespuestaUsuario",
                column: "idOpcion");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestaUsuario_idPregunta",
                table: "RespuestaUsuario",
                column: "idPregunta");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestaUsuario_idUsuario",
                table: "RespuestaUsuario",
                column: "idUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RespuestaUsuario");

            migrationBuilder.DropTable(
                name: "Opciones");

            migrationBuilder.DropTable(
                name: "Preguntas");
        }
    }
}

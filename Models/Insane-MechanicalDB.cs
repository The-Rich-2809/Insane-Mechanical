using Microsoft.EntityFrameworkCore;

namespace Insane_Mechanical.Models
{
    public class Insane_MechanicalDB : DbContext
    {
        public Insane_MechanicalDB(DbContextOptions<Insane_MechanicalDB> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<Comentario> Comentario { get; set; }
        public DbSet<Pregunta> Preguntas { get; set; }
        public DbSet<Opcion> Opciones { get; set; }
        public DbSet<RespuestasUsuario> RespuestaUsuario { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RespuestasUsuario>()
            .HasOne(r => r.Usuario)
            .WithMany(u => u.Respuestas)
            .HasForeignKey(r => r.idUsuario)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RespuestasUsuario>()
            .HasOne(r => r.Pregunta)
            .WithMany()
            .HasForeignKey(r => r.idPregunta)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RespuestasUsuario>()
                .HasOne(r => r.Opcion)
                .WithMany()
                .HasForeignKey(r => r.idOpcion)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
            .Property(u => u.Codigcadena)
            .HasMaxLength(6);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.FechadeExpiracion);
        }
    }
}

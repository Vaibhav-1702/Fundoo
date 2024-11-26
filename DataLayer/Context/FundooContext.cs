using Microsoft.EntityFrameworkCore;
using Model.Model;

namespace DataLayer.Context
{
    public class FundooContext : DbContext
    {
        public FundooContext(DbContextOptions<FundooContext> options) : base(options)
        {
        }

        public DbSet<User> users { get; set; }
        public DbSet<Note> notes { get; set; }
        public DbSet<Collaborator> Collaborators { get; set; }
        public DbSet<Label> Labels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Note>()
                .HasOne(n => n.User)  
                .WithMany(u => u.Notes)  
                .HasForeignKey(n => n.UserId)  
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Collaborator>()
               .HasOne(c => c.User)
               .WithMany(u => u.Collaborators)
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            modelBuilder.Entity<Collaborator>()
                .HasOne(c => c.Note)
                .WithMany(n => n.Collaborators)
                .HasForeignKey(c => c.NoteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Note>()
                .HasMany(n => n.Labels)
                .WithMany(l => l.Notes)
                .UsingEntity(j => j.ToTable("NoteLabels"));
        }
    }
}

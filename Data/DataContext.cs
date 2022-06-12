using Microsoft.EntityFrameworkCore;
using dotnetrpg.models;
using Microsoft.Extensions.Configuration;

namespace dotnetrpg.Data
{
  public class DataContext : DbContext
  {
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
      var connectionString = Configuration.GetConnectionString("DefaultConnection");
      options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Skill>().HasData(
        new Skill { Id = 1, Name = "Fireball", Damage = 30 },
        new Skill { Id = 2, Name = "Punchup", Damage = 20 },
        new Skill { Id = 3, Name = "Blizzard", Damage = 50 }
      );
    }

    public DbSet<Character> Characters { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Weapon> Weapons { get; set; }
    public DbSet<Skill> Skills { get; set; }
  }
}
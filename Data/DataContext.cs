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

    public DbSet<Character> Characters { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Weapon> Weapons { get; set; }
  }
}
using DTA.FUS.EFCompiledModels;
using DTA.Models.Files;
using Microsoft.EntityFrameworkCore;

namespace FUS.Data;

public partial class FileContext : DbContext
{
    public DbSet<FileModel>? Files { get; set; }
}

#if AOT

public partial class FileContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var model = FileContextModel.Instance;
        var connectionString = DbConfiguration.DefaultConnectionString;
        
        optionsBuilder
            .UseModel(model)
            .UseNpgsql(connectionString);
    }
}

#elif JIT

public partial class FileContext : DbContext
{
    public FileContext(DbContextOptions<FileContext> options) : base(options)
    { }
}

#endif
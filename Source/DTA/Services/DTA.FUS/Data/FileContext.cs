using Microsoft.EntityFrameworkCore;
using FUS.Models;

namespace FUS.Data;

public class FileContext : DbContext
{
    public FileContext(DbContextOptions<FileContext> options) : base(options) { }

    public DbSet<FileModel> Files { get; set; }
}
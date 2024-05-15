using Microsoft.EntityFrameworkCore;
using silicon_verificationprovider_bmfl.Data.Entities;

namespace silicon_verificationprovider_bmfl.Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<VerificationRequestEntity> VerificationRequests { get; set; }
}

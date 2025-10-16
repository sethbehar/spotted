// File: CloudyContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class CloudyContextFactory : IDesignTimeDbContextFactory<CloudyContext>
{
    public CloudyContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CloudyContext>()
            .UseNpgsql("Host=localhost;Database=cloudy;")
            .Options;

        return new CloudyContext(options);
    }
}

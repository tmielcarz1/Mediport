namespace WebApplication1.Infrastructure.Initializer
{
    public interface IDatabaseInitalizer
    {
        Task SeedData(CancellationToken ct);
    }
}

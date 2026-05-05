using Microsoft.EntityFrameworkCore;
using WebApplication1.Application.Services.Tags;
using WebApplication1.Infrastructure.Context;

namespace WebApplication1.Infrastructure.Initializer
{
    public class DatabaseInitializer : IDatabaseInitalizer
    {
        private readonly MediportDbContext _context;
        private readonly ITagsService _tagsService;

        public DatabaseInitializer(MediportDbContext context, ITagsService tagsService)
        {
            _context = context;
            _tagsService = tagsService;
        }

        public async Task SeedData(CancellationToken ct = default)
        {
            try
            {
                if (await _context.Tags.AnyAsync())
                    return;

                await _tagsService.SyncTags(ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

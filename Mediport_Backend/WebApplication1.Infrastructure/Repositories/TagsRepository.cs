using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebApplication1.Application.Interfaces;
using WebApplication1.Application.Models.Tags.GetTags.Requests;
using WebApplication1.Application.Models.Tags.GetTags.Responses;
using WebApplication1.Domain.Entities;
using WebApplication1.Infrastructure.Context;

namespace WebApplication1.Infrastructure.Repositories
{
    public class TagsRepository : ITagsRepository
    {
        private readonly MediportDbContext _mediportDbContext;

        public TagsRepository(MediportDbContext mediportDbContext)
        {
            _mediportDbContext = mediportDbContext;
        }

        public async Task SyncListTags(List<Tag> tags, CancellationToken ct)
        {
            var tagsToAdd = new List<Tag>();

            var normalizedTags = tags
                .Where(t => !string.IsNullOrWhiteSpace(t.Name))
                .GroupBy(t => t.Name!.Trim())
                .Select(g => g.Last())
                .ToList();

            var tagNames = normalizedTags
                .Select(t => t.Name!)
                .ToList();

            var existingTags = await _mediportDbContext.Tags
                .Where(t => tagNames.Contains(t.Name!))
                .ToListAsync(ct);

            var existingDict = existingTags.ToDictionary(t => t.Name!);

            foreach (var tag in normalizedTags)
            {
                var name = tag.Name!.Trim();

                if (existingDict.TryGetValue(name, out var existingTag))
                {
                    if (existingTag.Count != tag.Count)
                        existingTag.Count = tag.Count;
                }
                else
                {
                    tagsToAdd.Add(new Tag
                    {
                        Name = name,
                        Count = tag.Count
                    });
                }
            }

            if (tagsToAdd.Count > 0)
                await _mediportDbContext.Tags.AddRangeAsync(tagsToAdd, ct);

            await _mediportDbContext.SaveChangesAsync(ct);
        }

        public async Task<List<Tag>> GetAllTags(CancellationToken ct) =>
            await _mediportDbContext.Tags.ToListAsync(ct);

        public async Task<GetTagsResponse> GetTags(GetTagsRequest request, CancellationToken ct)
        {
            var parameters = new List<SqlParameter>();
            var baseSql = @"SELECT 
                    t.Id,
                    t.Name,
                    t.[Count],
                    CAST(t.[Count] * 100.0 / s.TotalCount AS decimal(10,2)) AS Percentage
                FROM Tags t
                CROSS JOIN (
                    SELECT SUM([Count]) AS TotalCount
                    FROM Tags
                ) s ";

            if (!string.IsNullOrWhiteSpace(request.SearchValue))
            {
                if (decimal.TryParse(request.SearchValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var number))
                {
                    baseSql += @"WHERE CAST(CAST(t.[Count] * 100.0 / NULLIF(s.TotalCount, 0) AS decimal(10,2)) AS varchar(20)) LIKE @Value";

                    parameters.Add(new SqlParameter("@Value", $"{request.SearchValue}%"));
                }
                else
                {
                    baseSql += @" WHERE t.Name LIKE @Value";
                    parameters.Add(new SqlParameter("@Value", $"%{request.SearchValue}%"));
                }
            }

            //liczenie wszystkich
            var countSql = $@"SELECT COUNT(*) as Count
                FROM (
                    {baseSql}
                ) AS TagStats";

            //sortowanie
            var orderBy = request.OrderBy switch
            {
                GetTagsRequestOrderBy.Name => "Name",
                GetTagsRequestOrderBy.Percentage => "Percentage",
                _ => "Name"
            };
            var sortDir = request.SortBy switch
            {
                GetTagsRequestSortBy.Ascending => "ASC",
                GetTagsRequestSortBy.Descending => "DESC",
                _ => "ASC"
            };

            baseSql += $@" ORDER BY {orderBy} {sortDir}
                           OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            //paginacja
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 1 : request.PageSize;
            parameters.Add(new SqlParameter("@Offset", (pageNumber - 1) * pageSize));
            parameters.Add(new SqlParameter("@PageSize", pageSize));

            var tagList = await _mediportDbContext.Database
                .SqlQueryRaw<GetTagsResponseList>(baseSql, parameters.ToArray())
                .ToListAsync(ct);

            var countResult = await _mediportDbContext.Database
                .SqlQueryRaw<GetTagsResponseCount>(countSql, parameters.ToArray())
                .SingleAsync(ct);

            return new GetTagsResponse()
            {
                Tags = tagList,
                Count = countResult.Count
            };
        }


    }
}

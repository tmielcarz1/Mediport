using BlazorApp2.Models.Common;
using BlazorApp2.Models.GetTags.Request;
using BlazorApp2.Models.GetTags.Response;
using Microsoft.AspNetCore.Components;

namespace BlazorApp2.Components.Pages;

public partial class Tags
{
    [Inject] public HttpClient Http { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;

    protected List<GetTagsResponseList> items = new();

    protected int _totalCount = 0;
    protected int _pageNumber = 1;
    protected int _pageSize = 10;

    private CancellationTokenSource? _cts;

    protected GetTagsRequest request = new()
    {
        PageNumber = 1,
        PageSize = 10,
        OrderBy = GetTagsRequestOrderBy.Name,
        SortBy = GetTagsRequestSortBy.Ascending,
        SearchValue = ""
    };

    protected int TotalPages =>
        _totalCount == 0
            ? 1
            : (int)Math.Ceiling((double)_totalCount / _pageSize);

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    protected async Task LoadData()
    {
        request.PageNumber = _pageNumber;
        request.PageSize = _pageSize;

        try
        {
			Console.WriteLine(Http.BaseAddress);
            var response = await Http.PostAsJsonAsync("api/Tags/GetTags", request);
            var state = await response.Content.ReadFromJsonAsync<State<GetTagsResponse>>();

            if (state is null)
            {
                Navigation.NavigateTo("/error");
                return;
            }

            items = state.StateObject?.Tags ?? new();
            _totalCount = state.StateObject?.Count ?? 0;

            var maxPage = Math.Max(1, TotalPages);

            if (_pageNumber > maxPage)
                _pageNumber = maxPage;

            if (_pageNumber < 1)
                _pageNumber = 1;
        }
        catch
        {
            Navigation.NavigateTo("/error");
            return;
        }
    }


    protected async Task OnSearchInput(ChangeEventArgs e)
    {
        request.SearchValue = e.Value?.ToString();
        _pageNumber = 1;

        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            await Task.Delay(300, _cts.Token);
            await LoadData();
        }
        catch (TaskCanceledException) { }
    }


    protected async Task SortBy(GetTagsRequestOrderBy orderBy)
    {
        if (request.OrderBy == orderBy)
        {
            request.SortBy = request.SortBy == GetTagsRequestSortBy.Ascending
                ? GetTagsRequestSortBy.Descending
                : GetTagsRequestSortBy.Ascending;
        }
        else
        {
            request.OrderBy = orderBy;
            request.SortBy = GetTagsRequestSortBy.Ascending;
        }

        _pageNumber = 1;
        await LoadData();
    }

    protected string GetSortIcon(GetTagsRequestOrderBy column)
    {
        if (request.OrderBy != column)
            return "⇅";

        return request.SortBy == GetTagsRequestSortBy.Ascending ? "▲" : "▼";
    }


    protected async Task OnPageChanged(int page)
    {
        _pageNumber = page;
        await LoadData();
    }

    protected async Task OnPageSizeChanged(int size)
    {
        _pageSize = size;
        _pageNumber = 1;
        await LoadData();
    }

}
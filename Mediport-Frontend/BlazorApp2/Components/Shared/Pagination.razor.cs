using Microsoft.AspNetCore.Components;

namespace BlazorApp2.Components.Shared;

public partial class Pagination
{
    [Parameter] public int CurrentPage { get; set; }
    [Parameter] public int TotalPages { get; set; }
    [Parameter] public int PageSize { get; set; }

    [Parameter] public int PageSizeDefault { get; set; } = 10;

    [Parameter] public EventCallback<int> CurrentPageChanged { get; set; }
    [Parameter] public EventCallback<int> PageSizeChanged { get; set; }

    [Parameter] public List<int> PageSizeOptions { get; set; } = new() { 5, 10, 15 };

    protected List<int> Pages => BuildPages();

    private List<int> BuildPages()
    {
        if (TotalPages <= 0)
            return new();

        if (TotalPages <= 10)
            return Enumerable.Range(1, TotalPages).ToList();

        var start = Math.Max(1, CurrentPage - 2);
        var end = Math.Min(TotalPages, start + PageSizeDefault - 1);

        if (end - start < PageSizeDefault)
            start = Math.Max(1, end - PageSizeDefault + 1);

        return Enumerable.Range(start, end - start + 1).ToList();
    }

    private async Task SelectPage(int page)
    {
        if (page == CurrentPage)
            return;

        await CurrentPageChanged.InvokeAsync(page);
    }

    private async Task GoPrevious()
    {
        if (CurrentPage <= 1)
            return;

        await CurrentPageChanged.InvokeAsync(CurrentPage - 1);
    }

    private async Task GoNext()
    {
        if (CurrentPage >= TotalPages)
            return;

        await CurrentPageChanged.InvokeAsync(CurrentPage + 1);
    }


    private async Task OnPageSizeChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var size))
        {
            await PageSizeChanged.InvokeAsync(size);
        }
    }
}
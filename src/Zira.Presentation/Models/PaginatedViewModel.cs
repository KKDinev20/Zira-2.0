using System.Collections.Generic;

namespace Zira.Presentation.Models;

public class PaginatedViewModel<T>
{
    public List<T> Items { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }

    public int ReminderCount { get; set; }
}
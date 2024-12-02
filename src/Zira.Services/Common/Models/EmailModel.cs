namespace Zira.Services.Common.Models;

public class EmailModel
{
    public required string ToEmail { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
}
namespace Zira.Services.Common.Options;

public class EmailSmtpOptions
{
    public required string Host { get; set; }

    public required int Port { get; set; }

    public required bool EnableSsl { get; set; }
}
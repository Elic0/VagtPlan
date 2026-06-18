using System.Globalization;

namespace VagtPlan.Web.Helpers;

public static class DateDisplayHelper
{
    public const string DisplayPattern = "dd/MM/yyyy";

    private static readonly CultureInfo DanishCulture = CultureInfo.GetCultureInfo("da-DK");

    public static string Format(DateTime date) => date.ToString(DisplayPattern, DanishCulture);

    public static string Format(DateOnly date) => date.ToString(DisplayPattern, DanishCulture);

    public static string Format(DateTime? date) => date.HasValue ? Format(date.Value) : "-";

    public static string Format(DateTimeOffset date) => Format(date.LocalDateTime);

    public static bool TryParse(string? input, out DateTime date) =>
        DateTime.TryParseExact(input?.Trim(), DisplayPattern, DanishCulture, DateTimeStyles.None, out date);
}

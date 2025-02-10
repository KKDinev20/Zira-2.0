using System.Globalization;
using Zira.Common;
using Zira.Data.Enums;

namespace Zira.Presentation.Extensions;

public static class EnumExtensions
{
    public static string GetLocalizedName(this Categories category)
    {
        var resourceManager = ExpenseCategories.ResourceManager;
        var culture = CultureInfo.CurrentUICulture;
        var localizedValue = resourceManager.GetString(category.ToString(), culture);

        return localizedValue ?? category.ToString();
    }
}
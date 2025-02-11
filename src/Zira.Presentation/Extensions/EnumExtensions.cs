using System.Globalization;
using Zira.Common;
using Zira.Data.Enums;

namespace Zira.Presentation.Extensions;

public static class EnumExtensions
{
    public static string GetLocalizedExpenseCategory(this Categories category)
    {
        var resourceManager = ExpenseCategories.ResourceManager;
        var culture = CultureInfo.CurrentUICulture;
        var localizedValue = resourceManager.GetString(category.ToString(), culture);

        return localizedValue ?? category.ToString();
    }

    public static string GetLocalizedIncomeSource(this Sources sources)
    {
        var resourceManager = IncomeSources.ResourceManager;
        var culture = CultureInfo.CurrentUICulture;
        var localizedValue = resourceManager.GetString(sources.ToString(), culture);

        return localizedValue ?? sources.ToString();
    }
}
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Domain.Shared.Enum;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace ExpenseFlow.Application.Services.Helper;


public static class QueryFilterHelper
{
    public static IQueryable<T> ApplyFilters<T>(
        IQueryable<T> query,
        List<FilterCriterionDto> filters,
        bool isAnd,
        string language,
        ParsingConfig config = null) where T : class
    {
        if (filters == null || !filters.Any())
            return query;

        config ??= ParsingConfig.Default;

        var predicateBuilder = new StringBuilder();
        var values = new List<object>();

        for (int i = 0; i < filters.Count; i++)
        {
            var filter = filters[i];

            // Normalize property path to match actual CLR property names (case-correct)
            var normalizedProp = GetNormalizedPropertyPath(typeof(T), filter.PropertyName);
            if (normalizedProp == null)
                throw new NotFoundException(
                    $"Property '{filter.PropertyName}' was not found on type '{typeof(T).Name}'.");

            if (i > 0)
                predicateBuilder.Append(isAnd ? " && " : " || ");

            // Get property type
            var propertyInfo = GetPropertyInfo(typeof(T), normalizedProp);
            var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            bool isTranslatedProperty = (propertyType == typeof(LanguagePropertyModel));

            switch (filter.Operator)
            {
                case FilterOperator.Equals:
                    if (propertyType.IsEnum)
                    {
                        predicateBuilder.Append($"{normalizedProp} == @{values.Count}");
                        values.Add(Enum.Parse(propertyType, filter.Value));
                    }
                    else if (propertyType == typeof(Guid))
                    {
                        predicateBuilder.Append($"{normalizedProp} == @{values.Count}");
                        values.Add(Guid.Parse(filter.Value));
                    }
                    else if (isTranslatedProperty)
                    {
                        predicateBuilder.Append($"LanguagePropertyModelExtension.IsEquals({normalizedProp}, @{values.Count})");
                        values.Add(filter.Value);
                    }
                    else if (propertyType == typeof(DateTime))
                    {
                        var date = DateTime.Parse(filter.Value).Date;
                        var startOfDay = date;
                        var endOfDay = date.AddDays(1);

                        predicateBuilder.Append($"({normalizedProp} >= @{values.Count} && {normalizedProp} < @{values.Count + 1})");
                        values.Add(startOfDay);
                        values.Add(endOfDay);
                    }
                    else
                    {
                        predicateBuilder.Append($"{normalizedProp} == @{values.Count}");
                        values.Add(Convert.ChangeType(filter.Value, propertyType));
                    }
                    break;

                case FilterOperator.Contains:
                    if (isTranslatedProperty)
                    {
                        predicateBuilder.Append($"LanguagePropertyModelExtension.Search({normalizedProp}, @{values.Count})");
                    }
                    else
                    {
                        predicateBuilder.Append($"{normalizedProp}.Contains(@{values.Count})");
                    }
                    values.Add(filter.Value);
                    break;

                case FilterOperator.StartsWith:
                    if (isTranslatedProperty)
                    {
                        predicateBuilder.Append($"LanguagePropertyModelExtension.StartsWith({normalizedProp}, @{values.Count})");
                    }
                    else
                    {
                        predicateBuilder.Append($"{normalizedProp}.StartsWith(@{values.Count})");
                    }
                    values.Add(filter.Value);
                    break;

                case FilterOperator.EndsWith:
                    if (isTranslatedProperty)
                    {
                        predicateBuilder.Append($"LanguagePropertyModelExtension.EndsWith({normalizedProp}, @{values.Count})");
                    }
                    else
                    {
                        predicateBuilder.Append($"{normalizedProp}.EndsWith(@{values.Count})");
                    }
                    values.Add(filter.Value);
                    break;

                case FilterOperator.GreaterThan:
                    predicateBuilder.Append($"{normalizedProp} > @{values.Count}");
                    values.Add(Convert.ChangeType(filter.Value, propertyType));
                    break;

                case FilterOperator.LessThan:
                    predicateBuilder.Append($"{normalizedProp} < @{values.Count}");
                    values.Add(Convert.ChangeType(filter.Value, propertyType));
                    break;

                case FilterOperator.NotEquals:
                    if (isTranslatedProperty)
                    {
                        predicateBuilder.Append($"LanguagePropertyModelExtension.IsNotEquals({normalizedProp}, @{values.Count})");
                        values.Add(filter.Value);
                    }
                    else
                    {
                        predicateBuilder.Append($"{normalizedProp} != @{values.Count}");
                        values.Add(Convert.ChangeType(filter.Value, propertyType));
                    }
                    break;

                case FilterOperator.EqualsOrGreaterThan:
                    predicateBuilder.Append($"{normalizedProp} >= @{values.Count}");
                    values.Add(Convert.ChangeType(filter.Value, propertyType));
                    break;

                case FilterOperator.EqualsOrLessThan:
                    predicateBuilder.Append($"{normalizedProp} <= @{values.Count}");
                    values.Add(Convert.ChangeType(filter.Value, propertyType));
                    break;

                case FilterOperator.Empty:
                    if (isTranslatedProperty)
                    {
                        predicateBuilder.Append($"LanguagePropertyModelExtension.IsEmptyVal({normalizedProp})");
                    }
                    else
                    {
                        // FIX: use || not OR
                        predicateBuilder.Append($"({normalizedProp} == null || {normalizedProp} == \"\")");
                    }
                    break;

                case FilterOperator.NotEmpty:
                    if (isTranslatedProperty)
                    {
                        predicateBuilder.Append($"LanguagePropertyModelExtension.IsNotEmptyVal({normalizedProp})");
                    }
                    else
                    {
                        // FIX: use && not AND
                        predicateBuilder.Append($"({normalizedProp} != null && {normalizedProp} != \"\")");
                    }
                    break;
            }
        }

        if (predicateBuilder.Length > 0)
        {
            // Optional debug logs (remove later)
            Console.WriteLine("Dynamic predicate: " + predicateBuilder.ToString());
            Console.WriteLine("Values: " + string.Join(", ", values.Select(v => v?.ToString())));

            query = query.Where(config, predicateBuilder.ToString(), values.ToArray());
        }

        return query;
    }

    private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return null;

        var parts = propertyName.Split('.');
        var currentType = type;
        PropertyInfo propertyInfo = null;

        foreach (var part in parts)
        {
            propertyInfo = currentType.GetProperty(part, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
                return null;

            currentType = propertyInfo.PropertyType;
        }

        return propertyInfo;
    }

    // NEW: Normalize path to correct case (so Dynamic LINQ won't choke on city vs City)
    private static string GetNormalizedPropertyPath(Type type, string propertyPath)
    {
        if (string.IsNullOrWhiteSpace(propertyPath))
            return null;

        var parts = propertyPath.Split('.');
        var currentType = type;
        var normalizedParts = new List<string>();

        foreach (var part in parts)
        {
            var pi = currentType.GetProperty(part, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (pi == null)
                return null;

            normalizedParts.Add(pi.Name);
            currentType = pi.PropertyType;
        }

        return string.Join(".", normalizedParts);
    }
}

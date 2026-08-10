using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace ExpenseFlow.Domain.Base.Language
{
    public static class LanguagePropertyModelExtension
    {
        public static PropertyBuilder<LanguagePropertyModel> JsonConverter(this PropertyBuilder<LanguagePropertyModel> propertybuilder)
        {
            var comparer = new ValueComparer<LanguagePropertyModel>(
                (l1, l2) => JsonConvert.SerializeObject(l1) == JsonConvert.SerializeObject(l2),
                v => v == null ? 0 : JsonConvert.SerializeObject(v).GetHashCode(),
                v => JsonConvert.DeserializeObject<LanguagePropertyModel>(JsonConvert.SerializeObject(v)));

            propertybuilder
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<LanguagePropertyModel>(v) ?? new LanguagePropertyModel(),
                    comparer)
                .HasColumnType("NVARCHAR(MAX)");

            //RelationalPropertyBuilderExtensions.HasColumnType(conversionBuilder, "NVARCHAR(MAX)");

            return propertybuilder;
        }

        [DbFunction]
        public static bool Search(this LanguagePropertyModel prop, string searchTerm)
        {
            return false;
        }
        [DbFunction]
        public static bool IsEquals(this LanguagePropertyModel prop, string searchTerm)
        {
            return false;
        }

        [DbFunction]
        public static bool IsNotEquals(this LanguagePropertyModel prop, string searchTerm)
        {
            return false;
        }

        [DbFunction]
        public static bool StartsWith(this LanguagePropertyModel prop, string searchTerm)
        {
            return false;
        }

        [DbFunction]
        public static bool EndsWith(this LanguagePropertyModel prop, string searchTerm)
        {
            return false;
        }

        [DbFunction]
        public static bool IsEmptyVal(this LanguagePropertyModel prop)
        {
            return false;
        }
        [DbFunction]
        public static bool IsNotEmptyVal(this LanguagePropertyModel prop)
        {
            return false;
        }
        [DbFunction]
        public static string ToDto(this LanguagePropertyModel prop, string langCode = "en")
        {
            if (!string.IsNullOrEmpty(langCode) && prop.TryGetValue(langCode, out var value))
            {
                return value;
            }

            string[] array = new string[2] { "en", "ar" };
            foreach (string key in array)
            {
                if (prop.TryGetValue(key, out var value2))
                {
                    return value2;
                }
            }

            return prop.Values.FirstOrDefault();
        }


        public static string ToLanguage(this long languageId) => languageId switch
        {
            1 => "ar",
            2 => "en",
            _ => "en",
        };
    }

}


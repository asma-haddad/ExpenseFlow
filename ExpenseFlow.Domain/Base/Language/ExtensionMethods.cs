namespace ExpenseFlow.Domain.Base.Language
{
    public static class ExtensionMethods
    {
        public static LanguagePropertyDto ToDto(this LanguagePropertyModel languagePropertyModel)
        {
            if (languagePropertyModel == null)
                return new LanguagePropertyDto();

            var languagePropertyDto = new LanguagePropertyDto();
            foreach (var (key, value) in languagePropertyModel)
                languagePropertyDto.Add(key, value);
            return languagePropertyDto;
        }
        public static LanguagePropertyModel ToModel(this LanguagePropertyDto languagePropertyDto)
        {
            if (languagePropertyDto == null)
                return new LanguagePropertyModel();

            var languagePropertyModel = new LanguagePropertyModel();
            foreach (var (key, value) in languagePropertyDto)
                languagePropertyModel.Create(key, value);
            return languagePropertyModel;
        }

        public static IQueryable<TModel> ApplyPagination<TModel>(this IQueryable<TModel> source, int pageSize = 10,
            int page = 0)
            => source.Skip(page * pageSize).Take(pageSize);

    }
}

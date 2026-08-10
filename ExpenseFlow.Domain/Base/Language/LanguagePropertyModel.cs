using Newtonsoft.Json;

namespace ExpenseFlow.Domain.Base.Language
{
    public class LanguagePropertyModel : Dictionary<string, string>

    {
        public void Create(string lang, string content)
        {
            if (lang != "en" && lang != "ar")
                throw new NotFoundException($"Your Language {lang} doesn't appear in database");

            Add(lang, content);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

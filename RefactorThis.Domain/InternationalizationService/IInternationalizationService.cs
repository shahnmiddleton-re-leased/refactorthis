namespace RefactorThis.Domain.InternationalizationService
{
    public interface IInternationalizationService
    {
        public string GetTranslationFromKey(TranslationKey translationKey);
    }
}
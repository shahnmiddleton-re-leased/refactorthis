using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Domain
{
    public class InternationalizationService: IInternationalizationService
    {
        private readonly ResourceManager _manager;

        public InternationalizationService()
        {
            var assem = typeof(TranslationKey).Assembly;
            _manager = new ResourceManager(
                assem.GetName().Name + ".Resource.LocalizedResources",
                assem);
        }

        public string GetTranslationFromKey(TranslationKey messageKey)
        {
            var cult = CultureInfo.CurrentCulture;
            var resourceMessage = _manager.GetResourceSet(cult, true, true)?.GetString(messageKey.Key) ?? string.Empty;
            return !string.IsNullOrEmpty(resourceMessage) ? resourceMessage : messageKey.Key;
        }
    }

    public interface IInternationalizationService
    {
        public string GetTranslationFromKey(TranslationKey translationKey);
    }
}

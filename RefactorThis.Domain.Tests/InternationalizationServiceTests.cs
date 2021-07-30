using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RefactorThis.Domain.InternationalizationService;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class InternationalizationServiceTests
    {
        private IInternationalizationService _internationalizationService = null!;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            _internationalizationService = new InternationalizationService.InternationalizationService();
        }

        [TestCase("fr", "Il n'y a pas de facture correspondant à ce paiement")]
        [TestCase("en", "There is no invoice matching this payment")]
        [TestCase("somethingelse", "There is no invoice matching this payment")]
        public void KeyAreTranslated(string culture, string expectedResult)
        {
            if (CultureExist(culture))
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            }
            var result = _internationalizationService.GetTranslationFromKey(new InvoiceNotFound());
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestCase("fr", "not-translated-key")]
        [TestCase("en", "not-translated-key")]
        [TestCase("somethingelse", "not-translated-key")]
        public void WhenKeyNotTranslatedReturnKeyName(string culture, string expectedResult)
        {
            if (CultureExist(culture))
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            }
            var result = _internationalizationService.GetTranslationFromKey(new NonTranslatedKey());
            result.Should().BeEquivalentTo(expectedResult);
        }

        record NonTranslatedKey() : TranslationKey("not-translated-key");

        private static bool CultureExist(string cultureName)
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures).Any(culture => string.Equals(culture.Name, cultureName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}

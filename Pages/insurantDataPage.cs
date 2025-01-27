using Microsoft.Playwright;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PlaywrightTest.Pages
{
    public class InsurantDataPage
    {
        private readonly IPage _page;
        private readonly Dictionary<string, string> _selectors;

        public InsurantDataPage(IPage page)
        {
            _page = page;

            // Carregar seletores do JSON
            var selectorsJson = File.ReadAllText("selectors.json");
            var allSelectors = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(selectorsJson);
            _selectors = allSelectors["InsurantDataPage"];
        }

        public async Task ClickEnterInsurantDataMenu()
        {
            // Usar o seletor do arquivo JSON
            var menu = _page.Locator(_selectors["EnterInsurantDataMenu"]);
            await menu.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            await menu.ClickAsync();
        }

        public async Task EnterFirstName(string firstName)
        {
            await _page.FillAsync(_selectors["FirstNameInput"], firstName);
        }

        public async Task EnterLastName(string lastName)
        {
            await _page.FillAsync(_selectors["LastNameInput"], lastName);
        }

        public async Task EnterBirthDate(string birthDate)
        {
            await _page.FillAsync(_selectors["DateOfBirthInput"], birthDate);
        }        

        public async Task EnterStreetAddress(string address)
        {
            await _page.FillAsync(_selectors["StreetAddressInput"], address);
        }

        public async Task SelectCountry(string country)
        {
            await _page.SelectOptionAsync(_selectors["CountryDropdown"], country);
        }
    }
}

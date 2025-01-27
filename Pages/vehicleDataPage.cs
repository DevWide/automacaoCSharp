using Microsoft.Playwright;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlaywrightTest.Pages
{
    public class VehicleDataPage
    {
        private readonly IPage _page;
        private readonly Dictionary<string, string> _selectors;

        public VehicleDataPage(IPage page)
        {
            _page = page;

            // Carregar os seletores do arquivo JSON
            var selectorsJson = File.ReadAllText("selectors.json");
            var allSelectors = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(selectorsJson);
            _selectors = allSelectors["VehicleDataPage"];
        }

        // Métodos para interagir com os campos
        public async Task SelectMake(string make)
        {
            await _page.Locator(_selectors["MakeDropdown"]).WaitForAsync();
            await _page.SelectOptionAsync(_selectors["MakeDropdown"], make);
        }

        public async Task SelectModel(string model)
        {
            await _page.Locator(_selectors["ModelDropdown"]).WaitForAsync();
            await _page.SelectOptionAsync(_selectors["ModelDropdown"], model);
        }

        public async Task EnterCylinderCapacity(string capacity)
        {
            await _page.FillAsync(_selectors["CylinderCapacityInput"], capacity);
        }

        public async Task EnterEnginePerformance(string performance)
        {
            await _page.FillAsync(_selectors["EnginePerformanceInput"], performance);
        }

        public async Task EnterDateOfManufacture(string date)
        {
            await _page.FillAsync(_selectors["DateOfManufactureInput"], date);
        }

        public async Task SelectNumberOfSeats(string seats)
        {
            var dropdownLocator = _page.Locator(_selectors["NumberOfSeatsDropdown"]);
            await dropdownLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            await dropdownLocator.SelectOptionAsync(seats);
            Console.WriteLine($"Selecionada a opção '{seats}' no campo Number of Seats.");
        }


        public async Task SelectRightHandDriveYes()
        {
            var radioButtonLocator = _page.Locator(_selectors["RightHandDriveYesRadioButton"]);

            // Aguarde o botão de rádio estar visível
            await radioButtonLocator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible
            });

            // Force o clique no botão de rádio
            await radioButtonLocator.ClickAsync(new() { Force = true });

            Console.WriteLine("Selecionado 'Yes' para Right Hand Drive.");
        }


        public async Task SelectFuelType(string fuelType)
        {
            await _page.SelectOptionAsync(_selectors["FuelTypeDropdown"], fuelType);
        }

        public async Task EnterPayload(string payload)
        {
            await _page.FillAsync(_selectors["PayloadInput"], payload);
        }

        public async Task EnterTotalWeight(string weight)
        {
            await _page.FillAsync(_selectors["TotalWeightInput"], weight);
        }

        public async Task EnterListPrice(string price)
        {
            await _page.FillAsync(_selectors["ListPriceInput"], price);
        }

        public async Task EnterLicensePlateNumber(string plateNumber)
        {
            await _page.FillAsync(_selectors["LicensePlateNumberInput"], plateNumber);
        }

        public async Task EnterAnnualMileage(string mileage)
        {
            await _page.FillAsync(_selectors["AnnualMileageInput"], mileage);
        }

        public async Task ClickNext()
        {
            await _page.ClickAsync(_selectors["NextButton"]);
        }

        public async Task CaptureScreenshot(string fileName)
        {
            if (Config.CaptureScreenshots)
            {
                var screenshotFilePath = Path.Combine(Config.ScreenshotPath, fileName);
                await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotFilePath });
                Console.WriteLine($"Screenshot salvo em: {screenshotFilePath}");
            }
        }

    }
}


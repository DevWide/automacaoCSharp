using Microsoft.Playwright;
using PlaywrightTest.Pages;
using System.Threading.Tasks;
using Xunit;

namespace PlaywrightTest.Tests
{
    public class VehicleDataTests : BaseTest
    {
        [Fact, Trait("Category", "VehicleData")]
        public async Task TestVehicleDataForm()
        {
            // Abrir a URL inicial
            await Page.GotoAsync(Config.BaseUrl);

            // Preencher a aba Enter Vehicle Data
            var vehicleDataPage = new VehicleDataPage(Page);
            await vehicleDataPage.SelectMake("Audi");
            await vehicleDataPage.SelectModel("Scooter");
            await vehicleDataPage.EnterCylinderCapacity("1500");
            await vehicleDataPage.EnterEnginePerformance("120");
            await vehicleDataPage.EnterDateOfManufacture("01/01/2020");
            await vehicleDataPage.SelectNumberOfSeats("5");
            await vehicleDataPage.SelectRightHandDriveYes();
            await vehicleDataPage.SelectFuelType("Diesel");
            await vehicleDataPage.EnterPayload("1000");
            await vehicleDataPage.EnterTotalWeight("2000");
            await vehicleDataPage.EnterListPrice("30000");
            await vehicleDataPage.EnterLicensePlateNumber("ABC1234");
            await vehicleDataPage.EnterAnnualMileage("15000");

            // Captura de tela 
            await CaptureScreenshot("VehicleDataTest.png");

            // Clicar em Next
            await vehicleDataPage.ClickNext();

            // Validação que a próxima página foi carregada
            var nextPageElement = Page.Locator("#enterinsurantdata");
            await nextPageElement.WaitForAsync();
            Assert.True(await nextPageElement.IsVisibleAsync(), "A aba Enter Insurant Data não foi carregada.");
           
        }
    }
}


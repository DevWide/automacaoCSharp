using Microsoft.Playwright;
using PlaywrightTest.Pages;
using System.Threading.Tasks;
using Xunit;

namespace PlaywrightTest.Tests
{
    public class InsurantDataTests : BaseTest
    {
        [Fact]
        [Trait("Category", "InsurantData")]
        public async Task TestInsurantDataForm()
        {
            // Navegar para a URL base
            await Page.GotoAsync(Config.BaseUrl);

            // Clicar no menu "Enter Insurant Data"
            var insurantDataPage = new InsurantDataPage(Page);
            await insurantDataPage.ClickEnterInsurantDataMenu();

            // Preencher os campos do formulário
            await insurantDataPage.EnterFirstName("John");
            await insurantDataPage.EnterLastName("Doe");
            await insurantDataPage.EnterBirthDate("01/01/1985");
            await insurantDataPage.EnterStreetAddress("123 Main St");
            await insurantDataPage.SelectCountry("Brazil");

            // Captura de tela (opcional)
            await Page.WaitForTimeoutAsync(3000);
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "InsurantDataTest_Filled.png" });

            // Captura de tela
            await CaptureScreenshot("InsurantDataTest.png");
        }
    }
}

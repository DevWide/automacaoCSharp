using Microsoft.Playwright;
using PlaywrightTest.Models;
using PlaywrightTest.Pages;
using Xunit;

namespace PlaywrightTest.Tests;

/// <summary>
/// Etapa "Enter Vehicle Data" do fluxo de cotacao de seguro.
/// Convencao de nome dos testes: Acao_Cenario_ResultadoEsperado.
/// </summary>
[Trait("Category", "VehicleData")]
public class VehicleDataTests : BaseTest
{
    [Fact]
    public async Task FillVehicleData_WithValidData_KeepsEveryValueInTheForm()
    {
        var vehicle = VehicleData.Valid();

        var vehicleDataPage = await VehicleDataPage.OpenAsync(Page);
        await vehicleDataPage.FillAsync(vehicle);

        await vehicleDataPage.AssertFormMatchesAsync(vehicle);

        // A propria aplicacao confirma o preenchimento: o contador de campos
        // invalidos da aba fica zerado.
        await vehicleDataPage.AssertStepHasNoValidationErrorsAsync();

        await CaptureScreenshotAsync("vehicle-data-filled.png");
    }

    [Fact]
    public async Task ClickNext_WithValidVehicleData_AdvancesToInsurantDataStep()
    {
        var vehicleDataPage = await VehicleDataPage.OpenAsync(Page);

        var insurantDataPage = await vehicleDataPage.FillAndContinueAsync(VehicleData.Valid());

        // ClickNextAsync so devolve o proximo Page Object depois de confirmar a
        // troca de etapa (classe de etapa ativa + title do documento). Esta linha
        // documenta o resultado esperado no proprio teste.
        await insurantDataPage.AssertIsCurrentStepAsync();
    }

    /// <summary>
    /// Cenario negativo dirigido por dados. As regras e as mensagens vem da propria
    /// aplicacao (forms/customization/configuration.js e o plugin idealforms):
    ///   Cylinder Capacity -> range 1..2000
    ///   List Price        -> range 500..100000
    ///   Annual Mileage    -> range 100..100000
    ///   License Plate     -> max 10 caracteres
    /// </summary>
    [Theory]
    [InlineData(VehicleField.CylinderCapacity, "9999", "Must be a number between 1 and 2000")]
    [InlineData(VehicleField.ListPrice, "1", "Must be a number between 500 and 100000")]
    [InlineData(VehicleField.AnnualMileage, "1", "Must be a number between 100 and 100000")]
    [InlineData(VehicleField.LicensePlateNumber, "ABCDEFGHIJK", "Must be under 10 characters")]
    public async Task SetVehicleField_WithValueOutsideTheBusinessRule_MarksFieldAsInvalid(
        VehicleField field,
        string invalidValue,
        string expectedMessage)
    {
        var vehicleDataPage = await VehicleDataPage.OpenAsync(Page);
        await vehicleDataPage.FillAsync(VehicleData.Valid());

        await vehicleDataPage.SetFieldAsync(field, invalidValue);

        await vehicleDataPage.AssertFieldIsInvalidAsync(field, expectedMessage);
    }

    /// <summary>
    /// Contraprova do cenario acima: com valor dentro da regra, o campo nao pode
    /// ficar marcado como invalido. Sem este teste, uma assertiva que sempre passa
    /// (ou sempre falha) nao seria percebida.
    /// </summary>
    [Theory]
    [InlineData(VehicleField.CylinderCapacity, "1500")]
    [InlineData(VehicleField.ListPrice, "30000")]
    [InlineData(VehicleField.AnnualMileage, "15000")]
    [InlineData(VehicleField.LicensePlateNumber, "ABC1234")]
    public async Task SetVehicleField_WithValueInsideTheBusinessRule_DoesNotMarkFieldAsInvalid(
        VehicleField field,
        string validValue)
    {
        var vehicleDataPage = await VehicleDataPage.OpenAsync(Page);
        await vehicleDataPage.FillAsync(VehicleData.Valid());

        await vehicleDataPage.SetFieldAsync(field, validValue);

        await vehicleDataPage.AssertFieldIsValidAsync(field);
    }
}

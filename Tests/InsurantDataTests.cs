using Microsoft.Playwright;
using PlaywrightTest.Models;
using PlaywrightTest.Pages;
using Xunit;

namespace PlaywrightTest.Tests;

/// <summary>
/// Etapa "Enter Insurant Data" do fluxo de cotacao de seguro.
/// Convencao de nome dos testes: Acao_Cenario_ResultadoEsperado.
/// </summary>
[Trait("Category", "InsurantData")]
public class InsurantDataTests : BaseTest
{
    [Fact]
    public async Task OpenStep_FromTheWizardNavigation_ShowsInsurantDataStep()
    {
        var insurantDataPage = await InsurantDataPage.OpenAsync(Page);

        await insurantDataPage.AssertIsCurrentStepAsync();
    }

    [Fact]
    public async Task FillInsurantData_WithValidData_KeepsEveryValueInTheForm()
    {
        var insurant = InsurantData.Valid();

        var insurantDataPage = await InsurantDataPage.OpenAsync(Page);
        await insurantDataPage.FillAsync(insurant);

        await insurantDataPage.AssertFormMatchesAsync(insurant);
        await CaptureScreenshotAsync("insurant-data-filled.png");
    }

    /// <summary>
    /// Cenario negativo dirigido por dados. Regras e mensagens da aplicacao:
    ///   Street Address    -> minimo de 3 caracteres
    ///   First / Last Name -> minimo de 2 caracteres, apenas letras
    /// </summary>
    [Theory]
    [InlineData(InsurantField.StreetAddress, "ab", "Must be at least 3 characters long")]
    [InlineData(InsurantField.StreetAddress, "x", "Must be at least 3 characters long")]
    [InlineData(InsurantField.FirstName, "123", "Must be at least 2 characters long and must only contain letters")]
    [InlineData(InsurantField.LastName, "456", "Must be at least 2 characters long and must only contain letters")]
    public async Task SetInsurantField_WithValueOutsideTheBusinessRule_MarksFieldAsInvalid(
        InsurantField field,
        string invalidValue,
        string expectedMessage)
    {
        var insurantDataPage = await InsurantDataPage.OpenAsync(Page);
        await insurantDataPage.FillAsync(InsurantData.Valid());

        await insurantDataPage.SetFieldAsync(field, invalidValue);

        await insurantDataPage.AssertFieldIsInvalidAsync(field, expectedMessage);
    }

    /// <summary>Contraprova: valor dentro da regra nao pode marcar o campo como invalido.</summary>
    [Theory]
    [InlineData(InsurantField.StreetAddress, "123 Main St")]
    [InlineData(InsurantField.FirstName, "John")]
    [InlineData(InsurantField.LastName, "Doe")]
    public async Task SetInsurantField_WithValueInsideTheBusinessRule_DoesNotMarkFieldAsInvalid(
        InsurantField field,
        string validValue)
    {
        var insurantDataPage = await InsurantDataPage.OpenAsync(Page);
        await insurantDataPage.FillAsync(InsurantData.Valid());

        await insurantDataPage.SetFieldAsync(field, validValue);

        await insurantDataPage.AssertFieldIsValidAsync(field);
    }
}

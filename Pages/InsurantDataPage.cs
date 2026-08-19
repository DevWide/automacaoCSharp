using Microsoft.Playwright;
using PlaywrightTest.Models;

namespace PlaywrightTest.Pages;

/// <summary>Campos da etapa "Enter Insurant Data" que possuem regra de validacao.</summary>
public enum InsurantField
{
    FirstName,
    LastName,
    DateOfBirth,
    StreetAddress
}

/// <summary>
/// Etapa 2 do wizard de cotacao: "Enter Insurant Data".
/// </summary>
public sealed class InsurantDataPage : BasePage
{
    public InsurantDataPage(IPage page) : base(page) { }

    protected override string StepAnchorId => "enterinsurantdata";
    protected override string StepTitle => "Enter Insurant Data";

    /// <summary>Mapa unico entre o dominio do teste e o DOM.</summary>
    private static readonly IReadOnlyDictionary<InsurantField, string> FieldIds =
        new Dictionary<InsurantField, string>
        {
            [InsurantField.FirstName] = "firstname",
            [InsurantField.LastName] = "lastname",
            [InsurantField.DateOfBirth] = "birthdate",
            [InsurantField.StreetAddress] = "streetaddress"
        };

    private ILocator Country => Page.Locator("#country");

    private ILocator Field(InsurantField field) => Page.Locator($"#{FieldIds[field]}");

    /// <summary>
    /// Abre a aplicacao e navega direto para esta etapa pela barra do wizard,
    /// sem depender do preenchimento da etapa anterior.
    /// </summary>
    public static async Task<InsurantDataPage> OpenAsync(IPage page)
    {
        await page.GotoAsync(Config.BaseUrl);

        var insurantDataPage = new InsurantDataPage(page);
        await insurantDataPage.WaitUntilReadyAsync();
        await insurantDataPage.OpenStepAsync();

        return insurantDataPage;
    }

    /// <summary>Preenche a etapa inteira a partir da massa de teste.</summary>
    public async Task FillAsync(InsurantData data)
    {
        await SetFieldAsync(InsurantField.FirstName, data.FirstName);
        await SetFieldAsync(InsurantField.LastName, data.LastName);
        await SetFieldAsync(InsurantField.DateOfBirth, data.DateOfBirth);
        await Country.SelectOptionAsync(data.Country);
        await SetFieldAsync(InsurantField.StreetAddress, data.StreetAddress);
    }

    /// <summary>Escreve em um unico campo, disparando a validacao da aplicacao.</summary>
    public Task SetFieldAsync(InsurantField field, string value) =>
        SetTextAsync(Field(field), value);

    /// <summary>Assertiva de campo reprovado na validacao, com a mensagem esperada.</summary>
    public Task AssertFieldIsInvalidAsync(InsurantField field, string expectedMessage) =>
        AssertFieldIsInvalidAsync(FieldIds[field], expectedMessage);

    /// <summary>Assertiva de campo aprovado na validacao.</summary>
    public Task AssertFieldIsValidAsync(InsurantField field) =>
        AssertFieldIsValidAsync(FieldIds[field]);

    /// <summary>Confere que os valores realmente chegaram ao formulario.</summary>
    public async Task AssertFormMatchesAsync(InsurantData data)
    {
        await Assertions.Expect(Field(InsurantField.FirstName)).ToHaveValueAsync(data.FirstName);
        await Assertions.Expect(Field(InsurantField.LastName)).ToHaveValueAsync(data.LastName);
        await Assertions.Expect(Field(InsurantField.DateOfBirth)).ToHaveValueAsync(data.DateOfBirth);
        await Assertions.Expect(Field(InsurantField.StreetAddress)).ToHaveValueAsync(data.StreetAddress);
        await Assertions.Expect(Country).ToHaveValueAsync(data.Country);
    }
}

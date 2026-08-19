using Microsoft.Playwright;
using PlaywrightTest.Models;

namespace PlaywrightTest.Pages;

/// <summary>Campos da etapa "Enter Vehicle Data" que possuem regra de validacao.</summary>
public enum VehicleField
{
    CylinderCapacity,
    EnginePerformance,
    DateOfManufacture,
    Payload,
    TotalWeight,
    ListPrice,
    LicensePlateNumber,
    AnnualMileage
}

/// <summary>
/// Etapa 1 do wizard de cotacao: "Enter Vehicle Data".
///
/// Os localizadores sao propriedades <see cref="ILocator"/> tipadas e preguicosas:
/// resolvem no momento do uso (sem risco de elemento obsoleto) e um nome errado
/// quebra a compilacao, nao a execucao.
///
/// Sobre a estrategia de localizacao: esta aplicacao nao associa &lt;label&gt; aos campos
/// (nao ha atributo "for", nem aria-label), entao GetByLabel/GetByRole nao produzem
/// nome acessivel aqui. O id e o contrato mais estavel disponivel.
/// </summary>
public sealed class VehicleDataPage : BasePage
{
    public VehicleDataPage(IPage page) : base(page) { }

    protected override string StepAnchorId => "entervehicledata";
    protected override string StepTitle => "Enter Vehicle Data";

    /// <summary>Mapa unico entre o dominio do teste e o DOM. Ids nao vazam para a camada de testes.</summary>
    private static readonly IReadOnlyDictionary<VehicleField, string> FieldIds =
        new Dictionary<VehicleField, string>
        {
            [VehicleField.CylinderCapacity] = "cylindercapacity",
            [VehicleField.EnginePerformance] = "engineperformance",
            [VehicleField.DateOfManufacture] = "dateofmanufacture",
            [VehicleField.Payload] = "payload",
            [VehicleField.TotalWeight] = "totalweight",
            [VehicleField.ListPrice] = "listprice",
            [VehicleField.LicensePlateNumber] = "licenseplatenumber",
            [VehicleField.AnnualMileage] = "annualmileage"
        };

    private ILocator Make => Page.Locator("#make");
    private ILocator Model => Page.Locator("#model");
    private ILocator NumberOfSeats => Page.Locator("#numberofseats");
    private ILocator NumberOfSeatsMotorcycle => Page.Locator("#numberofseatsmotorcycle");
    private ILocator RightHandDriveYes => Page.Locator("#righthanddriveyes");
    private ILocator RightHandDriveNo => Page.Locator("#righthanddriveno");

    /// <summary>
    /// Controle visual que o idealforms desenha por cima do radio. O &lt;input&gt; real e
    /// empurrado para fora da tela (style="position:absolute; left:-9999px"), entao
    /// clicar nele exige coordenadas negativas - origem de instabilidade. Este span
    /// fica dentro do &lt;label&gt; que embrulha o input, portanto clicar aqui marca o
    /// radio pela associacao nativa do HTML, sem force e sem coordenada invalida.
    /// </summary>
    private ILocator RightHandDriveToggle(bool rightHandDrive) =>
        Page.Locator($"label:has(#righthanddrive{(rightHandDrive ? "yes" : "no")}) span.ideal-radio");
    private ILocator FuelType => Page.Locator("#fuel");
    private ILocator NextButton => Page.Locator("#nextenterinsurantdata");

    private ILocator Field(VehicleField field) => Page.Locator($"#{FieldIds[field]}");

    /// <summary>Abre a aplicacao na primeira etapa e devolve o Page Object pronto para uso.</summary>
    public static async Task<VehicleDataPage> OpenAsync(IPage page)
    {
        await page.GotoAsync(Config.BaseUrl);

        var vehicleDataPage = new VehicleDataPage(page);
        await vehicleDataPage.WaitUntilReadyAsync();
        await vehicleDataPage.AssertIsCurrentStepAsync();

        return vehicleDataPage;
    }

    /// <summary>Preenche a etapa inteira a partir da massa de teste.</summary>
    public async Task FillAsync(VehicleData data)
    {
        await Make.SelectOptionAsync(data.Make);
        await Model.SelectOptionAsync(data.Model);
        await SetFieldAsync(VehicleField.CylinderCapacity, data.CylinderCapacity);
        await SetFieldAsync(VehicleField.EnginePerformance, data.EnginePerformance);
        await SetFieldAsync(VehicleField.DateOfManufacture, data.DateOfManufacture);
        await NumberOfSeats.SelectOptionAsync(data.NumberOfSeats);
        await NumberOfSeatsMotorcycle.SelectOptionAsync(data.NumberOfSeatsMotorcycle);
        await SetRightHandDriveAsync(data.RightHandDrive);
        await FuelType.SelectOptionAsync(data.FuelType);
        await SetFieldAsync(VehicleField.Payload, data.Payload);
        await SetFieldAsync(VehicleField.TotalWeight, data.TotalWeight);
        await SetFieldAsync(VehicleField.ListPrice, data.ListPrice);
        await SetFieldAsync(VehicleField.LicensePlateNumber, data.LicensePlateNumber);
        await SetFieldAsync(VehicleField.AnnualMileage, data.AnnualMileage);
    }

    /// <summary>Escreve em um unico campo, disparando a validacao da aplicacao.</summary>
    public Task SetFieldAsync(VehicleField field, string value) =>
        SetTextAsync(Field(field), value);

    /// <summary>Assertiva de campo reprovado na validacao, com a mensagem esperada.</summary>
    public Task AssertFieldIsInvalidAsync(VehicleField field, string expectedMessage) =>
        AssertFieldIsInvalidAsync(FieldIds[field], expectedMessage);

    /// <summary>Assertiva de campo aprovado na validacao.</summary>
    public Task AssertFieldIsValidAsync(VehicleField field) =>
        AssertFieldIsValidAsync(FieldIds[field]);

    /// <summary>
    /// Seleciona "Right Hand Drive" clicando no controle visivel e confirma o estado
    /// com uma assertiva que faz retry, em vez de confiar na leitura imediata do
    /// CheckAsync (que falha de forma intermitente com o input fora da tela).
    /// </summary>
    public async Task SetRightHandDriveAsync(bool rightHandDrive)
    {
        await RightHandDriveToggle(rightHandDrive).ClickAsync();

        await Assertions.Expect(rightHandDrive ? RightHandDriveYes : RightHandDriveNo)
            .ToBeCheckedAsync();
    }

    /// <summary>
    /// Avanca para a etapa seguinte e devolve o Page Object dela.
    /// O encadeamento deixa o fluxo explicito no teste e impede que um teste
    /// interaja com uma etapa que ainda nao esta na tela.
    /// </summary>
    public async Task<InsurantDataPage> ClickNextAsync()
    {
        await NextButton.ClickAsync();

        var insurantDataPage = new InsurantDataPage(Page);
        await insurantDataPage.AssertIsCurrentStepAsync();

        return insurantDataPage;
    }

    /// <summary>Preenche a etapa e avanca, no fluxo feliz.</summary>
    public async Task<InsurantDataPage> FillAndContinueAsync(VehicleData data)
    {
        await FillAsync(data);
        return await ClickNextAsync();
    }

    /// <summary>Confere que os valores realmente chegaram ao formulario.</summary>
    public async Task AssertFormMatchesAsync(VehicleData data)
    {
        await Assertions.Expect(Make).ToHaveValueAsync(data.Make);
        await Assertions.Expect(Model).ToHaveValueAsync(data.Model);
        await Assertions.Expect(NumberOfSeats).ToHaveValueAsync(data.NumberOfSeats);
        await Assertions.Expect(FuelType).ToHaveValueAsync(data.FuelType);

        await Assertions.Expect(Field(VehicleField.CylinderCapacity)).ToHaveValueAsync(data.CylinderCapacity);
        await Assertions.Expect(Field(VehicleField.EnginePerformance)).ToHaveValueAsync(data.EnginePerformance);
        await Assertions.Expect(Field(VehicleField.DateOfManufacture)).ToHaveValueAsync(data.DateOfManufacture);
        await Assertions.Expect(Field(VehicleField.Payload)).ToHaveValueAsync(data.Payload);
        await Assertions.Expect(Field(VehicleField.TotalWeight)).ToHaveValueAsync(data.TotalWeight);
        await Assertions.Expect(Field(VehicleField.ListPrice)).ToHaveValueAsync(data.ListPrice);
        await Assertions.Expect(Field(VehicleField.LicensePlateNumber)).ToHaveValueAsync(data.LicensePlateNumber);
        await Assertions.Expect(Field(VehicleField.AnnualMileage)).ToHaveValueAsync(data.AnnualMileage);

        await Assertions.Expect(data.RightHandDrive ? RightHandDriveYes : RightHandDriveNo)
            .ToBeCheckedAsync();
    }
}

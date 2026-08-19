namespace PlaywrightTest.Models;

/// <summary>
/// Massa de teste da etapa "Enter Vehicle Data".
/// Os valores padrao respeitam as regras de validacao da aplicacao
/// (forms/customization/configuration.js): ranges, datas passadas e tamanhos maximos.
/// </summary>
public sealed record VehicleData
{
    public string Make { get; init; } = "Audi";
    public string Model { get; init; } = "Scooter";

    /// <summary>Regra da aplicacao: required range:1:2000.</summary>
    public string CylinderCapacity { get; init; } = "1500";

    /// <summary>Regra da aplicacao: required range:1:2000.</summary>
    public string EnginePerformance { get; init; } = "120";

    /// <summary>Regra da aplicacao: required date pastdate (formato MM/DD/YYYY).</summary>
    public string DateOfManufacture { get; init; } = "01/01/2020";

    public string NumberOfSeats { get; init; } = "5";

    /// <summary>
    /// A aplicacao exibe um segundo campo "Number of Seats" (motocicleta) quando o
    /// tipo de veiculo nao foi escolhido na home. Ele e obrigatorio: sem preencher,
    /// a etapa nunca fica totalmente valida.
    /// </summary>
    public string NumberOfSeatsMotorcycle { get; init; } = "2";
    public bool RightHandDrive { get; init; } = true;
    public string FuelType { get; init; } = "Diesel";

    /// <summary>Regra da aplicacao: required range:1:1000.</summary>
    public string Payload { get; init; } = "1000";

    /// <summary>Regra da aplicacao: required range:100:50000.</summary>
    public string TotalWeight { get; init; } = "2000";

    /// <summary>Regra da aplicacao: required range:500:100000.</summary>
    public string ListPrice { get; init; } = "30000";

    /// <summary>Regra da aplicacao: max:10.</summary>
    public string LicensePlateNumber { get; init; } = "ABC1234";

    /// <summary>Regra da aplicacao: required range:100:100000.</summary>
    public string AnnualMileage { get; init; } = "15000";

    /// <summary>Cenario feliz: todos os campos dentro das regras de validacao.</summary>
    public static VehicleData Valid() => new();
}

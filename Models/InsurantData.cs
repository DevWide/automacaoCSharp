namespace PlaywrightTest.Models;

/// <summary>
/// Massa de teste da etapa "Enter Insurant Data".
/// </summary>
public sealed record InsurantData
{
    public string FirstName { get; init; } = "John";
    public string LastName { get; init; } = "Doe";

    /// <summary>Regra da aplicacao: required date dateinterval (formato MM/DD/YYYY).</summary>
    public string DateOfBirth { get; init; } = "01/01/1985";

    /// <summary>Regra da aplicacao: min:3.</summary>
    public string StreetAddress { get; init; } = "123 Main St";

    public string Country { get; init; } = "Brazil";

    /// <summary>Cenario feliz: todos os campos dentro das regras de validacao.</summary>
    public static InsurantData Valid() => new();
}

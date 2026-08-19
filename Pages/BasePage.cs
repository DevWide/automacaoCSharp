using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace PlaywrightTest.Pages;

/// <summary>
/// Base dos Page Objects do formulario de cotacao.
///
/// A aplicacao e um wizard construido com o plugin jQuery "idealforms". Quatro
/// comportamentos dela definem o desenho desta classe:
///
///  1. Cada etapa e uma &lt;section class="idealsteps-step"&gt;, e o &lt;li&gt; da etapa
///     corrente recebe a classe "idealsteps-step-active".
///  2. A aplicacao replica o rotulo da etapa ativa no title do documento.
///  3. Cada campo vive em uma &lt;div class="field"&gt; que ganha a classe "valid" ou
///     "invalid" e carrega um &lt;span class="error"&gt; com a mensagem.
///  4. A aba de cada etapa exibe um contador de campos invalidos.
///
/// Sao esses sinais que permitem assertivas sobre o estado real do formulario,
/// em vez de apenas checar se um elemento existe na pagina.
/// </summary>
public abstract class BasePage
{
    private static readonly Regex InvalidClass = new(@"(^|\s)invalid(\s|$)");

    protected readonly IPage Page;

    protected BasePage(IPage page) => Page = page;

    /// <summary>Id da ancora da etapa na navegacao do wizard (atribuido por makeapp.js).</summary>
    protected abstract string StepAnchorId { get; }

    /// <summary>Rotulo da etapa, que a aplicacao replica no title do documento.</summary>
    protected abstract string StepTitle { get; }

    private ILocator StepTab => Page.Locator($"#idealsteps-nav li:has(#{StepAnchorId})");

    private ILocator ActiveStepTab =>
        Page.Locator($"#idealsteps-nav li.idealsteps-step-active:has(#{StepAnchorId})");

    /// <summary>Contador de campos invalidos que a aplicacao mantem na aba desta etapa.</summary>
    private ILocator InvalidFieldCounter => Page.Locator($"#{StepAnchorId} span");

    /// <summary>Wrapper &lt;div class="field"&gt; que envolve rotulo, campo e mensagem de erro.</summary>
    private ILocator FieldContainer(string fieldId) => Page.Locator($"div.field:has(#{fieldId})");

    /// <summary>
    /// Escreve em um campo de texto e garante que a aplicacao revalide o valor.
    ///
    /// A validacao do idealforms esta ligada aos eventos "change keyup". FillAsync
    /// define o valor sem produzir keyup, entao o ultimo campo preenchido continuava
    /// marcado como pendente ("This field is mandatory") mesmo estando preenchido.
    /// A tecla End nao altera o conteudo e dispara exatamente o keyup que faltava.
    /// </summary>
    protected static async Task SetTextAsync(ILocator locator, string value)
    {
        await locator.FillAsync(value);
        await locator.PressAsync("End");
    }

    /// <summary>Navega ate esta etapa clicando na aba correspondente.</summary>
    public async Task OpenStepAsync()
    {
        await StepTab.ClickAsync();
        await AssertIsCurrentStepAsync();
    }

    /// <summary>
    /// Assertiva de que o wizard esta de fato nesta etapa.
    /// Verifica a classe de etapa ativa e o title do documento - dois sinais
    /// independentes, ambos com retry automatico do Playwright.
    /// </summary>
    public async Task AssertIsCurrentStepAsync()
    {
        await Assertions.Expect(ActiveStepTab).ToBeVisibleAsync();

        // Casar o title por expressao regular evita depender dos espacos em branco
        // que o plugin gera ao montar o rotulo em runtime.
        await Assertions.Expect(Page).ToHaveTitleAsync(new Regex(Regex.Escape(StepTitle)));
    }

    /// <summary>
    /// Assertiva de que a propria aplicacao considera a etapa inteira preenchida:
    /// o contador de campos invalidos da aba esta zerado.
    /// </summary>
    public Task AssertStepHasNoValidationErrorsAsync() =>
        Assertions.Expect(InvalidFieldCounter).ToHaveTextAsync("0");

    /// <summary>
    /// Assertiva de campo reprovado na validacao.
    ///
    /// Verifica a classe "invalid" no wrapper, e nao apenas a mensagem: o
    /// &lt;span class="error"&gt; preserva o texto da ultima validacao mesmo depois que o
    /// campo volta a ser valido - apenas deixa de ser exibido. Assertar somente o
    /// texto produziria falso positivo.
    ///
    /// Protegido de proposito: a camada de testes referencia campos por enum tipado,
    /// nunca por id cru.
    /// </summary>
    protected async Task AssertFieldIsInvalidAsync(string fieldId, string expectedMessage)
    {
        await Assertions.Expect(FieldContainer(fieldId)).ToHaveClassAsync(InvalidClass);

        var error = FieldContainer(fieldId).Locator("span.error");

        await Assertions.Expect(error).ToBeVisibleAsync();
        await Assertions.Expect(error).ToHaveTextAsync(expectedMessage);
    }

    /// <summary>Assertiva oposta: o campo passou na validacao e nao exibe mensagem.</summary>
    protected async Task AssertFieldIsValidAsync(string fieldId)
    {
        await Assertions.Expect(FieldContainer(fieldId)).Not.ToHaveClassAsync(InvalidClass);
        await Assertions.Expect(FieldContainer(fieldId).Locator("span.error")).Not.ToBeVisibleAsync();
    }

    /// <summary>
    /// Espera a etapa estar pronta para interacao. Os ids da navegacao sao atribuidos
    /// por JavaScript no evento window.load, entao esperar pela ancora garante que os
    /// scripts da pagina ja executaram.
    /// </summary>
    protected Task WaitUntilReadyAsync() =>
        Page.Locator($"#{StepAnchorId}").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Attached
        });
}

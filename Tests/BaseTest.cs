using Microsoft.Playwright;
using Xunit;

namespace PlaywrightTest.Tests;

/// <summary>
/// Ciclo de vida do Playwright para cada teste.
///
/// Cada teste recebe um <see cref="IBrowserContext"/> novo - sessao, cookies e
/// armazenamento isolados -, o que permite rodar as classes em paralelo sem
/// interferencia entre elas.
///
/// Todo teste e gravado em um trace do Playwright (DOM, rede, console e screenshots
/// a cada acao). O trace e o artefato que transforma uma falha vermelha na pipeline
/// em diagnostico: basta abrir o arquivo em https://trace.playwright.dev.
/// </summary>
public abstract class BaseTest : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;

    protected IPage Page { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Directory.CreateDirectory(Config.TraceDirectory);
        Directory.CreateDirectory(Config.ScreenshotDirectory);

        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        _browser = await _playwright[Config.BrowserName].LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Config.Headless,
            SlowMo = Config.SlowMo
        });

        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            RecordVideoDir = Config.RecordVideo ? Config.VideoDirectory : null
        });

        _context.SetDefaultTimeout(Config.DefaultTimeout);

        await _context.Tracing.StartAsync(new TracingStartOptions
        {
            Title = GetType().Name,
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });

        Page = await _context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        var tracePath = Path.Combine(
            Config.TraceDirectory,
            $"{GetType().Name}-{DateTime.UtcNow:yyyyMMdd-HHmmss-fff}.zip");

        // Guardas contra falha durante o InitializeAsync: se o navegador nao subiu,
        // o teardown nao pode mascarar o erro original com um NullReference.
        if (_context is not null)
        {
            await _context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
            await _context.CloseAsync();
        }

        if (_browser is not null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();
    }

    /// <summary>
    /// Screenshot de pagina inteira gravado junto dos demais artefatos.
    /// Diferente da versao anterior, roda tambem na pipeline: e justamente la que
    /// nao ha ninguem olhando para a tela.
    /// </summary>
    protected async Task<string> CaptureScreenshotAsync(string fileName)
    {
        var path = Path.Combine(Config.ScreenshotDirectory, fileName);

        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = path,
            FullPage = true
        });

        return path;
    }
}

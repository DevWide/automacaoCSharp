using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace PlaywrightTest.Tests
{
    public class BaseTest : IAsyncLifetime
    {
        protected IPlaywright Playwright;
        protected IBrowser Browser;
        protected IBrowserContext Context;
        protected IPage Page;

        public async Task InitializeAsync()
        {
            // Inicializa o Playwright, o navegador e a página
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            Context = await Browser.NewContextAsync();
            Page = await Context.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            // Fecha o navegador e libera recursos
            if (Browser != null)
            {
                await Browser.CloseAsync();
                Playwright?.Dispose();
            }
        }

        /// <summary>
        /// Captura uma captura de tela da página e a salva no diretório local "Screenshots".
        /// Este método não executa capturas de tela na pipeline (se CI=true no ambiente).
        /// </summary>
        /// <param name="fileName">O nome do arquivo da captura de tela.</param>
        protected async Task CaptureScreenshot(string fileName)
        {
            // Verifica se estamos em um ambiente local (não na pipeline)
            if (Environment.GetEnvironmentVariable("CI") != "true")
            {
                // Obtém o caminho da raiz do projeto
                var projectRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
                var screenshotsDir = Path.Combine(projectRoot, "Screenshots");

                // Cria o diretório "Screenshots" se não existir
                if (!Directory.Exists(screenshotsDir))
                {
                    Directory.CreateDirectory(screenshotsDir);
                }

                // Salva ou atualiza a captura de tela no diretório correto
                var screenshotPath = Path.Combine(screenshotsDir, fileName);

                // Configura o navegador para capturar toda a página
                await Page.SetViewportSizeAsync(1920, 1080); // Redimensiona a janela para garantir a captura
                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
                Console.WriteLine($"Screenshot salvo ou atualizado em: {screenshotPath}");
            }
            else
            {
                Console.WriteLine("Captura de tela ignorada na pipeline (CI=true).");
            }
        }

    }
}


using System.IO;

namespace PlaywrightTest
{
    public static class Config
    {
        public static string BaseUrl { get; } = "http://sampleapp.tricentis.com/101/app.php";
        public static string ScreenshotPath { get; } = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
        public static bool CaptureScreenshots { get; } = true;
        public static bool Headless { get; } = false; // Configuração para testes visuais
        public static int DefaultTimeout { get; } = 30000; // Timeout padrão em milissegundos

        static Config()
        {
            if (!Directory.Exists(ScreenshotPath))
            {
                Directory.CreateDirectory(ScreenshotPath);
            }
        }
    }
}


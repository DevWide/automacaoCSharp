namespace PlaywrightTest;

/// <summary>
/// Configuracao da suite. Todo valor pode ser sobrescrito por variavel de ambiente,
/// o que permite apontar a mesma suite para outro ambiente (dev/staging) ou trocar de
/// navegador na pipeline sem recompilar o projeto.
/// </summary>
public static class Config
{
    public static bool IsCi { get; } = Read("CI") == "true";

    /// <summary>URL da aplicacao sob teste. Sobrescreva com BASE_URL.</summary>
    public static string BaseUrl { get; } =
        Read("BASE_URL") ?? "https://sampleapp.tricentis.com/101/app.php";

    /// <summary>chromium | firefox | webkit. Sobrescreva com BROWSER.</summary>
    public static string BrowserName { get; } = Read("BROWSER") ?? "chromium";

    /// <summary>Headless por padrao na pipeline, com janela visivel no ambiente local.</summary>
    public static bool Headless { get; } =
        (Read("HEADLESS") ?? (IsCi ? "true" : "false")) == "true";

    /// <summary>Atraso artificial entre acoes, util para depurar visualmente (SLOW_MO).</summary>
    public static float SlowMo { get; } = ReadFloat("SLOW_MO", 0);

    /// <summary>
    /// Gravacao de video, desligada por padrao: o trace ja carrega um screenshot por
    /// acao, e o video multiplica o tamanho do artefato da pipeline. Ligue com
    /// RECORD_VIDEO=true quando precisar ver a execucao em movimento.
    /// </summary>
    public static bool RecordVideo { get; } = Read("RECORD_VIDEO") == "true";

    /// <summary>Timeout padrao de acoes e assertivas, em milissegundos.</summary>
    public static float DefaultTimeout { get; } = ReadFloat("DEFAULT_TIMEOUT", 30_000);

    /// <summary>Raiz dos artefatos de execucao (traces, screenshots, videos).</summary>
    public static string ArtifactsDirectory { get; } =
        Read("ARTIFACTS_DIR") ?? Path.Combine(RepositoryRoot(), "TestResults");

    public static string TraceDirectory { get; } = Path.Combine(ArtifactsDirectory, "traces");
    public static string ScreenshotDirectory { get; } = Path.Combine(ArtifactsDirectory, "screenshots");
    public static string VideoDirectory { get; } = Path.Combine(ArtifactsDirectory, "videos");

    private static string? Read(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static float ReadFloat(string key, float fallback) =>
        float.TryParse(Read(key), out var parsed) ? parsed : fallback;

    /// <summary>
    /// Sobe a arvore de diretorios a partir do output do build ate achar o .sln.
    /// Evita o "../../.." fragil, que quebra a cada mudanca de target framework.
    /// </summary>
    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && directory.GetFiles("*.sln").Length == 0)
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? AppContext.BaseDirectory;
    }
}

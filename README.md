# PlaywrightTest - Automação de Testes para Aplicação de Seguro de Veículos

Este projeto implementa uma automação de testes usando **Microsoft Playwright** para validar diferentes etapas de uma aplicação de seguro de veículos. Ele foi projetado para rodar localmente e integrado com **GitHub Actions** para pipelines CI/CD.

---

## Objetivo
Garantir a qualidade da aplicação de seguro, validando as funcionalidades das abas:
- **Enter Vehicle Data**
- **Enter Insurant Data**

Os testes incluem validações de preenchimento de campos obrigatórios, seleção de opções e transição entre as etapas do formulário.

---

## Estrutura do Projeto
Abaixo está a estrutura principal do projeto:

```plaintext
.
├── .github/workflows          # Configuração da pipeline do GitHub Actions
│   └── playwright_tests.yml   # Pipeline de execução de testes
├── Pages                      # Page Objects (abstração dos elementos das páginas)
│   ├── insurantDataPage.cs    # Classe para manipular a aba 'Enter Insurant Data'
│   ├── vehicleDataPage.cs     # Classe para manipular a aba 'Enter Vehicle Data'
├── Tests                      # Testes unitários e funcionais
│   ├── insurantDataTests.cs   # Testes para 'Enter Insurant Data'
│   ├── vehicleDataTests.cs    # Testes para 'Enter Vehicle Data'
│   ├── BaseTest.cs            # Classe base para configuração de Playwright
├── Screenshots/               # Capturas de tela geradas localmente
├── Config.cs                  # Configuração de variáveis do projeto
├── selectors.json             # Seletor de elementos das páginas
├── PlaywrightTest.csproj      # Arquivo de configuração do projeto .NET
└── .gitignore                 # Arquivo para ignorar arquivos e pastas não versionadas
```

## Pré-requisitos
Certifique-se de ter as ferramentas abaixo instaladas no seu ambiente:

- [.NET SDK 6.0+](https://dotnet.microsoft.com/download) (recomendado: .NET 9.0)
- [Node.js](https://nodejs.org/) (necessário para Playwright)
- [Git](https://git-scm.com/)
- Uma IDE como [Visual Studio](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)


## Passos para Clonar e Rodar o Projeto 
### 1. Clone o Repositório
```
git clone <URL_DO_REPOSITORIO>
cd PlaywrightTest
```

### 2. Restaure as Dependências do .NET
```
dotnet restore
```

### 3. Instale as Dependências do Playwright
```
npx playwright install
```

### 4. Configure o Ambiente Local
* Verifique se o arquivo **Config.cs** contém a URL correta da aplicação (exemplo: https://sampleapp.tricentis.com/101/app.php).

* Certifique-se de que a pasta **Screenshots/** está sendo ignorada pelo Git usando o arquivo **.gitignore**.

## Rodando os Testes Localmente
1. Testar Apenas a Aba "Enter Vehicle Data"
```
dotnet test --filter "Category=VehicleData"
```

2. Testar Apenas a Aba "Enter Insurant Data"
```
dotnet test --filter "Category=InsurantData"
```

3. Rodar Todos os Testes
```
dotnet test
```

## Captura de Tela
Durante a execução dos testes, capturas de tela serão geradas automaticamente na pasta **Screenshots/** ao final de cada teste.

### **Configuração:**
* Para habilitar ou desabilitar capturas de tela, ajuste a propriedade **CaptureScreenshots** no arquivo **Config.cs**:

```
public static bool CaptureScreenshots = true;
```

## Integração com GitHub Actions
O projeto já inclui uma pipeline configurada no arquivo:
**.github/workflows/playwright_tests.yml**:

* Passos da pipeline:
    1. Configuração do ambiente (instalação do .NET e Playwright)
    2. Restauração e build do projeto.
    3. Execução de todos os testes.

### Executar Localmente:
Para simular a pipeline localmente, execute o comando:
```
dotnet test
```

## Licença
Este projeto é protegido por uma licença MIT.


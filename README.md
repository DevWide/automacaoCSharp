# Test Automation — Playwright + C# (.NET 10)

[![Playwright Tests](https://github.com/DevWide/automacaoCSharp/actions/workflows/playwright_tests.yml/badge.svg)](https://github.com/DevWide/automacaoCSharp/actions/workflows/playwright_tests.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Playwright](https://img.shields.io/badge/Playwright-1.49-2EAD33?logo=playwright&logoColor=white)](https://playwright.dev/dotnet/)
[![xUnit](https://img.shields.io/badge/xUnit-2.9-512BD4)](https://xunit.net/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](#licenca)

Suíte de testes end-to-end sobre o [Tricentis Sample Insurance App](https://sampleapp.tricentis.com/101/app.php),
um formulário multi-etapas de cotação de seguro. O projeto é a minha referência prática de
como estruturo automação de UI em .NET: **Page Objects tipados, massa de teste separada do
código, assertivas com retry e uma pipeline que entrega diagnóstico — não só um X vermelho.**

---

## O que este projeto demonstra

| Competência | Onde está no código |
| --- | --- |
| Page Object Model com localizadores tipados | [Pages/](Pages/) — `ILocator` como propriedade, sem string solta no teste |
| Herança e reuso entre páginas | [Pages/BasePage.cs](Pages/BasePage.cs) — navegação e assertivas do wizard em um só lugar |
| Fluent POM (encadeamento de páginas) | `VehicleDataPage.ClickNextAsync()` devolve `InsurantDataPage` já validada |
| Testes dirigidos por dados | `[Theory]` + `[InlineData]` nos cenários negativos |
| Massa de teste como modelo de domínio | [Models/](Models/) — `record` imutável, derivável com `with` |
| Assertivas web-first (com retry) | `Assertions.Expect(...)` — zero `Thread.Sleep`, zero `WaitForTimeout` |
| Cenários negativos e regras de negócio | Ranges, tamanho mínimo e formato, extraídos das regras da própria aplicação |
| Isolamento e paralelismo | Um `IBrowserContext` novo por teste + [xunit.runner.json](xunit.runner.json) |
| Configuração por ambiente | [Config.cs](Config.cs) — tudo sobrescrevível por variável de ambiente |
| CI/CD multi-navegador | [.github/workflows/playwright_tests.yml](.github/workflows/playwright_tests.yml) |
| Diagnóstico de falha | Trace, vídeo e screenshot publicados como artefato em toda execução |

---

## Arquitetura

```plaintext
.
├── .github/workflows/
│   └── playwright_tests.yml     # Pipeline: build → testes (3 navegadores) → relatório
├── Models/                      # Massa de teste (records) — o "o quê"
│   ├── VehicleData.cs
│   └── InsurantData.cs
├── Pages/                       # Page Objects — o "como" interagir
│   ├── BasePage.cs              # Navegação do wizard + assertiva de etapa ativa
│   ├── VehicleDataPage.cs
│   └── InsurantDataPage.cs
├── Tests/                       # Cenários — o "por quê"
│   ├── BaseTest.cs              # Ciclo de vida do Playwright + trace + screenshot
│   ├── VehicleDataTests.cs
│   └── InsurantDataTests.cs
├── Config.cs                    # Configuração por variável de ambiente
├── xunit.runner.json            # Paralelismo do runner
└── PlaywrightTest.csproj
```

O fluxo de dependência é sempre **Tests → Pages → Models**. Um teste nunca vê um seletor:
todos os ids vivem dentro da camada `Pages`, mapeados por um `enum` tipado. Se um id mudar
na aplicação, existe exatamente um lugar para corrigir — e o compilador acusa se o nome do
campo estiver errado.

### Decisões de projeto

**Por que seletores por `id` e não `GetByRole`/`GetByLabel`?**
A aplicação sob teste não associa `<label>` aos campos (sem atributo `for`, sem `aria-label`),
então os campos não têm nome acessível — localizadores semânticos simplesmente não resolvem
aqui. O `id` é o contrato mais estável disponível. Onde a aplicação oferecer semântica, a
preferência é sempre o localizador semântico.

**Por que a assertiva de navegação verifica duas coisas?**
A aplicação marca a etapa corrente com a classe `idealsteps-step-active` **e** replica o nome
da etapa no `title` do documento. `AssertIsCurrentStepAsync()` checa os dois — dois sinais
independentes, ambos com retry automático. Verificar apenas se um elemento "existe" produziria
falso positivo, porque as abas do wizard estão sempre no DOM.

**Por que escrever com `Fill` + `Press("End")` em vez de só `Fill`?**
A validação do formulário está ligada aos eventos `change keyup`. `FillAsync` define o valor
sem produzir `keyup`, então o último campo preenchido continuava marcado como pendente
(*"This field is mandatory"*) mesmo estando preenchido — e a aba exibia contador de erros
diferente de zero. A tecla `End` não altera o conteúdo e dispara exatamente o `keyup` que
faltava. Sem isso, os testes de validação eram falso-negativos.

**Por que a assertiva de campo inválido olha a classe CSS, e não a mensagem?**
O `<span class="error">` preserva o texto da última validação mesmo depois de o campo voltar
a ser válido — apenas deixa de ser exibido. Assertar só o texto produziria falso positivo.
A classe `invalid` no wrapper é o único sinal que reflete o estado atual.

**Por que clicar no `span.ideal-radio` em vez do `<input type="radio">`?**
O plugin empurra o input real para fora da tela (`left: -9999px`) e desenha um controle por
cima. Clicar no input exigia coordenadas negativas, o que causava falha intermitente
(*"Clicking the checkbox did not change its state"*) em cerca de metade das execuções da
suíte. Clicar no controle visível marca o radio pela associação nativa do HTML — sem `force`
e sem instabilidade.

---

## Pré-requisitos

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) (LTS)
- [PowerShell 7+](https://learn.microsoft.com/powershell/scripting/install/installing-powershell)
  (`pwsh`) — usado pelo script de instalação de navegadores do Playwright
- [Git](https://git-scm.com/)

> Não é necessário Node.js. O Playwright para .NET instala os navegadores pelo próprio script
> gerado no build.


## Executando localmente

```bash
git clone https://github.com/DevWide/automacaoCSharp.git
cd automacaoCSharp

dotnet restore
dotnet build

# Instala os navegadores (uma única vez por máquina)
pwsh bin/Debug/net10.0/playwright.ps1 install

dotnet test
```

### Recortes da suíte

```bash
dotnet test --filter "Category=VehicleData"     # apenas a etapa de veículo
dotnet test --filter "Category=InsurantData"    # apenas a etapa de segurado
dotnet test --filter "FullyQualifiedName~ShowsValidationError"   # apenas cenários negativos
```

### Configuração por variável de ambiente

Nenhum valor de execução está fixo no código — a mesma suíte aponta para outro ambiente ou
outro navegador sem recompilar.

| Variável | Padrão | Para que serve |
| --- | --- | --- |
| `BASE_URL` | `https://sampleapp.tricentis.com/101/app.php` | Ambiente sob teste |
| `BROWSER` | `chromium` | `chromium`, `firefox` ou `webkit` |
| `HEADLESS` | `false` local / `true` na CI | Executar com ou sem janela |
| `SLOW_MO` | `0` | Atraso entre ações, em ms, para depurar visualmente |
| `DEFAULT_TIMEOUT` | `30000` | Timeout de ações e assertivas, em ms |
| `RECORD_VIDEO` | `false` | Grava vídeo da execução (o trace já traz screenshot por ação) |
| `ARTIFACTS_DIR` | `./TestResults` | Destino de traces, vídeos e screenshots |

```bash
# Depuração visual: navegador aberto e em câmera lenta
HEADLESS=false SLOW_MO=500 dotnet test --filter "Category=VehicleData"

# Mesma suíte, outro navegador
BROWSER=webkit dotnet test
```

---

## Cobertura atual

**19 testes**, executados nos três motores de navegador:

| Suíte | Cenários |
| --- | --- |
| `VehicleDataTests` | Preenchimento completo, avanço de etapa, 4 casos de valor fora da regra, 4 contraprovas com valor válido |
| `InsurantDataTests` | Navegação pela aba, preenchimento completo, 4 casos de valor fora da regra, 3 contraprovas |

Cada cenário negativo tem uma contraprova com valor válido. Sem ela, uma assertiva que sempre
passa (ou sempre falha) não seria percebida.

---

## Diagnóstico de falhas

Toda execução grava um **trace do Playwright** em `TestResults/traces/` — DOM, requisições de
rede, console e um screenshot por ação.

```bash
pwsh bin/Debug/net10.0/playwright.ps1 show-trace TestResults/traces/VehicleDataTests-*.zip
```

Ou arraste o `.zip` para <https://trace.playwright.dev> — nada para instalar.

Vídeo é opcional (`RECORD_VIDEO=true`): o trace já carrega um screenshot por ação, e gravar
vídeo de toda a suíte multiplicava o tamanho do artefato sem agregar diagnóstico.

Na pipeline, os traces (junto com screenshots e o `.trx`) são publicados como artefato
**em toda execução, inclusive nas que falham**. Investigar uma falha da CI não exige reproduzir
nada na máquina local: baixa-se o trace e navega-se pelo teste passo a passo.

---

## Pipeline (GitHub Actions)

```
build ──► test (chromium) ──┐
      ├─► test (firefox)  ──┼──► summary
      └─► test (webkit)   ──┘
```

| Etapa | O que faz |
| --- | --- |
| `build` | Portão rápido de compilação e resolução da matriz de navegadores |
| `test` | Matriz de 3 navegadores em paralelo: build, instalação do navegador e testes |
| `summary` | Resumo consolidado no sumário do job |

Pontos de atenção resolvidos na pipeline:

- **Cache de pacotes NuGet** e **cache dos binários dos navegadores**, com chave derivada da
  versão do Playwright — invalida sozinho quando o pacote é atualizado.
- **Cada job de teste compila o próprio código** em vez de baixar o output do job `build`.
  O output carrega o driver Node do Playwright e passa de 130 MB; transferir isso para cada
  navegador da matriz custaria muito mais do que recompilar, que leva cerca de um segundo com
  o cache do NuGet quente.
- **A matriz é resolvida em bash**, não numa expressão `fromJSON(format(...))`. A expressão
  dependeria de avaliação preguiçosa de `&&`/`||` e quebraria em `push`, onde `inputs.browser`
  não existe.
- **`fail-fast: false`**: uma falha no WebKit não esconde o resultado do Chromium.
- **`concurrency`**: um push novo cancela a execução anterior da mesma branch.
- **Execução noturna agendada**, para separar "quebrou porque mudei o teste" de
  "quebrou porque a aplicação mudou".
- **`workflow_dispatch`** com escolha de navegador, para execução sob demanda.
- **Relatório de testes no próprio Pull Request**, via `dorny/test-reporter`.

---

## Roadmap

- [ ] Cobrir as etapas restantes do wizard (Product Data, Price Option, Send Quote)
- [ ] Camada de testes de API sobre o mesmo domínio
- [ ] Geração de massa com [Bogus](https://github.com/bchavez/Bogus)
- [ ] Relatório HTML publicado no GitHub Pages
- [ ] Testes de acessibilidade com `Deque.AxeCore.Playwright`

---

## Licença

Distribuído sob a licença MIT.

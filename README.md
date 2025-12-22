# PlaywrightTest – Test Automation for Vehicle Insurance Application

This project implements test automation using Microsoft Playwright to validate different stages of a vehicle insurance application. It is designed to run locally and is integrated with GitHub Actions for CI/CD pipelines.

---

## Objective
Ensure the quality of the insurance application by validating the functionalities of the following tabs:
- **Enter Vehicle Data**
- **Enter Insurant Data**

The tests include validations for required field completion, option selection, and navigation between form steps.

---

## Project Structure
Below is the main structure of the project:

```plaintext
.
├── .github/workflows          # GitHub Actions pipeline configuration
│   └── playwright_tests.yml   # Test execution pipeline
├── Pages                      # Page Objects (page element abstraction)
│   ├── insurantDataPage.cs    # Class to handle the 'Enter Insurant Data' tab
│   ├── vehicleDataPage.cs     # Class to handle the 'Enter Vehicle Data' tab
├── Tests                      # Unit and functional tests
│   ├── insurantDataTests.cs   # Tests for 'Enter Insurant Data'
│   ├── vehicleDataTests.cs    # Tests for 'Enter Vehicle Data'
│   ├── BaseTest.cs            # Base class for Playwright setup
├── Screenshots/               # Locally generated screenshots
├── Config.cs                  # Project configuration variables
├── selectors.json             # Page element selectors
├── PlaywrightTest.csproj      # .NET project configuration file
└── .gitignore                 # File to ignore untracked files and folders
```

## Pre-requisites

Make sure you have the following tools installed in your environment:

- [.NET SDK 6.0+](https://dotnet.microsoft.com/download) (recommended: .NET 9.0)
- [Node.js](https://nodejs.org/) (required for Playwright)
- [Git](https://git-scm.com/)
- Uma IDE como [Visual Studio](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)


## Steps to Clone and Run the Project
### 1. Clone the Repository
```
git clone <URL_DO_REPOSITORIO>
cd PlaywrightTest
```

### 2. Restore .NET Dependencies
```
dotnet restore
```

### 3. Install Playwright Dependencies
```
npx playwright install
```

### 4. Configure the Local Environment
* Verify that the **Config.cs** file contains the correct application URL (exemplo: https://sampleapp.tricentis.com/101/app.php).

* Ensure that the **Screenshots/** folder is ignored by Git using the **.gitignore** file.

## Running Tests Locally
1. Run Only the "Enter Vehicle Data" Tests
```
dotnet test --filter "Category=VehicleData"
```

2. Run Only the "Enter Insurant Data" Tests
```
dotnet test --filter "Category=InsurantData"
```

3. Run All Tests
```
dotnet test
```

## Screenshots
During test execution, screenshots are automatically generated in the **Screenshots/** folder at the end of each test.

### **Configuration**
* To enable or disable screenshots, adjust the **CaptureScreenshots** in the **Config.cs** file:

```
public static bool CaptureScreenshots = true;
```

## GitHub Actions Integration
The project already includes a pipeline configured in:
**.github/workflows/playwright_tests.yml**:

* Pipeline Steps:
    1. Environment setup (installing .NET and Playwright)
    2. Project restore and build.
    3. Execution of all tests.

### Run Locally:
To simulate the pipeline locally, run:
```
dotnet test
```

## License
This project is licensed under the MIT License.


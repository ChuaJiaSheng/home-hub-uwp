# Home Hub Desktop App

Two C#/XAML UWP coursework applications that bring clock, alarms, weather, sketching, and chat into a desktop dashboard. The advanced application expands the page set with news and translation.

The prototype and advanced version are separate applications with independent solutions.

## Versions and requirements

| Version | Solution | Windows SDK target | Minimum target |
| --- | --- | --- | --- |
| Prototype | [Project1_222257U.sln](prototype/Project1_222257U.sln) | 10.0.26100.0 | 10.0.17763.0 |
| Advanced | [Project2_222257U.sln](advanced/Project2_222257U.sln) | 10.0.22621.0 | 10.0.17763.0 |

Use Windows, Visual Studio with UWP development tools, and the SDK required by the solution you open. These are UWP projects, not WinUI 3 or cross-platform desktop applications.

## Features and source areas

| Area | Relevant source |
| --- | --- |
| Navigation and dashboard | `MainPage`, `HomePage` |
| Drawing surface | `SketchingPage` |
| Chat interface | `AIChat` |
| Preferences | `SettingsPage` |
| Advanced translation | `TranslatorPage` |
| Advanced news | `NewsPage` |
| Advanced shared settings/theme | `GlobalSettings`, `ThemeManager`, `Alarm` |

Each page's XAML defines its UI; the matching `.xaml.cs` file contains its event handlers and service logic. Weather, news, translation, and chat integrations require their corresponding service configuration.

## Build the prototype

1. Open the prototype solution in Visual Studio.
2. Restore its NuGet dependencies and select an installed local target compatible with the project.
3. Build and launch locally. Review the pages and controls before relying on any connected-service behaviour.

## Configure and build the advanced version

From the repository root in PowerShell:

```powershell
Copy-Item advanced/Project2_222257U/ApiCredentials.example.cs advanced/Project2_222257U/ApiCredentials.local.cs
```

Fill in your own values in the ignored local file:

| Constant | Service used by the original source |
| --- | --- |
| `OpenWeather` | Weather |
| `BingNews` | News |
| `AzureTranslator` | Translation |
| `TogetherAi` | Chat |

Then open the advanced solution, restore dependencies, and build. The `.csproj` explicitly references `ApiCredentials.local.cs`; the example file is a template and does not satisfy that compiled-source reference by itself.

## Troubleshooting and limits

- Missing `ApiCredentials.local.cs`: copy the example before building the advanced app.
- SDK/targeting error: check the version-specific SDK target above rather than treating both solutions as interchangeable.
- Service error or empty response: inspect that page's endpoint, expected response shape, account configuration, and credentials. Check whether the configured provider still supports the endpoint.
- Keep the local credentials file outside Git. Compiled client applications do not provide a secure place to hide reusable service secrets.

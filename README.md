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

## Reproduce on Windows

**Prerequisites:** Git, Windows with UWP local development/deployment enabled, Visual Studio with UWP tools, and the matching Windows SDK from the version table. The two solutions are built separately.

```powershell
git clone https://github.com/ChuaJiaSheng/home-hub-uwp.git
cd home-hub-uwp
Start-Process .\\prototype\\Project1_222257U.sln
```

In Visual Studio, restore NuGet packages, choose a Debug target supported by the project and your machine, select **Local Machine**, then build and press **F5**. Install the requested SDK if Visual Studio reports it missing.

**Local UI check:** navigate to the dashboard and sketching page and try drawing. These are separate checks from network-backed weather/chat. For the advanced version, close the prototype solution and follow the configuration below before opening `advanced/Project2_222257U.sln`.

## Configure and build the advanced version

From the cloned repository root in PowerShell:

```powershell
Copy-Item .\\advanced\\Project2_222257U\\ApiCredentials.example.cs .\\advanced\\Project2_222257U\\ApiCredentials.local.cs
Start-Process .\\advanced\\Project2_222257U.sln
```

Edit the new local file and replace the placeholders with credentials from your own service accounts:

| Constant | Required service |
| --- | --- |
| `OpenWeather` | Weather API access |
| `BingNews` | Access to the news endpoint used in `NewsPage.xaml.cs` |
| `AzureTranslator` | Translator resource key; align any region/endpoint in `TranslatorPage.xaml.cs` with your resource |
| `TogetherAi` | Chat service access matching the model/endpoint in `AIChat.xaml.cs` |

Restore packages, build, and launch with F5. The project explicitly compiles `ApiCredentials.local.cs`; leaving only the example file causes a missing-source error.

**Expected results:** the advanced dashboard opens; sketching responds to drawing input. With valid credentials and supported endpoints, weather/news should return content, translation should return translated text, and chat should return a response. If only a service page fails, inspect that page's HTTP response and provider configuration rather than assuming the whole app failed to install. Historical provider endpoints may require updating; placeholder keys cannot reproduce live-service features.

## Troubleshooting and limits

- Missing `ApiCredentials.local.cs`: copy the example before building the advanced app.
- SDK/targeting error: check the version-specific SDK target above rather than treating both solutions as interchangeable.
- Service error or empty response: inspect that page's endpoint, expected response shape, account configuration, and credentials. Check whether the configured provider still supports the endpoint.
- Keep the local credentials file outside Git. Compiled client applications do not provide a secure place to hide reusable service secrets.

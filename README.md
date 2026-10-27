# Home Hub Desktop App

Two coursework versions of a desktop dashboard with clock, alarms, weather, sketching and chat. The advanced version adds news and translation.

**Technology:** C#, XAML and UWP.

## Run or inspect

Two versions are kept as independent solution trees:

| Directory | Entry point | Purpose |
| --- | --- | --- |
| `prototype` | `prototype/Project1_222257U.sln` | Initial UI and interaction coursework |
| `advanced` | `advanced/Project2_222257U.sln` | Expanded features and service integrations |

Use Windows and Visual Studio with UWP tooling and the SDKs targeted by the original projects: prototype 10.0.26100.0 and advanced 10.0.22621.0; both declare minimum 10.0.17763.0. Restore NuGet dependencies.

Before building the advanced version, copy:

```powershell
Copy-Item advanced/Project2_222257U/ApiCredentials.example.cs advanced/Project2_222257U/ApiCredentials.local.cs
```

Set your own OpenWeather, news, Azure Translator and Together AI credentials in the local file. The `.csproj` explicitly includes that local file; the example is a template, not compiled source. Keep the local file out of Git.

The service providers and original endpoints have not been tested for current availability. Some may need replacement. The initial version contains UI demonstrations and should not be described as a fully connected service.

## Repository preparation

Source and required assets were organised from existing coursework. Build outputs and IDE state are excluded. Original reports, slides and course handouts remain in the separate full backup; they are not bundled for public upload. Real credentials and original database records are not included. See the collection `UPLOAD_GUIDE.md` for upload steps and preparation notes.

## Validation status

The preparation checked file paths, Python syntax where applicable, structured files, credential removal and package integrity. Windows desktop apps, Flutter apps, hardware and online services have not been built or run in this environment.

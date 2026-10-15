# Repository guidance

## Build and test

The repository uses the .NET SDK pinned by `global.json` (10.0.401, rolling forward to the latest feature band). Projects target .NET 8, 9, and 10.

```powershell
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release
```

To run one test class or method, pass the test project and a Microsoft Testing Platform filter:

```powershell
dotnet test test\Shorthand.HttpArchive.Tests\Shorthand.HttpArchive.Tests.csproj --filter-class Shorthand.HttpArchive.Tests.HARSerializerTests
dotnet test test\Shorthand.HttpArchive.Tests\Shorthand.HttpArchive.Tests.csproj --filter-method Shorthand.HttpArchive.Tests.HARSerializerTests.Parse_WhenCalledWithValidFile_ReturnsHARRoot
```

Use `--framework net8.0`, `net9.0`, or `net10.0` to limit a test run to one target framework. There is no separate lint command: `dotnet build` enforces the repository's analyzers and code-style diagnostics, with warnings treated as errors.

## Architecture

- `src/Shorthand.HttpArchive` defines HAR 1.2 data types and JSON parsing/serialization. `HARSerializer` validates the root and HAR version before deserializing; the model types represent the `log`, entries, requests, responses, content, cookies, and timings.
- `src/Shorthand.HttpArchive.HttpClient` references the model library. `HARMessageHandler` is a `DelegatingHandler` that sends requests through its inner handler, captures request/response details and timing data, and appends entries to a `HARSession`. The session serializes those entries through `HARSerializer`. `HAROptions` controls masking of headers, cookies, and query parameters.
- `src/Shorthand.HttpArchive.HttpClient.CommandLine` is a small consumer of the handler that requests URLs supplied as arguments and writes the captured session to `output.har`.
- Tests are split by library: `test\Shorthand.HttpArchive.Tests` covers HAR models and serialization, while `test\Shorthand.HttpArchive.HttpClient.Tests` covers handler behavior and HTTP integration using a test web application. Shared test helpers and embedded HAR fixtures live under `test\Shorthand.HttpArchive.TestHelpers` and `test\Shorthand.HttpArchive.Tests\Resources`.

## Codebase conventions

- HAR model properties generally use `required` for fields required by the format; optional fields are nullable. Preserve those distinctions when changing serialization or model behavior.
- JSON uses `System.Text.Json` with web defaults, indented output, and null properties omitted. Parsing wraps malformed JSON and invalid/unsupported HAR versions in the library's `HARJsonException` and `HARValidationException`.
- HTTP capture methods are asynchronous and accept/pass a `CancellationToken`. Request/response conversion belongs with the HAR model types; handler orchestration and session collection belong in the HttpClient project.
- Masking matches header, cookie, and query-parameter names case-insensitively and replaces matched values with `<Redacted>`. Keep these paths consistent for recorded request and response data.
- Tests use xUnit v3 and Shouldly, with FakeItEasy available for fakes. Test names describe the operation, condition, and expected result; integration tests use the in-process ASP.NET Core test server.
- Package versions are centrally managed in `Directory.Packages.props`. Shared build settings in `Directory.Build.props` enable nullable reference types, implicit usings, latest C# language features, and analyzer/style enforcement.

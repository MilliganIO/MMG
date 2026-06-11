# Skill: Create Integration Tests

> Used by the Developer agent. Defines the exact steps to write HTTP-level integration tests for a feature's API endpoints.

---

## Trigger
Human says:
```
@dev Create integration tests for specs/features/[feature-name].md
```
or
```
@dev Create integration tests for [feature]
```

---

## What Integration Tests Cover
- Full HTTP pipeline: routing, middleware, auth, response codes, body, headers, cookies
- Happy paths and all error states from the spec's error table (Section 4.3)
- No real database — repositories are replaced with in-memory fakes via DI override
- No boilerplate endpoints (e.g. `/weatherforecast`) — only feature endpoints

---

## Checklist

### Setup (one-time per project, skip if already done)
- [ ] `public partial class Program { }` exists at the bottom of `src/Api/Program.cs`
- [ ] `IntegrationTests.csproj` references:
  - `Microsoft.AspNetCore.Mvc.Testing`
  - `FluentAssertions`
  - Project references: `Api`, `Core`
- [ ] `ApiWebApplicationFactory.cs` exists in `tests/IntegrationTests/`
  - Overrides JWT config with known `TestJwtSettings` (secret 32+ chars)
  - Replaces `IUserRepository` (and any other repos) with fakes via `ConfigureServices`
  - Overrides `ConnectionStrings:CDEDashboard` so no real DB is needed
- [ ] A `Fake[X]Repository.cs` exists for each repository the feature uses

### Per-Feature Tests
- [ ] Read the spec's Section 4 (flows) and Section 4.3 (error states) — these become test cases
- [ ] Read the spec's Section 9/11 (acceptance criteria) — every criterion gets at least one test
- [ ] One test class per feature: `[Feature]EndpointsTests.cs`
- [ ] Test naming: `[HttpMethod][Endpoint]_[Scenario]_[ExpectedResult]`
  - e.g. `PostAuthToken_InactiveUser_Returns401`
- [ ] Seed data is defined as private static properties on the test class
- [ ] Each test creates its own `ApiWebApplicationFactory` with the seed data it needs
- [ ] Dispose the factory with `using var factory = ...`

### What to Assert
- [ ] HTTP status code (always)
- [ ] Response body shape (for 200 responses)
- [ ] Response headers/cookies (if the endpoint sets them)
- [ ] JWT claims content (if the endpoint issues a token)

### Do NOT Test
- [ ] Boilerplate/scaffold endpoints not related to the feature
- [ ] Internal service logic (that belongs in unit tests)
- [ ] Real database connectivity

---

## Standard Factory Pattern
If `ApiWebApplicationFactory` does not yet exist, create it:

```csharp
// tests/IntegrationTests/ApiWebApplicationFactory.cs
public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly IEnumerable<User> _users;

    public static readonly JwtSettings TestJwtSettings = new()
    {
        Secret = "integration-test-secret-at-least-32-chars!!",
        Issuer  = "ClaudeLearningApi",
        Audience = "ClaudeLearningPortal",
        ExpiryMinutes = 60
    };

    public ApiWebApplicationFactory(IEnumerable<User>? users = null)
        => _users = users ?? Enumerable.Empty<User>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"]        = TestJwtSettings.Secret,
                ["Jwt:Issuer"]        = TestJwtSettings.Issuer,
                ["Jwt:Audience"]      = TestJwtSettings.Audience,
                ["Jwt:ExpiryMinutes"] = TestJwtSettings.ExpiryMinutes.ToString(),
                ["ConnectionStrings:CDEDashboard"] = "Server=localhost;Database=IntegrationTest;TrustServerCertificate=True;"
            }));

        builder.ConfigureServices(services =>
        {
            services.Remove(services.Single(d => d.ServiceType == typeof(IUserRepository)));
            services.AddScoped<IUserRepository>(_ => new FakeUserRepository(_users));
        });
    }
}
```

---

## Output Format
At the end, provide:
1. List of test cases written and which spec criterion each covers
2. List of files created/modified
3. Confirmation that no boilerplate endpoints were tested

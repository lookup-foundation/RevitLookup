using Build.Modules;
using Build.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModularPipelines;
using ModularPipelines.Extensions;

var builder = Pipeline.CreateBuilder();

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

builder.Services.AddOptions<BuildOptions>().Bind(builder.Configuration.GetSection("Build"));
builder.Services.AddOptions<BundleOptions>().Bind(builder.Configuration.GetSection("Bundle"));
builder.Services.AddOptions<PublishOptions>().Bind(builder.Configuration.GetSection("Publish"));
builder.Services.AddOptions<SigningOptions>().Bind(builder.Configuration.GetSection("Signing"));

if (args.Length == 0)
{
    builder.Services.AddModule<CompileProjectModule>();
}

if (args.Contains("test"))
{
    builder.Services.AddModule<TestProjectModule>();
}

if (args.Contains("pack"))
{
    builder.Services.AddModule<CleanProjectModule>();
    builder.Services.AddModule<CreateBundleModule>();
    builder.Services.AddModule<CreateInstallerModule>();
}

if (args.Contains("publish"))
{
    builder.Services.AddModule<SignAssembliesModule>();
    builder.Services.AddModule<SignInstallerModule>();
    builder.Services.AddModule<PublishGithubModule>();
    builder.Services.AddModule<PublishWinGetModule>();
}

await builder.Build().RunAsync();
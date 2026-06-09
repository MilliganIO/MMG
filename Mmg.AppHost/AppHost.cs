using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var webProj = builder.AddProject<MmgExplorer>("web");

builder.Build().Run();

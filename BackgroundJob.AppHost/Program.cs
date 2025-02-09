var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.BackgroundJob_Producer>("backgroundjob-producer-api");

builder.AddProject<Projects.BackgroundJob_Distributor>("backgroundjob-distributor");

builder.AddProject<Projects.BackgroundJob_Worker>("backgroundjob-worker");

builder.Build().Run();

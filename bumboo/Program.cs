using BuildBot.GrpcHistory.v1;
using Bumboo;
using Bumboo.GrpcBuild.v1;
using Bumboo.GrpcCommon.v1;
using Bumboo.GrpcRelease.v1;
using Grpc.Net.Client;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Env = Bumboo.GrpcRelease.v1.Environment;

const string serverAddress = "https://localhost:5001";

var buildCmd = new Command("build", "Record component builds");
buildCmd.AddOption(new Option<string>(new string[] { "--component", "-c" }, "Name of component which has been built"));
buildCmd.AddOption(new Option<string>(new string[] { "--version", "-v" }, "Version component has been built to"));
buildCmd.Handler = CommandHandler.Create<string, string>(Build);

var checkCmd = new Command("check", "Check if a build is using a valid version number");
checkCmd.AddOption(new Option<string>(new string[] { "--component", "-c" }, "Name of component which has been built"));
checkCmd.AddOption(new Option<string>(new string[] { "--version", "-v" }, "Version component has been built to"));
checkCmd.Handler = CommandHandler.Create<string, string>(Check);

var releaseCmd = new Command("release", "Record the creation of a release");
releaseCmd.AddOption(new Option<int>(new string[] { "--build-id", "-b" }, "Build id for release candidate"));
releaseCmd.AddOption(new Option<string[]>(new string[] { "--components", "-c" }, "Components in release"));
releaseCmd.Handler = CommandHandler.Create<int, string[]>(Release);

var deployCmd = new Command("deploy", "Record that a release candidate has been deployed to an environment");
deployCmd.AddOption(new Option<int>(new string[] { "--rcid", "-r" }, "Id of the canidate being deployed"));
deployCmd.AddOption(new Option<string>(new string[] { "--name", "-n" }, "Tag given to the candidate"));
deployCmd.AddOption(new Option<Env>(new string[] { "--env", "-e" }, "Environment being deployed to"));
deployCmd.Handler = CommandHandler.Create<int, string, Env>(Deploy);

var historyCmd = new Command("history", "Get details of past events");
historyCmd.AddOption(new Option<string>(new string[] { "--component", "-c" }, "Get history for component"));
historyCmd.Handler = CommandHandler.Create<string>(History);

var root = new RootCommand();
root.AddCommand(buildCmd);
root.AddCommand(checkCmd);
root.AddCommand(releaseCmd);
root.AddCommand(deployCmd);
root.AddCommand(historyCmd);
root.Invoke(args);

void Build(string component, string version)
{
    Console.WriteLine($"Built {component} to version {version}");
    var channel = GrpcChannel.ForAddress(serverAddress);
    var client = new Build.BuildClient(channel);

    client.RecordBuild(new Component() { Name = component, Version = version.ToVersion() });
}

void Check(string component, string version)
{
    Console.WriteLine($"Checking if {version} is a valid version for {component}");
    var channel = GrpcChannel.ForAddress(serverAddress);
    var client = new Build.BuildClient(channel);

    var result = client.CheckVersion(new Component() { Name = component, Version = version.ToVersion() });

    Console.WriteLine(result.Message);
}

void Deploy(int rcId, string name, Env env)
{
    Console.WriteLine($"Deploying rc {rcId} to {env}");
    var channel = GrpcChannel.ForAddress(serverAddress);
    var client = new Release.ReleaseClient(channel);
    var tag = new CandidateTag() { Id = rcId, Name = name };

    var result = client.DeployCandidate(new Deployment { Environment = env, Candidate = tag });
}

void Release(int buildId, string[] components)
{
    Console.WriteLine($"Release version {buildId} containing {string.Join(' ', components)}");
    var channel = GrpcChannel.ForAddress(serverAddress);
    var client = new Release.ReleaseClient(channel);
    var rc = new ReleaseCandidate() { Id = buildId };

    foreach (var component in components)
    {
        var parts = component.Split(',');
        rc.Components.Add(new Component() { Name = parts[0], Version = parts[1].ToVersion() });
    }
    var result = client.ProposeCandidate(rc);
}

void History(string component)
{
    Console.WriteLine($"Get build history for {component}");
    var channel = GrpcChannel.ForAddress(serverAddress);
    var client = new History.HistoryClient(channel);

    var results = client.GetComponentHistory(new Identifier() { Id = component });

    foreach (var build in results.History)
    {
        Console.WriteLine($"\tVersion {build.Version} built at {build.OccurredAt}");
    }
}

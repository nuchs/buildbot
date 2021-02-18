namespace BuildBot.Services
{
    using BuildBot.Eventing;
    using BuildBot.GrpcCommon.v1;
    using Version = BuildBot.GrpcCommon.v1.Version;
    using Environment = BuildBot.GrpcRelease.v1.Environment;
    using System;
    using BuildBot.GrpcBuild.v1;

    public static class Convertors
    {
        public static BuildVersion ToBuildVersion(this Version source)
            => new BuildVersion(
                source.Major,
                source.Minor,
                source.Patch);

        public static ComponentInstance ToComponentInstance(this Component source)
            => new ComponentInstance(source.Name, source.Version.ToBuildVersion());

        public static DeployEnviornment ToDeployEnviornment(this Environment source)
            => source switch
            {
                Environment.Dev => DeployEnviornment.Development,
                Environment.Staging => DeployEnviornment.Staging,
                Environment.Prod => DeployEnviornment.Production,
                _ => throw new ArgumentException($"Unrecognised environment {source}")
            };

        public static CheckResult ToCheckResult(this bool result, ComponentInstance component)
            => new CheckResult
            {
                ValidVersion = result,
                Message = result ? "Ok" : $"Version {component.Version} has already been published for {component.Name}"
            };
    }
}

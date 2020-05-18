using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitLab;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Publish);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "source";
    AbsolutePath OutputDirectory => RootDirectory / "output";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(OutputDirectory);
        });


    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Publish => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .Executes(() =>
        {

            DotNetPublish(o => o
                .SetProject(RootDirectory / "middlerApp.API")
                .EnableNoRestore()
                .SetConfiguration(Configuration)
                .SetOutput(OutputDirectory)
                .SetFramework("netcoreapp3.1")
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetInformationalVersion(GitVersion.NuGetVersionV2)
                .EnableNoRestore()
            );

        });

    Target BuildContainer => _ => _
        .Executes(
            () =>
            {
                var buildTag = $"doob/middlerapp:{GitLab.Instance?.CommitTag ?? GitVersion.SemVer}";
                DockerTasks.DockerBuild(settings => settings.SetTag(buildTag).SetPath("."));
            });

}

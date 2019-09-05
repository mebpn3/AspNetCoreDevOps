using System;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Nuke.Docker;
using System.IO;
using Nuke.Common.Tooling;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "test";
    AbsolutePath TestsProject => RootDirectory / "test/AspNetCoreDevOps.Controllers.Tests/AspNetCoreDevOps.Controllers.Tests.csproj";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath DockerFilePath => RootDirectory / "dockerfile";

    string tag = "";
    bool isMaster = false;

    //[Parameter("Github access token for packages")]
    //readonly string GitHubAccessToken;
    string repo = "docker.pkg.github.com/iambipinpaul/dairy/dairy";
    string user = "iambipinpaul";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target CheckDockerVersion => _ => _
    // .DependsOn(CheckBranch)
        .Executes(() =>
        {
            DockerTasks.DockerVersion();
        });

    //Target BuildDockerImage => _ => _
    //    .DependsOn(LoginIntoDockerHub)
    //    .DependsOn(DetermineTag)
    //    .Executes(() =>
    //    {

    //        DockerTasks.DockerBuild(b =>
    //       b.SetFile(DockerFilePath.ToString())
    //           .SetPath(".")
    //           .SetTag($"{repo}:{tag}")
    //   );
    //    });

    //Target LoginIntoDockerHub => _ => _
    //    .DependsOn(CheckDockerVersion)
    //    .Executes(() =>
    //    {
    //        DockerTasks.DockerLogin(l => l
    //        .SetServer("docker.pkg.github.com")
    //        .SetUsername(user)
    //            .SetPassword(GitHubAccessToken)

    //        );
    //    });

    Target ComposeDocuer => _ => _
      .DependsOn(CheckDockerVersion)
      .Executes(() =>
      {
          DockerTasks.DockerRun(l =>
          l.SetImage("postgres")
          .SetName("travis_db")
          .SetEnv("POSTGRES_USER=admin", "POSTGRES_PASSWORD=1q2w3e", "POSTGRES_DB=travisdb")
         // .SetExpose("1234 : 5432")
          .SetDetach(true)
          );
      });


    Target Test => _ => _
       .DependsOn(Compile)
       .DependsOn(ComposeDocuer)
       .Executes(() =>
       {

           DotNetTest(l => l.SetProjectFile(TestsProject));
       });

    //Target DetermineTag => _ => _
    //  .Executes(() =>
    //  {
    //      isMaster = GitRepository.Branch.Equals("master") || GitRepository.Branch.Equals("origin/master");
    //      if (isMaster)
    //      {
    //          tag = "latest";
    //      }
    //      else
    //      {
    //          if (GitRepository.Branch.Equals("dev") || GitRepository.Branch.Equals("origin/dev"))
    //          {
    //              tag = $"dev";
    //          }
    //          if (GitRepository.Branch.Equals("multitenancy") || GitRepository.Branch.Equals("origin/multitenancy"))
    //          {
    //              tag = $"multitenancy";
    //          }
    //      }
    //  });

    //Target PushDockerImage => _ => _
    //    .DependsOn(BuildDockerImage)
    //    .Executes(() =>
    //    {
    //        DockerTasks.DockerPush(p =>
    //            p.SetName($"{repo}"));
    //    });

    //Target LogoutFromDockerHub => _ => _
    //    .DependsOn(PushDockerImage)
    //    .Executes(() =>
    //    {
    //        DockerTasks.DockerLogout(l => l
    //       .SetServer("docker.pkg.github.com")
    //       );

    //    });
    //Target CheckBranch => _ => _
    //   .Executes(() =>
    //   {
    //       Console.WriteLine(GitRepository.Branch);
    //   });


}

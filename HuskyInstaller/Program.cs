using HuskyInstaller;
using Spectre.Console;

internal class Program
{
    private static void RunSetupCommandsInPath(string path)
    {
        var runner = new CommandRunner(path);

        runner.RunCommand("dotnet new tool-manifest");
        runner.RunCommand("dotnet tool install husky");
        runner.RunCommand("dotnet tool restore");
        runner.RunCommand("dotnet husky install");
        runner.RunCommand("dotnet husky add pre-commit -c \"dotnet husky run\"");
    }

    private static void Main()
    {
        var basePath = Directory.GetCurrentDirectory();
        AnsiConsole.Markup("Husky Installer");

        AnsiConsole.MarkupLine($"Working in path [blue]{basePath}[/]");

        var gitPath = Path.Combine(basePath, ".git");


        if (! Directory.Exists(gitPath))
        {
            AnsiConsole.MarkupLine($"[red]{gitPath} not found, not a git repo?[/]");
            return;
        }

        var taskRunnerFile = Path.Combine(basePath, ".husky", "task-runner.json");

        if (File.Exists(taskRunnerFile))
        {
            AnsiConsole.MarkupLine($"Task runner file at [red]{taskRunnerFile}[/] already exists");
            return;
        }
        AnsiConsole.MarkupLine($"Task runner file at [blue]{taskRunnerFile}[/] does not exist");

        RunSetupCommandsInPath(basePath);

        const string taskRunnerData =
            """
            {
               "tasks": [
                  {
                     "name": "dotnet-format",
                     "group": "pre-commit",
                     "command": "dotnet",
                     "args": ["format","--no-restore", "--include", "${staged}"],
                     "include": ["**/*.cs"]
                  }
               ]
            }
            """;
        File.WriteAllText(taskRunnerFile, taskRunnerData.Trim());

        AnsiConsole.MarkupLine("Done");
    }
}
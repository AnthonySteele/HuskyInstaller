using HuskyInstaller;
using Spectre.Console;

static void RunSetupCommandsInPath(string path)
{
    var runner = new CommandRunner(path);

    runner.RunCommand("dotnet new tool-manifest");
    runner.RunCommand("dotnet tool install husky");
    runner.RunCommand("dotnet tool restore");
    runner.RunCommand("dotnet husky install");
    runner.RunCommand("dotnet husky add pre-commit -c \"dotnet husky run\"");
}

static void WriteTaskRunnerFile(string filePath)
{
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

    File.WriteAllText(filePath, taskRunnerData.Trim());
}

var basePath = args.Length > 0 ? args[0] : string.Empty;

if (string.IsNullOrWhiteSpace(basePath))
{
    basePath = Directory.GetCurrentDirectory();
}

AnsiConsole.MarkupLine("Husky Installer...");
AnsiConsole.MarkupLine($"Working in path [blue]{basePath}[/]");

if (!Directory.Exists(basePath))
{
    AnsiConsole.MarkupLine($"[red]{basePath} not found[/]");
    return;
}

var gitPath = Path.Combine(basePath, ".git");

if (!Directory.Exists(gitPath))
{
    AnsiConsole.MarkupLine($"[red]{gitPath} not found, not a git repo?[/]");
    return;
}

var taskRunnerFilePath = Path.Combine(basePath, ".husky", "task-runner.json");

if (File.Exists(taskRunnerFilePath))
{
    AnsiConsole.MarkupLine($"Task runner file at [red]{taskRunnerFilePath}[/] already exists. Exiting.");
    return;
}

RunSetupCommandsInPath(basePath);
WriteTaskRunnerFile(taskRunnerFilePath);

AnsiConsole.MarkupLine($"Written [blue]{taskRunnerFilePath}[/]");
AnsiConsole.MarkupLine("Done");

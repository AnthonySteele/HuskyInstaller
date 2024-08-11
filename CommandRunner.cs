using Spectre.Console;
using System.Diagnostics;

namespace HuskyInstaller;

public class CommandRunner(string path)
{
    public void RunCommand(string command)
    {
        AnsiConsole.MarkupLine($"[green]{command}[/]");

        Process process = new();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.WorkingDirectory = path;
        process.StartInfo.Arguments = "/c " + command;
        process.StartInfo.RedirectStandardOutput = true;

        process.Start();
        process.WaitForExit();

        string output = process.StandardOutput.ReadToEnd();
        Console.WriteLine(output);
    }
}
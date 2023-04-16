using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PairProgrammer;

public class CommandExecutor
{
    private readonly DirectoryViewer _directoryViewer;
    private readonly ProgrammerInterface _programmerInterface;

    public CommandExecutor(string workingDirectory, ProgrammerInterface programmerInterface) {
        _directoryViewer = new DirectoryViewer(workingDirectory);
        _programmerInterface = programmerInterface;
    }

    public string ExecuteCommand(string command) {
        _programmerInterface.LogCommand(command);
        var commands = command.Split('|');
        var output = string.Empty;

        foreach (var cmd in commands) {
            var splitCommand = cmd.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var commandType = splitCommand[0].ToLower();
            var argument = splitCommand.Length > 1 ? splitCommand[1] : string.Empty;
            var additionalArgument = splitCommand.Length > 2 ? splitCommand[2] : string.Empty;

            output = commandType switch {
                "ls" => Command_ls(argument, additionalArgument),
                "cat" => Command_cat(argument),
                "prompt" => Command_prompt(argument),
                "grep" => Command_grep(output, argument, additionalArgument),
                "wc" => Command_wc(output, argument),
                _ => $"Command '{commandType}' not recognized. Please try again."
            };
        }

        return output;
    }

    private string Command_ls(string flag, string path) {
        var entries = flag == "-R" ? _directoryViewer.ListRecursive(path) : _directoryViewer.List(path);
        return string.Join(Environment.NewLine, entries);
    }

    private string Command_cat(string path) {
        return _directoryViewer.Access(path);
    }

    private string Command_prompt(string commandMessage) {
        _programmerInterface.ShowMessage(commandMessage);
        return _programmerInterface.GetMessage();
    }

    private string Command_grep(string input, string flag, string pattern) {
        if (flag == "-c" && !string.IsNullOrEmpty(pattern)) {
            var regex = new Regex(pattern);
            var count = input.Split(Environment.NewLine).Count(line => regex.IsMatch(line));
            return count.ToString();
        }
        return "Invalid usage of 'grep' command. Please try again.";
    }

    private string Command_wc(string input, string flag) {
        if (flag == "-l") {
            var count = input.Split(Environment.NewLine).Length;
            return count.ToString();
        }
        return "Invalid usage of 'wc' command. Please try again.";
    }
}

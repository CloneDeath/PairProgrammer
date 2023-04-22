using System;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace PairProgrammer;

public class CommandExecutor
{
    private readonly DirectoryViewer _directoryViewer;
    private readonly IProgrammerInterface _programmerInterface;

    public CommandExecutor(string workingDirectory, IProgrammerInterface programmerInterface, IFileSystem fileSystem) {
        _directoryViewer = new DirectoryViewer(workingDirectory, fileSystem);
        _programmerInterface = programmerInterface;
    }

    public string ExecuteCommand(Command command) {
        _programmerInterface.LogCommand(command);

        if (!string.IsNullOrEmpty(command.Bash)) {
            var output = ExecuteBash(command.Bash);
            var response = new { output };
            return JsonConvert.SerializeObject(response);
        }
        else {
            var chat = ExecuteChat();
            var response = new { user = chat };
            return JsonConvert.SerializeObject(response);
        }
    }

    private string ExecuteChat() {
        return _programmerInterface.GetMessage();
    }

    public string ExecuteBash(string bash) {
        var commands = bash.Split('|', StringSplitOptions.RemoveEmptyEntries);
        var output = string.Empty;

        foreach (var cmd in commands) {
            var splitCommand = cmd.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var commandType = splitCommand[0].ToLower();
            var argument = splitCommand.Length > 1 ? splitCommand[1] : string.Empty;
            var additionalArgument = splitCommand.Length > 2 ? splitCommand[2] : string.Empty;

            output = commandType switch {
                "ls" => Command_ls(argument, additionalArgument),
                "cat" => Command_cat(argument),
                "prompt" => ExecuteChat(),
                "grep" => Command_grep(output, argument, additionalArgument),
                "wc" => Command_wc(output, argument),
                "date" => Command_date(),
                _ => throw new CommandNotRecognizedException(commandType)
            };
        }

        return output;
    }

    private string Command_date() {
        return DateTime.Now.ToString("ddd MMM dd hh:mm:ss tt zz yyyy");
    }


    private string Command_ls(string flag, string path) {
        var entries = flag == "-R" ? _directoryViewer.ListRecursive(path) : _directoryViewer.List(path);
        return string.Join(Environment.NewLine, entries);
    }

    private string Command_cat(string path) {
        if (_directoryViewer.IsDirectory(path)) {
            return $"cat: {path}: Is a directory";
        }
        return _directoryViewer.Exists(path) 
                   ? _directoryViewer.Access(path) 
                   : $"cat: {path}: No such file or directory";
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

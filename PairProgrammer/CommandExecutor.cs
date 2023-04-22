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

    public string ExecuteBash(string bash)
    {
        var commands = bash.Split('|', StringSplitOptions.RemoveEmptyEntries);
        var output = string.Empty;

        foreach (var cmd in commands) {
            var splitCommand = Regex.Matches(cmd, @"[\+\w]*([^\s""']+|""([^""]*)""|'([^']*)')")
                                    .Select(m => m.Value.Trim('\'', '\"'))
                                    .Where(s => !string.IsNullOrEmpty(s))
                                    .ToArray();
            var commandType = splitCommand[0].ToLower();
            var args = splitCommand.Skip(1).ToArray();


            output = commandType switch {
                "ls" => Command_ls(args),
                "cat" => Command_cat(args),
                "prompt" => ExecuteChat(),
                "grep" => Command_grep(output, args),
                "wc" => Command_wc(output, args),
                "date" => Command_date(args),
                _ => throw new CommandNotRecognizedException(commandType)
            };
        }
        return output;
    }
    
    private string Command_date(string[] args) {
        if (args.Length == 0) {
            return DateTime.Now.ToString("ddd MMM dd hh:mm:ss tt zz yyyy");
        }
        if (args.Length > 1) {
            return $"date: extra operand '{args[1]}'";
        }

        var csFormat = args[0].Replace("'", "")
                              .Replace("+", "")
                              .Replace("%r", "hh:mm:ss tt")
                              .Replace("%A", "dddd")
                              .Replace("%B", "MMMM")
                              .Replace("%d", "dd")
                              .Replace("%Y", "yyyy");
        return DateTime.Now.ToString(csFormat);
    }


    private string Command_ls(string[] args) {
        var flag = args.Length > 0 ? args[0] : string.Empty;
        var path = args.Length > 1 ? args[1] : string.Empty;
        var entries = flag == "-R" ? _directoryViewer.ListRecursive(path) : _directoryViewer.List(path);
        return string.Join(Environment.NewLine, entries);
    }

    private string Command_cat(string[] args) {
        var path = args.Length > 0 ? args[0] : string.Empty;
        if (_directoryViewer.IsDirectory(path)) {
            return $"cat: {path}: Is a directory";
        }
        return _directoryViewer.Exists(path) 
                   ? _directoryViewer.Access(path) 
                   : $"cat: {path}: No such file or directory";
    }

    private string Command_grep(string input, string[] args) {
        var flag = args.Length > 0 ? args[0] : string.Empty;
        var pattern = args.Length > 1 ? args[1] : string.Empty;
        if (flag == "-c" && !string.IsNullOrEmpty(pattern)) {
            var regex = new Regex(pattern);
            var count = input.Split(Environment.NewLine).Count(line => regex.IsMatch(line));
            return count.ToString();
        }
        return "Invalid usage of 'grep' command. Please try again.";
    }

    private string Command_wc(string input, string[] args) {
        var flag = args.Length > 0 ? args[0] : string.Empty;
        if (flag == "-l") {
            var count = input.Split(Environment.NewLine).Length;
            return count.ToString();
        }
        return "Invalid usage of 'wc' command. Please try again.";
    }
}

using System;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using PairProgrammer.Commands;

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
        Thread.Sleep(1);

        if (!string.IsNullOrEmpty(command.Bash)) {
            var output = ExecuteBash(command.Bash);
            var response = new { output };
            return JsonConvert.SerializeObject(response);
        }
        else {
            var chat = _programmerInterface.GetMessage();
            var response = new { user = chat };
            return JsonConvert.SerializeObject(response);
        }
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
                "ls" => new LsCommand(_directoryViewer).Execute(args, output),
                "cat" => new CatCommand(_directoryViewer).Execute(args, output),
                "grep" => new GrepCommand(_directoryViewer).Execute(args, output),
                "wc" => new WcCommand().Execute(args, output),
                "date" => new DateCommand().Execute(args, output),
                _ => throw new CommandNotRecognizedException(commandType)
            };
        }
        return output;
    }
}

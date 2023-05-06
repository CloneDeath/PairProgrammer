using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using PairProgrammer.Commands;
using PairProgrammer.Commands.Grep;

namespace PairProgrammer;

public class CommandExecutor : ICommandFactory {
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
            var splitCommand = ArgumentSplitter.Split(cmd);
            var commandType = splitCommand[0].ToLower();
            var args = splitCommand.Skip(1).ToArray();

            var command = GetCommand(commandType);
            output = command.Execute(args, output);
        }
        return output;
    }

    public ICommand GetCommand(string commandType) {
        var commands = new ICommand[] {
            new CatCommand(_directoryViewer),
            new DateCommand(),
            new GrepCommand(_directoryViewer),
            new FindCommand(_directoryViewer),
            new LsCommand(_directoryViewer),
            new WcCommand(),
            new XArgsCommand(this)
        };
        return commands.FirstOrDefault(c => c.Name == commandType)
               ?? throw new CommandNotRecognizedException(commandType);
    }
}

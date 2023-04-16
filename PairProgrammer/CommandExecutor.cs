using System;

namespace PairProgrammer;

public class CommandExecutor
{
	private readonly DirectoryViewer _directoryViewer;
	private readonly ProgrammerInterface _programmerInterface;

	public CommandExecutor(string workingDirectory, ProgrammerInterface programmerInterface) {
		_directoryViewer = new DirectoryViewer(workingDirectory);
		_programmerInterface = programmerInterface;
	}

	public string ExecuteCommand(Command command)
	{
		if (command.List != null)
		{
			return List(command.List);
		}

		if (command.Access != null)
		{
			return Access(command.Access);
		}

		if (command.Message != null) {
			return CommandMessage(command.Message);
		}

		throw new NotSupportedException("Unsupported command.");
	}

	private string List(string path) {
		_programmerInterface.LogList(path);
		var entries = _directoryViewer.List(path);
		return string.Join(Environment.NewLine, entries);
	}

	private string Access(string path) {
		_programmerInterface.LogAccess(path);
		return _directoryViewer.Access(path);
	}

	private string CommandMessage(string commandMessage) {
		_programmerInterface.ShowMessage(commandMessage);
		return _programmerInterface.GetMessage();
	}
}
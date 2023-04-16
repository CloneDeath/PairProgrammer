using System;

namespace PairProgrammer;

public class CommandExecutor
{
	private readonly DirectoryViewer _directoryViewer;

	public CommandExecutor(string workingDirectory)
	{
		_directoryViewer = new DirectoryViewer(workingDirectory);
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

		if (command.Message != null)
		{
			return $"Message from AI: {command.Message}";
		}

		throw new NotSupportedException("Unsupported command.");
	}

	private string List(string path)
	{
		var entries = _directoryViewer.List(path);
		return string.Join(Environment.NewLine, entries);
	}

	private string Access(string path)
	{
		return _directoryViewer.Access(path);
	}
}
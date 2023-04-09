using System;

namespace PairProgrammer;

public class CommandExecutor
{
	private readonly DirectoryViewer _directoryViewer;

	public CommandExecutor(string workingDir)
	{
		_directoryViewer = new DirectoryViewer(workingDir);
	}

	public string ExecuteCommand(Command command)
	{
		if (command.List != null)
		{
			var list = _directoryViewer.List(command.List);
			return string.Join(Environment.NewLine, list);
		}

		if (command.Access != null)
		{
			return _directoryViewer.Access(command.Access);
		}

		throw new NotSupportedException("Command not supported.");
	}
}
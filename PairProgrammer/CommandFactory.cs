using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using PairProgrammer.GptApi;

namespace PairProgrammer; 

public class CommandFactory {
	private readonly List<ICommand> _commands = new();
	public IEnumerable<ICommand> Commands => _commands;

	public void Register(ICommand command) {
		_commands.Add(command);
	}

	public object Execute(FunctionCall functionCall) {
		var command = _commands.First(c => c.Name == functionCall.Name);
		var argument = JsonSerializer.Deserialize(functionCall.Arguments, command.InputType) ?? throw new NullReferenceException();
		return command.Execute(argument);
	}
}
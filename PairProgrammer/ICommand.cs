using System;

namespace PairProgrammer; 

public interface ICommand {
	public string Name { get; }
	public string Description { get; }
	public Type InputType { get; }

	public object Execute(object input);
}
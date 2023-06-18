using System;

namespace PairProgrammer.Functions; 

public class ReadFileCommand : ICommand {
	private readonly FileSystemAccess _fs;
	
	public string Name => "read_file";
	public string Description => "reads a single file";
	public Type InputType => typeof(string);

	public ReadFileCommand(FileSystemAccess fs) {
		_fs = fs;
	}

	public object Execute(object input) {
		if (input is not string stringInput) throw new ArgumentException("not a string", nameof(input)); 
		return _fs.ReadFile(stringInput);
	}
}
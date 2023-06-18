using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace PairProgrammer.Commands; 

public class ReadFileCommand : ICommand {
	private readonly FileSystemAccess _fs;
	
	public string Name => "read_file";
	public string Description => "reads a single file";
	public Type InputType => typeof(ReadFileInput);

	public ReadFileCommand(FileSystemAccess fs) {
		_fs = fs;
	}

	public object Execute(object input) {
		var args = (ReadFileInput)input;
		return _fs.ReadFile(args.File);
	}
}

public class ReadFileInput {
	[JsonProperty("file", Required = Required.Always)]
	[Description("file to read")]
	public string File { get; set; } = string.Empty;
}
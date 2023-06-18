using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PairProgrammer.Functions; 

public class ReadFileCommand : ICommand {
	private readonly FileSystemAccess _fs;
	
	public string Name => "read_file";
	public string Description => "reads a single file";
	public Type InputType => typeof(ReadFileInput);

	public ReadFileCommand(FileSystemAccess fs) {
		_fs = fs;
	}

	public object Execute(JObject input) {
		var data = input.ToObject<ReadFileInput>() ?? throw new NullReferenceException();
		return _fs.ReadFile(data.File);
	}
}

public class ReadFileInput {
	[JsonProperty("file", Required = Required.Always)]
	[Description("file to read")]
	public string File { get; set; } = string.Empty;
}
using System;
using System.Text.Json.Serialization;
using Json.Schema.Generation;

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
		if (!_fs.FileExists(args.File)) return new { success = false, reason = "File does not exist." };
		return _fs.ReadFile(args.File);
	}
}

public class ReadFileInput {
	[JsonPropertyName("file")]
	[Required]
	[Description("file to read")]
	public string File { get; set; } = string.Empty;
}
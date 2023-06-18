using System;
using System.Linq;
using System.Text.Json.Serialization;
using Json.Schema.Generation;

namespace PairProgrammer.Commands; 

public class ListCommand : ICommand {
	private readonly FileSystemAccess _fs;
	public string Name => "list";
	public string Description => "lists all files and folders in a directory";
	public Type InputType => typeof(ListInput);

	public ListCommand(FileSystemAccess fs) {
		_fs = fs;
	}

	public object Execute(object input) {
		var args = (ListInput)input;
		return new ListOutput {
			Files = _fs.ListFiles(args.Directory, args.Recursive).ToArray(),
			Directories = _fs.ListDirectories(args.Directory, args.Recursive).ToArray()
		};
	}
}

public class ListInput {
	[JsonPropertyName("directory")]
	[Required]
	[Description("the directory to list files & directories from")]
	public string Directory { get; set; } = "./";
	
	[JsonPropertyName("recursive")]
	[Description("if true, returns child files & directories too")]
	public bool Recursive { get; set; }
}

public class ListOutput {
	[JsonPropertyName("files")] public string[] Files { get; set; } = Array.Empty<string>();
	[JsonPropertyName("directories")] public string[] Directories { get; set; } = Array.Empty<string>();
}
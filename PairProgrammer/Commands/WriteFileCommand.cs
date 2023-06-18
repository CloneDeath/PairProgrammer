using System;
using Json.Schema.Generation;

namespace PairProgrammer.Commands
{
	public class WriteFileCommand : ICommand
	{
		private readonly FileSystemAccess _fs;
		private readonly IProgrammerInterface _pi;

		public string Name => "write_file";
		public string Description => "writes to a file";
		public Type InputType => typeof(WriteFileInput);

		public WriteFileCommand(FileSystemAccess fs, IProgrammerInterface pi) {
			_fs = fs;
			_pi = pi;
		}

		public object Execute(object input)
		{
			var args = (WriteFileInput)input;
			if (!_pi.GetApprovalToWriteToFile(args.File, args.Content))
				return new { success = false, reason = "User declined file write." };
			_fs.WriteFile(args.File, args.Content);
			return new { success = true };
		}
	}

	public class WriteFileInput
	{
		[Required]
		[Description("file to write to")]
		public string File { get; set; } = string.Empty;
        
		[Required]
		[Description("content to be written")]
		public string Content { get; set; } = string.Empty;
	}
}
using System;
using Json.Schema.Generation;

namespace PairProgrammer.Commands
{
	public class DeleteFileCommand : ICommand
	{
		private readonly FileSystemAccess _fs;
		private readonly IProgrammerInterface _pi;

		public string Name => "delete_file";
		public string Description => "deletes a file";
		public Type InputType => typeof(DeleteFileInput);

		public DeleteFileCommand(FileSystemAccess fs, IProgrammerInterface pi) {
			_fs = fs;
			_pi = pi;
		}

		public object Execute(object input)
		{
			var args = (DeleteFileInput)input;
			if (!_pi.GetApprovalToDeleteFile(args.File))
				return new { success = false, reason = "User declined file deletion." };
			_fs.DeleteFile(args.File);
			return new { success = true };
		}
	}

	public class DeleteFileInput
	{
		[Required]
		[Description("file to delete")]
		public string File { get; set; } = string.Empty;
	}
}
using System;
using System.IO.Abstractions;

namespace PairProgrammer; 

public class FileSystemAccess {
	private readonly string _root;
	private readonly IFileSystem _fileSystem;
	
	public FileSystemAccess(string root, IFileSystem fileSystem) {
		var sanitizedRoot = fileSystem.Path.GetFullPath(root);
		if (!sanitizedRoot.EndsWith("/")) sanitizedRoot += "/";
		_root = sanitizedRoot;
		_fileSystem = fileSystem;
	}
	
	private string GetFullPath(string path) {
		if (path == ".") return _root;
		
		var fullPath = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(_root, path));
		if (!fullPath.StartsWith(_root)) {
			throw new UnauthorizedAccessException("Access outside of the restricted directory is not allowed. "
												  + $"Tried to access: '{fullPath}', outside of: '{_root}'.");
		}
		return fullPath;
	}
	
	public string ReadFile(string path) {
		var fullPath = GetFullPath(path);
		return _fileSystem.File.ReadAllText(fullPath);
	}
}
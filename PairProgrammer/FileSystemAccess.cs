using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

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

	public string ReadFile(string path) {
		var fullPath = GetFullPath(path);
		return _fileSystem.File.ReadAllText(fullPath);
	}

	public IEnumerable<string> ListFiles(string directory, bool recursive) {
		var fullPath = GetFullPath(directory);
		var files = recursive
			? _fileSystem.Directory.EnumerateFiles(fullPath, "*", SearchOption.AllDirectories)
			: _fileSystem.Directory.EnumerateFiles(fullPath);
		return files.Select(GetLocalPath)
					.Where(f => !IsHidden(f));
	}

	public IEnumerable<string> ListDirectories(string directory, bool recursive) {
		var fullPath = GetFullPath(directory);
		var directories = recursive
							  ? _fileSystem.Directory.EnumerateDirectories(fullPath, "*", SearchOption.AllDirectories)
							  : _fileSystem.Directory.EnumerateDirectories(fullPath);
		return directories.Select(GetLocalPath)
						  .Where(d => !IsHidden(d));
	}

	private string GetFullPath(string path) {
		if (path == ".") return _root;
		if (path == "/") return _root;
		
		var fullPath = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(_root, path));
		if (!fullPath.StartsWith(_root)) {
			throw new UnauthorizedAccessException("Access outside of the restricted directory is not allowed. "
												  + $"Tried to access: '{fullPath}', outside of: '{_root}'.");
		}
		return fullPath;
	}

	private string GetLocalPath(string path) {
		return path.Replace(_root, "/");
	}

	private bool IsHidden(string path) {
		return path.Contains("/.");
	}
}
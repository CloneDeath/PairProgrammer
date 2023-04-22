using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace PairProgrammer;

public class DirectoryViewer {
	private readonly string _root;
	private readonly IFileSystem _fileSystem;

	public DirectoryViewer(string root, IFileSystem fileSystem) {
		_root = fileSystem.Path.GetFullPath(root);
		_fileSystem = fileSystem;
	}

	private string GetFullPath(string path) {
		var fullPath = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(_root, path));
		if (!fullPath.StartsWith(_root)) {
			throw new UnauthorizedAccessException("Access outside of the restricted directory is not allowed.");
		}
		return fullPath;
	}

	public IEnumerable<string> List(string path) {
		var fullPath = GetFullPath(path);
		var directories = _fileSystem.Directory.EnumerateDirectories(fullPath)
									 .Select(d => _fileSystem.Path.GetFileName(d));
		var files = _fileSystem.Directory.EnumerateFiles(fullPath)
							   .Select(f => _fileSystem.Path.GetFileName(f));
		return directories.Concat(files);
	}

	public IEnumerable<string> ListRecursive(string path) {
		var fullPath = GetFullPath(path);
		var directories = _fileSystem.Directory.EnumerateDirectories(fullPath, "*", SearchOption.AllDirectories).Select(d => $"{d}/");
		var files = _fileSystem.Directory.EnumerateFiles(fullPath, "*.*", SearchOption.AllDirectories);
		return directories.Concat(files).Select(e => e.Replace(fullPath, string.Empty));
	}

	public string Access(string path) {
		var fullPath = GetFullPath(path);
		return _fileSystem.File.ReadAllText(fullPath);
	}

	public bool Exists(string path) {
		var fullPath = GetFullPath(path);
		return _fileSystem.File.Exists(fullPath);
	}

	public bool IsDirectory(string path) {
		var fullPath = GetFullPath(path);
		return _fileSystem.Directory.Exists(fullPath);
	}
}
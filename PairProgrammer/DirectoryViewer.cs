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
	
	public IEnumerable<string> ListFiles(string path) {
		var fullPath = GetFullPath(path);
		return _fileSystem.Directory.EnumerateFiles(fullPath);
	}

	public IEnumerable<string> List(string path) {
		var fullPath = GetFullPath(path);
		var directories = _fileSystem.Directory.EnumerateDirectories(fullPath);
		var files = _fileSystem.Directory.EnumerateFiles(fullPath);
		return directories.Concat(files);
	}

	public IEnumerable<string> ListRecursive(string path) {
		var fullPath = GetFullPath(path);
		var directories = _fileSystem.Directory.EnumerateDirectories(fullPath, "*", SearchOption.AllDirectories).Select(d => $"{d}/");
		var files = _fileSystem.Directory.EnumerateFiles(fullPath, "*.*", SearchOption.AllDirectories);
		return directories.Concat(files);
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

	public string GetLocalPath(string file) {
		return file.Replace(_root, ".");
	}

	public string GetFileName(string file) => _fileSystem.Path.GetFileName(file);
}
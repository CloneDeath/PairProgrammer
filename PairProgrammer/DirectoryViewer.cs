using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PairProgrammer; 

public class DirectoryViewer {
	private readonly string _root;

	public DirectoryViewer(string root) {
		_root = root;
	}

	public List<string> List(string path) {
		var fullPath = _root + path.TrimStart('/');
		var directories = Directory.EnumerateDirectories(fullPath).Select(d => $"{d}/");
		var files = Directory.EnumerateFiles(fullPath);
		return directories.Concat(files).Select(e => e.Replace(fullPath, string.Empty)).ToList();
	}

	public string Access(string path) {
		var fullPath = Path.Combine(_root, path);
		return File.ReadAllText(fullPath);
	}
}
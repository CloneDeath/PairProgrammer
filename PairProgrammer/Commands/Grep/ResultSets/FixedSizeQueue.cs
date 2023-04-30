using System.Collections.Generic;
using System.Linq;

namespace PairProgrammer.Commands.Grep.ResultSets; 

public class FixedSizeQueue<T> {
	private readonly int _size;
	private readonly List<T> _contents = new();

	public FixedSizeQueue(int size) {
		_size = size;
	}

	public IEnumerable<T> Contents => _contents;

	public void PushBack(T value) {
		_contents.Add(value);
		if (_contents.Count > _size) {
			_contents.RemoveAt(0);
		}
	}

	public bool TryPushBack(T value) {
		if (_contents.Count >= _size) return false;
		_contents.Add(value);
		return true;
	}

	public bool Any() => Contents.Any();
	public void Clear() => _contents.Clear();
}
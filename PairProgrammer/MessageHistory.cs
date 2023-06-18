using System.Collections.Generic;
using System.Linq;
using PairProgrammer.GptApi;

namespace PairProgrammer; 

public class MessageHistory {
	public List<Message> Messages { get; } = new();
	public int Length => Messages.Count;

	public void Add(Role role, string content) {
		Messages.Add(new Message {
			Role = role, 
			Content = content
		});
	}

	public void Add(Message message) {
		Messages.Add(message);
	}

	public void PopOldest() {
		var message = Messages.First(m => m.Role != Role.System);
		Messages.Remove(message);
	}
}
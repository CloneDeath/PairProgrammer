namespace PairProgrammer; 

public interface ICommandFactory {
	ICommand GetCommand(string commandType);
}
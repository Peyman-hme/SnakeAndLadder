using System.Collections.Generic;

public class Invoker
{
    private static Invoker instance;

    private List<ICommand> history = new List<ICommand>();
    public void ExecuteCommand(ICommand command)
    {
        history.Add(command);
        command.Execute();
    }


    private Invoker()
    {
    }

    public static Invoker GetInstance()
    {
        if (instance == null)
        {
            instance = new Invoker();
        }

        return instance;
    }
}
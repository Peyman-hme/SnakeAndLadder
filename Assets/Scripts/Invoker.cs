using System.Collections.Generic;

public class Invoker
{
    private static Invoker instance;

    public void ExecuteCommand(ICommand command)
    {
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
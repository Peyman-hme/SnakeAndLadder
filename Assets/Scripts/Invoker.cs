using System;
using System.Collections.Generic;
using UnityEngine;

public class Invoker
{
    private static Invoker instance;

    private List<ICommand> history = new List<ICommand>();
    private Queue<ICommand> _commands = new Queue<ICommand>();

    public void ExecuteCommand(ICommand command)
    {
        // history.Add(command);
        // command.Execute();
        AddCommand(command);
    }

    public void AddCommand(ICommand command)
    {
        _commands.Enqueue(command);
    }

    public void Update()
    {
        if (_commands.Count > 0)
        {
            _commands.Dequeue().Execute();
        }
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
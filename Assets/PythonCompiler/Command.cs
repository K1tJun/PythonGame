using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public abstract class Command {}

public abstract class Function {}


public class Move : Function
{ 
    public PyroCommand.MoveDirection Direction;
}

public class Rotate : Function
{
    public PyroCommand.RotateDirection Direction;
}

public class For : Command 
{
    public string Variable;
    public int Count;
    public int Ident;
    public List<Command> Body = new List<Command>();
}

public class ExecutionFrame
{
    public For loop;
    public int Iteration;
    public int commandIndex;
}

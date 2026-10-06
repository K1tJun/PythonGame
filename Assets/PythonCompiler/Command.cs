using UnityEngine;
using System.Collections.Generic;

public abstract class Command { }

public abstract class Function : Command { }

public abstract class Block : Command 
{
    public int Ident;
    public List<Command> Body = new List<Command>();
}



public class Move : Function
{
    public PyroCommand.MoveDirection Direction;
}

public class Rotate : Function
{
    public PyroCommand.RotateDirection Direction;
}


public class For : Block
{
    public string Variable;
    public int Count;
}

public class If : Block
{
    public string Left;
    public string op;
    public string Right;
}


public class Variable : Command 
{
    public string name;
    public int value;
}


public class ExecutionFrame
{
    public Block block;
    public int Iteration;
    public int commandIndex;
}

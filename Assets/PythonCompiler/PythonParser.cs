using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PythonParser : MonoBehaviour
{
    public PyroController _pyroController;

    List<Command> code = new List<Command>();
    List<int> codeSpace = new List<int>();

    List<Command> enterCode = new List<Command>();
    Stack<ExecutionFrame> frame = new Stack<ExecutionFrame>();

    int currentCommandIndex = 0;


    Dictionary<string, int> Variables = new Dictionary<string, int>();


    public void chekTokens(string[] line)
    {
        codeSpace.Clear();
        code.Clear();
        enterCode.Clear();

        currentCommandIndex = 0;

        for (int i = 0; i < line.Length; i++)
        {
            string[] pieceLine = line[i].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    
            if (line[i].Trim().StartsWith("robot."))
                ChekFun(line[i]);
            else if (line[i].Trim().StartsWith("for"))
                ChekFor(line[i]);

            // Variables
            else if (pieceLine.Length > 1)
            {
                if (pieceLine[1] == "=")
                    ChekVariable(line[i]);
            }
        }

        AST();
    }

    private void ChekFun(string getLine)
    {
        int trim = getLine.Length - getLine.TrimStart().Length;

        string[] _line = getLine.Trim().Split(".");

        switch (_line[1])
        {
            case "move":
                Move move = new Move();
                switch (_line[2])
                {
                    case "forward()":
                        move.Direction = PyroCommand.MoveDirection.forward;
                        code.Add(move);
                        codeSpace.Add(trim);
                        break;
                    case "backward()":
                        move.Direction = PyroCommand.MoveDirection.backward;
                        code.Add(move);
                        codeSpace.Add(trim);
                        break;
                }
                break;
            case "rotate":
                Rotate rotate = new Rotate();
                switch (_line[2])
                {
                    case "right()":
                        rotate.Direction = PyroCommand.RotateDirection.right;
                        code.Add(rotate);
                        codeSpace.Add(trim);
                        break;
                    case "left()":
                        rotate.Direction = PyroCommand.RotateDirection.left;
                        code.Add(rotate);
                        codeSpace.Add(trim);
                        break;
                }
                break;
        }


    }

    private void ChekFor(string getLine)
    {
        int trim = getLine.Length - getLine.TrimStart().Length;
        string[] line = getLine.Trim().Split(" ");

        if (line.Length == 4)
        {
            For _for = new For();

            _for.Variable = line[1];

            if (line[2] == "in")
            {
                if (line[3].StartsWith("range") && line[3].EndsWith(":"))
                {
                    int start = line[3].IndexOf('(');
                    int end = line[3].IndexOf(')');

                    string numberText = line[3].Substring(start + 1, end - start - 1);

                    _for.Count = int.Parse(numberText);
                    _for.Ident = trim;

                    code.Add(_for);
                    codeSpace.Add(trim);
                }
            }
        }
    }

    
    private void ChekVariable(string getLine)
    {
        int trim = getLine.Length - getLine.TrimStart().Length;

        string[] _line = getLine.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if(_line.Length == 3)
        {
            if(int.TryParse(_line[2], out int value) && !int.TryParse(_line[0], out int _x_))
            {
                Variable newVariable = new Variable();
                newVariable.name = _line[0];
                newVariable.value = value;

                code.Add(newVariable);
                codeSpace.Add(trim);

                Variables[_line[0]] = value;
            }
        }
    }
    


    private void ExecuteFunctions(Command fun)
    {
        if (fun is Move move)
            _pyroController.Move(move.Direction);
        else if (fun is Rotate rotate)
            _pyroController.Rotate(rotate.Direction);
    }





    private void AST() //Abstract Syntax Tree
    {
        Stack<For> forStack = new Stack<For>();

        for(int i = 0; i < code.Count; i++)
        {
            int ident = codeSpace[i];

            while (forStack.Count > 0 &&
                ident <= forStack.Peek().Ident)
            {
                forStack.Pop();
            } 

            if(code[i] is For newFor)
            {
                if (forStack.Count > 0)
                    forStack.Peek().Body.Add(code[i]);
                else
                    enterCode.Add(code[i]);

                forStack.Push(newFor);
            }
            else
            {
                if (forStack.Count > 0)
                    forStack.Peek().Body.Add(code[i]);
                else
                    enterCode.Add(code[i]);
            }
        }


    }


    private void Update()
    {
        Debuger();

        Executer();
    }

    private void Executer()
    {
        if (_pyroController.IsBusy)
            return;

        if (enterCode.Count <= currentCommandIndex)
            return;

        Command enterCommand = enterCode[currentCommandIndex];


        if (enterCommand is Function)
        {
            ExecuteFunctions(enterCommand);
            currentCommandIndex++;
        }

        else if (enterCommand is For enterFor)
        {
            if(frame.Count == 0)
            {
                ExecutionFrame _newFrame = new();
                _newFrame.loop = enterFor;

                frame.Push(_newFrame);
            }



            if (frame.Count > 0)
            {
                ExecutionFrame currentFrame = frame.Peek();

                if (currentFrame.loop.Count > currentFrame.Iteration)
                {
                    if (currentFrame.loop.Body.Count > currentFrame.commandIndex)
                    {
                        Command currentCommand = currentFrame.loop.Body[currentFrame.commandIndex];
                        if (currentCommand is Function)
                        {
                            ExecuteFunctions(currentCommand);
                            currentFrame.commandIndex++;
                        }
                        else if (currentCommand is For newFor)
                        {
                            ExecutionFrame newFrame = new();
                            newFrame.loop = newFor;

                            frame.Push(newFrame);
                            currentFrame.commandIndex++;
                        }
                    }
                    else
                    {
                        currentFrame.commandIndex = 0;
                        currentFrame.Iteration++;
                    }
                }
                else
                {
                    frame.Pop();

                    if (frame.Count == 0)
                        currentCommandIndex++;
                }
            }
        }


        // костыль for variable
        else
            currentCommandIndex++;
    }

    private void Debuger()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(string.Join(" ,", code));
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach (var codeLine in code)
            {
                if (codeLine is For forLine)
                {
                    Debug.Log(forLine.Variable + "  " + forLine.Count + "  " + string.Join(" ,", forLine.Body));
                    //Debug.Log(string.Join(" ,", forLine.Body));
                }
            }
        }
    }
}


using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PythonParser : MonoBehaviour
{
    public PyroController _pyroController;

    List<Command> commands = new List<Command>();
    List<int> commandSpace = new List<int>();

    int lineIndex = 0;


    public void chekTokens(string[] line)
    {
        commandSpace.Clear();
        commands.Clear();

        lineIndex = 0;

        for(int i = 0; i < line.Length; i++)
        {
            if (line[i].Trim().StartsWith("robot."))
                ChekFun(line[i]);
            else if (line[i].Trim().StartsWith("for"))
                ChekFor(line[i]);
        }
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
                        commands.Add(move);
                        commandSpace.Add(trim);
                        break;
                    case "backward()":
                        move.Direction = PyroCommand.MoveDirection.backward;
                        commands.Add(move);
                        commandSpace.Add(trim);
                        break;
                }
                break;
            case "rotate":
                Rotate rotate = new Rotate();
                switch (_line[2])
                {               
                    case "right()":
                        rotate.Direction = PyroCommand.RotateDirection.right;
                        commands.Add(rotate);
                        commandSpace.Add(trim);
                        break;
                    case "left()":
                        rotate.Direction = PyroCommand.RotateDirection.left;
                        commands.Add(rotate);
                        commandSpace.Add(trim);
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

                    commands.Add(_for);
                    commandSpace.Add(trim);
                }
            }
        }
    }


    private void functions(Command fun)
    {
        if (fun is Move move)
            _pyroController.Move(move.Direction);
        else if (fun is Rotate rotate)
            _pyroController.Rotate(rotate.Direction);
    }


    private void Update()
    {
        if(commands != null)
        {
            if (commands.Count >= lineIndex + 1 && !_pyroController.IsBusy)
            {
                if (commands[lineIndex] is Move or Rotate)
                    functions(commands[lineIndex]);

                Debug.Log(commands[lineIndex] + " space: " + commandSpace[lineIndex]);
                lineIndex++;
            }
        }


        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            Debug.Log(string.Join(" ,", commands));
        }
    }
}


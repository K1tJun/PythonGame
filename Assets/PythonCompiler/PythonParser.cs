using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PythonParser : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference holdSpace;
    public InputActionReference pressL;



    [Space(20)]
    [Header("Components")]

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
            else if (line[i].Trim().StartsWith("if"))
                ChekIf(line[i]);

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
        string[] line = getLine.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

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

                    if (int.TryParse(numberText, out int intNumberText))
                        _for.Count = intNumberText;
                    else if (Variables.ContainsKey(numberText))
                        _for.Count = Variables[numberText];

                    _for.Ident = trim;

                    code.Add(_for);
                    codeSpace.Add(trim);
                }
            }
        }
    }

    private void ChekIf(string getLine)
    {
        int trim = getLine.Length - getLine.TrimStart().Length;
        string[] line = getLine.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (line.Length != 4 || !line[3].EndsWith(":"))
            return;

        string[] operators = { "<", ">", "==", "<=", ">=" };

        if (!operators.Contains(line[2]))
            return;

        string left = line[1];
        string right = line[3].TrimEnd(':');

        if (!int.TryParse(left, out int _x_) && !Variables.ContainsKey(left))
            return;
        if (!int.TryParse(right, out _x_) && !Variables.ContainsKey(right))
            return;

        If newIf = new();

        newIf.op = line[2];
        newIf.Left = left;
        newIf.Right = right;

        newIf.Ident = trim;

        code.Add(newIf);
        codeSpace.Add(trim);
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
    







    private void AST() //Abstract Syntax Tree
    {
        Stack<For> forStack = new Stack<For>();
        Stack<Block> blockStack = new Stack<Block>();

        for(int i = 0; i < code.Count; i++)
        {
            int ident = codeSpace[i];

            while (blockStack.Count > 0 &&
                ident <= blockStack.Peek().Ident)
            {
                blockStack.Pop();
            }

            if (code[i] is Block newBlock)
            {
                if (blockStack.Count > 0)
                    blockStack.Peek().Body.Add(code[i]);
                else
                    enterCode.Add(code[i]);

                blockStack.Push(newBlock);
            }
            else
            {
                if (blockStack.Count > 0)
                    blockStack.Peek().Body.Add(code[i]);
                else
                    enterCode.Add(code[i]);
            }
        }


    }


    private void Update()
    {
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

        else if (enterCommand is Block enterBlock)
        {
            if(frame.Count == 0)
            {
                ExecutionFrame _newFrame = new();
                _newFrame.block = enterBlock;

                frame.Push(_newFrame);
            }



            ExecutionFrame currentFrame = frame.Peek();

            if (currentFrame.block is For currentFor)
                ExecuteFor(currentFrame, currentFor);
            else if (currentFrame.block is If currentIf)
                ExecuteIf(currentFrame, currentIf);
        }


        // костыль for variable
        else
            currentCommandIndex++;
    }


    private void ExecuteFunctions(Command fun)
    {
        if (fun is Move move)
            _pyroController.Move(move.Direction);
        else if (fun is Rotate rotate)
            _pyroController.Rotate(rotate.Direction);
    }

    private void ExecuteFor(ExecutionFrame currentFrame, For currentFor)
    {
        if (currentFor.Count > currentFrame.Iteration)
        {
            if (currentFrame.block.Body.Count > currentFrame.commandIndex)
            {
                Command currentCommand = currentFrame.block.Body[currentFrame.commandIndex];

                EndExecute(currentCommand, currentFrame);
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

    private void ExecuteIf(ExecutionFrame currentFrame, If currentIf)
    {
        if(currentFrame.block.Body.Count > currentFrame.commandIndex)
        {
            if(ChekIfCondition(currentIf.Left, currentIf.op, currentIf.Right) || currentFrame.commandIndex > 0)
            {
                Command currentCommand = currentFrame.block.Body[currentFrame.commandIndex];

                EndExecute(currentCommand, currentFrame);
            }
            else
            {
                frame.Pop();

                if (frame.Count == 0)
                    currentCommandIndex++;
            }
        }
        else
        {
            frame.Pop();

            if (frame.Count == 0)
                currentCommandIndex++;
        }
    }
    bool ChekIfCondition(string strLeft, string oper, string strRight)
    {
        int left;
        int right;


        if (int.TryParse(strLeft, out int intLeft))
            left = intLeft;
        else
            left = Variables[strLeft];

        if (int.TryParse(strRight, out int intRight))
            right = intRight;
        else
            right = Variables[strRight];

        switch (oper) 
        {
            case "<":
                if (left < right)
                    return true;
                else
                    return false;

            case ">":
                if (left > right)
                    return true;
                else
                    return false;

            case "<=":
                if (left <= right)
                    return true;
                else
                    return false;

            case ">=":
                if (left >= right)
                    return true;
                else
                    return false;

            case "==":
                if (left == right)
                    return true;
                else
                    return false;

            default:
                return false;
        }

    }


    private void EndExecute(Command currentCommand, ExecutionFrame currentFrame)
    {
        if (currentCommand is Function)
        {
            ExecuteFunctions(currentCommand);
            currentFrame.commandIndex++;
        }
        else if (currentCommand is For newFor)
        {
            ExecutionFrame newFrame = new();
            newFrame.block = newFor;

            frame.Push(newFrame);
            currentFrame.commandIndex++;
        }
        else if (currentCommand is If newIf)
        {
            ExecutionFrame newFrame = new();
            newFrame.block = newIf;

            frame.Push(newFrame);
            currentFrame.commandIndex++;
        }
    }

    





    //Input Action

    private void OnEnable()
    {
        holdSpace.action.Enable();

        pressL.action.performed += OnalgPressed;
        pressL.action.Enable();
    }

    private void OnDisable()
    {
        pressL.action.performed -= OnalgPressed;

        holdSpace.action.Disable();
        pressL.action.Disable();
    }

    private void OnalgPressed(InputAction.CallbackContext context)
    {
        if (holdSpace.action.IsPressed())
        {
            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                Debug.Log(string.Join(" ,", code));
            }
        }
    }
}

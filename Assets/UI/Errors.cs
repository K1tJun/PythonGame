using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class Errors : MonoBehaviour
{
    public ErrorMes[] errors;

    public ErrorMes GetError(string name)
    {
        foreach (var er in errors)
        {
            if (er.errorName == name)
                return er;

        }
        return errors[0];
    }
}



using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UnityEngine;

public class PythonOutputRedirector
{
    private StringBuilder buffer = new();

    public void write(string text) => buffer.Append(text);
    public void flush() { }
    public string GetOutput() => buffer.ToString();
    public void Clear() => buffer.Clear();
}

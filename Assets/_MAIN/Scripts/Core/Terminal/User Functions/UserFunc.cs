using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserFunc
{

    public string code;
    public int index;
    public string name;

    public UserFunc(string code, int index, string name = "")
    {
        this.code = code;
        this.index = index;
        this.name = name;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE.LogicalLines
{
    // this is so that we can add multiple types of logical lines we want to add
    // for now it's just going to be for the choices which is the short quiz from the npc
    public interface ILogicalLine
    {
        string keyword { get; }
        bool Matches(DIALOGUE_LINES line);
        IEnumerator Execute(DIALOGUE_LINES line);
    }

}


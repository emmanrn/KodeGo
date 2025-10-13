using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAIN_GAME
{
    [System.Serializable]
    public class Game_ConversationData
    {
        public List<string> conversation = new List<string>();
        public int progress;
    }

}
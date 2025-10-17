using HISTORY;
using UnityEngine;

namespace PLAYER
{
    public class PlayerToggleHistoryLogs : MonoBehaviour
    {
        public void OnToggleHistoryLog()
        {
            var logs = HistoryManager.instance.logManager;

            if (!logs.isOpen)
                logs.Open();
            else
                logs.Close();
        }
    }

}
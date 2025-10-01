using System.Collections.Generic;

namespace HISTORY
{

    // this script is for saving the state like the actual state in the dialogue, so you can go back and forth those states in case you want to go back
    // BUT, obviously this is not need for the game, but ill integrate it anyway for the history logs, because this will be part of how to get the logs

    // we make this class saveable
    // as well as the data containers
    [System.Serializable]
    public class HistoryState
    {
        public DialogueData dialogue;
        public List<CharacterData> characters;
        // public List<AudioData> audios;
        // public List<GraphicData> graphics;

        public static HistoryState Capture()
        {
            HistoryState state = new HistoryState();
            state.dialogue = DialogueData.Capture();
            state.characters = CharacterData.Capture();

            return state;
        }
        public void Load()
        {
            DialogueData.Apply(dialogue);
            CharacterData.Apply(characters);
        }
    }
}

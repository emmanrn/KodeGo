using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        // Start is called before the first frame update\
        private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);
        void Start()
        {
            //Character Pseudo = CharacterManager.instance.CreateCharacter("Pseudo");
            //Character Pseudo2 = CharacterManager.instance.CreateCharacter("Pseudo");

            StartCoroutine(Test());
        }


        IEnumerator Test()
        {
            Character_Sprite Kode = CreateCharacter("Kode") as Character_Sprite;

            Sprite bodySprite = Kode.GetSprite("A1");
            Kode.SetSprite(bodySprite);

            bodySprite = Kode.GetSprite("A24");

            yield return new WaitForSeconds(1f);

            Kode.SetSprite(bodySprite);

            yield return null;
        }

    }

}
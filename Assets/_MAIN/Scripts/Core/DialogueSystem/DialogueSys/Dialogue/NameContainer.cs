using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    [System.Serializable]
    public class NameContainer
    {
        // this is part of the dialogue container
        // the box that holds the name txt on screen.

        [SerializeField] private GameObject root;
        [SerializeField] private TextMeshProUGUI nameTxt;
        public void Show(string nameToShow = "")
        {
            root.SetActive(true);

            if (nameToShow != string.Empty)
                nameTxt.text = nameToShow;
        }
        public void Hide()
        {
            root.SetActive(false);
        }

        public void SetNameColor(Color color) => nameTxt.color = color;
    }

}

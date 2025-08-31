using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CHARACTERS;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DB_EXTENSION_CHARACTERS : CMD_DB_EXTENSION
    {
        private static string[] PARAM_IMMEDIATE => new string[] { "-i", "-immediate" };
        private static string[] PARAM_ENABLED => new string[] { "-e", "-enabled" };
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("createcharacter", new Action<string[]>(CreateCharacter));
            database.AddCommand("show", new Func<string[], IEnumerator>(ShowAll));
            database.AddCommand("hide", new Func<string[], IEnumerator>(HideAll));

            // add commands available to all characters
            CommandDatabase baseCommands = CommandManager.instance.CreateSubDatabase(CommandManager.DATABASE_CHARACTERS_BASE);
            baseCommands.AddCommand("show", new Func<string[], IEnumerator>(Show));
            baseCommands.AddCommand("hide", new Func<string[], IEnumerator>(Hide));

            CommandDatabase spriteCharacterCommands = CommandManager.instance.CreateSubDatabase(CommandManager.DATABASE_CHARACTERS_SPRITE);
            spriteCharacterCommands.AddCommand("setsprite", new Func<string[], IEnumerator>(SetSprite));

        }

        public static void CreateCharacter(string[] data)
        {
            string characterName = data[0];
            bool enable = false;
            bool immediate = false;

            var parameters = ConvertDataToParams(data);

            parameters.TryGetValue(PARAM_ENABLED, out enable, defaultVal: false);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultVal: false);

            Character character = CharacterManager.instance.CreateCharacter(characterName);

            if (!enable)
                return;

            if (immediate)
                character.isVisible = true;
            else
                character.Show();
        }

        public static IEnumerator ShowAll(string[] data)
        {
            List<Character> characters = new();
            bool immediate = false;

            foreach (string s in data)
            {
                Character character = CharacterManager.instance.GetCharacter(s, createIfDoesnNotExist: false);
                if (character != null)
                    characters.Add(character);
            }

            if (characters.Count == 0)
                yield break;

            // convert the data array to a param container
            var parameters = ConvertDataToParams(data);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultVal: false);

            foreach (Character character in characters)
            {
                if (immediate)
                    character.isVisible = true;
                else
                    character.Show();
            }

            if (!immediate)
            {
                // if we are working on a function and we want the player to skip it
                // MAKE SURE TO ADD THIS LINE TO ADD IT TO THE ACTIVE PROCESSES LIST AND MAKE IT FORCE COMPLETE
                CommandManager.instance.AddTerminateActionToCurrentProcess(() =>
                {
                    foreach (Character character in characters)
                    {
                        character.isVisible = true;
                    }
                });
                while (characters.Any(c => c.isRevealing))
                    yield return null;
            }

        }

        public static IEnumerator HideAll(string[] data)
        {
            List<Character> characters = new();
            bool immediate = false;

            foreach (string s in data)
            {
                Character character = CharacterManager.instance.GetCharacter(s, createIfDoesnNotExist: false);
                if (character != null)
                    characters.Add(character);
            }

            if (characters.Count == 0)
                yield break;

            // convert the data array to a param container
            var parameters = ConvertDataToParams(data);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultVal: false);

            foreach (Character character in characters)
            {
                if (immediate)
                    character.isVisible = false;
                else
                    character.Hide();
            }

            if (!immediate)
            {
                CommandManager.instance.AddTerminateActionToCurrentProcess(() =>
                {
                    foreach (Character character in characters)
                    {
                        character.isVisible = true;
                    }
                });
                while (characters.Any(c => c.isHiding))
                    yield return null;
            }

        }
        #region CHARACTER BASE COMMANDS
        private static IEnumerator Show(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0]);

            if (character == null)
                yield break;

            character.isVisible = true;
        }
        private static IEnumerator Hide(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0]);

            if (character == null)
                yield break;

            character.isVisible = false;
        }

        public static IEnumerator SetSprite(string[] data)
        {
            Character_Sprite character = CharacterManager.instance.GetCharacter(data[0], createIfDoesnNotExist: false) as Character_Sprite;
            string spriteName;

            if (character == null || data.Length < 2)
                yield break;

            var parameters = ConvertDataToParams(data, startingIndex: 1);

            parameters.TryGetValue(new string[] { "-s", "-sprite" }, out spriteName);

            Sprite sprite = character.GetSprite(spriteName);

            if (sprite == null)
                yield break;

            character.SetSprite(sprite);
        }


        #endregion
    }
}

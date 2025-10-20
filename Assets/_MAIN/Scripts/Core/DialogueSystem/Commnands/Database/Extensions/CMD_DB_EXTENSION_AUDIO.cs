using System;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DB_EXTENSION_AUDIO : CMD_DB_EXTENSION
    {
        // SFX PARAMS
        private static string[] PARAM_SFX = new string[] { "-s", "-sfx" };
        private static string[] PARAM_VOLUME = new string[] { "-v", "-vol", "-volume" };
        private static string[] PARAM_PITCH = new string[] { "-p", "-pitch" };
        private static string[] PARAM_LOOP = new string[] { "-l", "-loop" };

        // MUSIC/AMBIENCE PARAMS
        private static string[] PARAM_CHANNEL = new string[] { "-c", "-channel" };
        private static string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
        private static string[] PARAM_START_VOLUME = new string[] { "-sv", "-startvolume" };
        private static string[] PARAM_SONG = new string[] { "-s", "-song" };
        private static string[] PARAM_AMBIENCE = new string[] { "-a", "-ambience" };

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("playsfx", new Action<string[]>(PlaySFX));
            database.AddCommand("stopsfx", new Action<string>(StopSFX));

            database.AddCommand("playsong", new Action<string[]>(PlaySong));
            database.AddCommand("playambience", new Action<string[]>(PlayAmbience));

            database.AddCommand("stopsong", new Action<string>(StopSong));
            database.AddCommand("stopambience", new Action<string>(StopAmbience));
        }

        private static void PlaySFX(string[] data)
        {
            string filePath;
            float volume, pitch;
            bool loop;

            var parameters = ConvertDataToParams(data);

            parameters.TryGetValue(PARAM_SFX, out filePath);
            parameters.TryGetValue(PARAM_VOLUME, out volume, defaultVal: 1f);
            parameters.TryGetValue(PARAM_PITCH, out pitch, defaultVal: 1f);
            parameters.TryGetValue(PARAM_LOOP, out loop, defaultVal: false);

            AudioClip sound = Resources.Load<AudioClip>(FilePaths.GetPathToResource(FilePaths.resources_sfx, filePath));

            if (sound == null)
                return;

            AudioManager.instance.PlaySoundEffect(sound, volume: volume, pitch: pitch, loop: loop);
        }

        private static void StopSFX(string data)
        {
            AudioManager.instance.StopSoundEffect(data);
        }

        private static void PlaySong(string[] data)
        {
            string filePath;
            int channel;

            var parameters = ConvertDataToParams(data);

            parameters.TryGetValue(PARAM_SONG, out filePath);
            filePath = FilePaths.GetPathToResource(FilePaths.resources_music, filePath);

            parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultVal: 1);

            PlayTrack(filePath, channel, parameters);
        }


        private static void PlayAmbience(string[] data)
        {
            string filePath;
            int channel;

            var parameters = ConvertDataToParams(data);

            parameters.TryGetValue(PARAM_AMBIENCE, out filePath);
            filePath = FilePaths.GetPathToResource(FilePaths.resources_ambience, filePath);

            parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultVal: 1);

            PlayTrack(filePath, channel, parameters);

        }

        private static void PlayTrack(string filePath, int channel, CommandParameters parameters)
        {
            bool loop;
            float volumeCap;
            float startVolume;
            float pitch;

            parameters.TryGetValue(PARAM_VOLUME, out volumeCap, defaultVal: 1f);
            parameters.TryGetValue(PARAM_START_VOLUME, out startVolume, defaultVal: 0f);
            parameters.TryGetValue(PARAM_PITCH, out pitch, defaultVal: 1f);
            parameters.TryGetValue(PARAM_LOOP, out loop, defaultVal: true);

            AudioClip sound = Resources.Load<AudioClip>(filePath);

            if (sound == null)
            {
                Debug.LogError($"Was not able to load track '{filePath}");
                return;
            }

            AudioManager.instance.PlayTrack(sound, channel, loop, startVolume, volumeCap, pitch, filePath);
        }

        private static void StopSong(string data)
        {
            if (data == string.Empty)
                StopTrack("1");
            else
                StopTrack(data);
        }

        private static void StopAmbience(string data)
        {
            if (data == string.Empty)
                StopTrack("1");
            else
                StopTrack(data);
        }

        private static void StopTrack(string data)
        {
            if (int.TryParse(data, out int channel))
                AudioManager.instance.StopTrack(channel);
            else
                AudioManager.instance.StopTrack(data);
        }



    }
}
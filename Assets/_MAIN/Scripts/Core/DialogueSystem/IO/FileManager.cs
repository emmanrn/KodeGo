using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class FileManager
{
    private const string KEY = "VERYSCRETKEY";
    public static List<string> ReadTxtFiles(string filePath, bool includeBlankLines = true)
    {
        // checking if the file path is like relative or absolute path
        // relative path -> will always be contained in the defined "root" directory
        // bsolute path -> is basically a path that points to anywhere inside or outside the game folders
        // if path starts with '/' then that is an abosolute path then add the root directory to that absolute 
        if (!filePath.StartsWith('/'))
            filePath = FilePaths.root + filePath;


        // reading the contents in the txt file
        List<string> lines = new();

        try
        {
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();

                    if (includeBlankLines || !string.IsNullOrWhiteSpace(line))
                        lines.Add(line);
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            Debug.LogError($"File not found: '{ex.FileName}'");
        }

        return lines;
    }

    public static List<string> ReadTxtAsset(string filePath, bool includeBlankLines = true)
    {
        TextAsset asset = Resources.Load<TextAsset>(filePath);
        if (asset == null)
        {
            Debug.LogError($"Asset not found: '{filePath}'");
            return null;
        }
        return ReadTxtAsset(asset, includeBlankLines);
    }

    public static List<string> ReadTxtAsset(TextAsset asset, bool includeBlankLines = true)
    {
        List<string> lines = new List<string>();
        using (StringReader sr = new StringReader(asset.text))
        {
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                if (includeBlankLines || !string.IsNullOrWhiteSpace(line))
                    lines.Add(line);
            }
        }
        return lines;
    }

    public static bool TryCreateDirectoryFromPath(string path)
    {
        if (Directory.Exists(path) || File.Exists(path))
            return true;

        if (path.Contains("."))
        {
            path = Path.GetDirectoryName(path);
            if (Directory.Exists(path))
                return true;
        }

        if (path == string.Empty)
            return false;

        try
        {
            Directory.CreateDirectory(path);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Could not create directory! {e} ");
            return false;
        }
    }

    public static void Save(string filePath, string JSONData, bool encrypt = false)
    {
        if (!TryCreateDirectoryFromPath(filePath))
        {
            Debug.LogError($"Failed to save file {filePath}");
            return;
        }

        // for the encryption of the files we're going to use a simple encryption with XOR
        // first we get the bytes of the data we want to save and the bytes of the KEY
        if (encrypt)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(JSONData);
            byte[] keyBytes = Encoding.UTF8.GetBytes(KEY);
            byte[] encryptedBytes = XOR(dataBytes, keyBytes);

            File.WriteAllBytes(filePath, encryptedBytes);
        }
        else
        {
            StreamWriter sw = new StreamWriter(filePath);
            sw.Write(JSONData);
            sw.Close();
        }



        Debug.Log($"Saved at {filePath}");
    }

    public static T Load<T>(string filePath, bool encrypt = false)
    {
        if (File.Exists(filePath))
        {
            if (encrypt)
            {
                byte[] encryptedBytes = File.ReadAllBytes(filePath);
                byte[] keyBytes = Encoding.UTF8.GetBytes(KEY);

                byte[] decryptedBytes = XOR(encryptedBytes, keyBytes);

                string decryptedString = Encoding.UTF8.GetString(decryptedBytes);

                return JsonUtility.FromJson<T>(decryptedString);
            }
            else
            {
                string JSONData = File.ReadAllLines(filePath)[0];
                return JsonUtility.FromJson<T>(JSONData);
            }
        }
        else
        {
            Debug.LogError($"Error file does not exist '{filePath}");
            return default(T);
        }
    }

    private static byte[] XOR(byte[] input, byte[] key)
    {
        byte[] output = new byte[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            // '^' is basically the XOR symbol
            // this is where we loop through each byte of the input and the key and XOR to encrypt it
            output[i] = (byte)(input[i] ^ key[i % key.Length]);
        }

        return output;
    }
}

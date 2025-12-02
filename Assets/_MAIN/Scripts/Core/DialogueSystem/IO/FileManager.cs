using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class FileManager
{
    private const string KEY = "VERYSCRETKEY";
    private static readonly byte[] HMACKey = Encoding.UTF8.GetBytes("MORESUPERSECRETKEY123!");
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

    public static void Save(string filePath, string JSONData, bool encrypt = false, bool useHMAC = true)
    {
        if (!TryCreateDirectoryFromPath(filePath))
        {
            Debug.LogError($"Failed to save file {filePath}");
            return;
        }

        byte[] dataBytes = Encoding.UTF8.GetBytes(JSONData);

        if (encrypt)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(KEY);
            dataBytes = XOR(dataBytes, keyBytes);
        }

        if (useHMAC)
        {
            byte[] hmacBytes = ComputeHMAC(dataBytes);

            byte[] finalBytes = new byte[hmacBytes.Length + dataBytes.Length];
            Buffer.BlockCopy(hmacBytes, 0, finalBytes, 0, hmacBytes.Length);
            Buffer.BlockCopy(dataBytes, 0, finalBytes, hmacBytes.Length, dataBytes.Length);

            File.WriteAllBytes(filePath, finalBytes);
        }
        else
        {
            // Save without HMAC
            // File.WriteAllBytes(filePath, dataBytes);
            StreamWriter sw = new StreamWriter(filePath);
            sw.Write(JSONData);
            sw.Close();
        }

        Debug.Log($"Saved at {filePath}");

    }

    public static T Load<T>(string filePath, T defaultInstance, bool encrypt = false, bool useHMAC = true)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file not found. Creating a new one...");
            Save(filePath, JsonUtility.ToJson(defaultInstance), encrypt, useHMAC);
            return defaultInstance;
        }

        try
        {
            if (!encrypt && !useHMAC)
            {
                // Plain text file
                string readData = File.ReadAllText(filePath);
                return JsonUtility.FromJson<T>(readData);
            }

            // Encrypted / HMAC file
            byte[] allBytes = File.ReadAllBytes(filePath);
            int hmacLength = 32;

            if (allBytes.Length < hmacLength && useHMAC)
                throw new Exception("Save file too small/corrupted.");

            byte[] dataBytes;

            if (useHMAC)
            {
                byte[] hmacBytes = new byte[hmacLength];
                dataBytes = new byte[allBytes.Length - hmacLength];

                Buffer.BlockCopy(allBytes, 0, hmacBytes, 0, hmacLength);
                Buffer.BlockCopy(allBytes, hmacLength, dataBytes, 0, dataBytes.Length);

                if (!VerifyHMAC(dataBytes, hmacBytes))
                    throw new Exception("HMAC verification failed.");
            }
            else
            {
                dataBytes = allBytes; // no HMAC, just the raw bytes
            }

            if (encrypt)
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(KEY);
                dataBytes = XOR(dataBytes, keyBytes);
            }

            string JSONData = Encoding.UTF8.GetString(dataBytes);
            return JsonUtility.FromJson<T>(JSONData);
        }
        catch (Exception e)
        {
            Debug.LogError($"Save file error: {e.Message}. Using default instance...");
            Save(filePath, JsonUtility.ToJson(defaultInstance), encrypt, useHMAC);
            return defaultInstance;
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

    private static byte[] ComputeHMAC(byte[] data)
    {
        using (var hmac = new HMACSHA256(HMACKey))
        {
            return hmac.ComputeHash(data);
        }
    }

    private static bool VerifyHMAC(byte[] data, byte[] hmacToCheck)
    {
        byte[] computed = ComputeHMAC(data);
        if (computed.Length != hmacToCheck.Length) return false;

        for (int i = 0; i < computed.Length; i++)
        {
            if (computed[i] != hmacToCheck[i])
                return false;
        }
        return true;
    }

}

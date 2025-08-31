using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager
{
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
}

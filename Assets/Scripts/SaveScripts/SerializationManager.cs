using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SerializationManager
{
    public static void Save(string saveName, object data)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        string directoryPath = Application.persistentDataPath + "/saves";
        if (!Directory.Exists(directoryPath)) {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = directoryPath + "/" + saveName + ".save";
        FileStream file = File.Create(filePath);

        formatter.Serialize(file, data);
        file.Close();
    }

    public static object Load(string saveName)
    {
        string path = Application.persistentDataPath + "/saves/" + saveName + ".save";
        if (!File.Exists(path))
            return null;

        BinaryFormatter formatter = GetBinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            object data = formatter.Deserialize(file);
            file.Close();
            return data;
        }
        catch
        {
            Debug.LogError("Failed to load file at" + path);
            file.Close();
            return null;
        }
    }

    private static BinaryFormatter GetBinaryFormatter()
    {
        return new BinaryFormatter();
    }
}

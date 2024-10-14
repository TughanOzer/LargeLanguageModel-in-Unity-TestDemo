using System.IO;
using UnityEngine;

public class JsonReader : MonoBehaviour {
    //public string filePath = "path/placeholderFile.json";

    public string ReadJsonFile(string filePath) {
        string jsonContent = string.Empty;

        try {
            TextAsset mytxtData=(TextAsset)Resources.Load(filePath);
            jsonContent = mytxtData.text;
            //jsonContent = File.ReadAllText(filePath);
        }
        catch (IOException e) {
            Debug.LogError("Error reading JSON file: " + e.Message);
        }

        return jsonContent;
    }
}


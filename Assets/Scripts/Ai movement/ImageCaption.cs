using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ImageCaptionGenerator : MonoBehaviour {
    private string apiUrl = "http://localhost:5001/sdapi/v1/interrogate";

    [System.Serializable]
    public class RequestBody {
        public string image;
        public string model = "clip";
    }

    [System.Serializable]
    public class ResponseBody {
        public string caption;
    }

    public void Start() {
        string imagePath = "Place/holder.jpg"; // Replace image path with camera output
        string base64ImageData = ImageToBase64.ConvertImageToBase64(imagePath);
        StartCoroutine(GenerateCaption(base64ImageData));
    }

    IEnumerator GenerateCaption(string base64ImageData) {
        RequestBody requestBody = new RequestBody {
            image = base64ImageData
        };
        string jsonData = JsonUtility.ToJson(requestBody);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            string responseText = request.downloadHandler.text;
            ResponseBody responseBody = JsonUtility.FromJson<ResponseBody>(responseText);
            Debug.Log("Caption: " + responseBody.caption);
        }
        else {
            Debug.LogError("Error: " + request.error);
        }
    }
}

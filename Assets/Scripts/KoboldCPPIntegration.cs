using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

public class KoboldCPPIntegration : MonoBehaviour {

    private string apiUrl = "http://localhost:5001/api/v1/generate";
    [SerializeField] TextMeshProUGUI textMeshProText;
    [SerializeField] TMP_InputField userInput;

    static string personalityPrompt, promptUserInput, historyText;
    static string assistantIntro = "<|eot_id|><|start_header_id|>assistant<|end_header_id|>"; static string userIntro = "<| start_header_id |> user <| end_header_id |>"; static string conversationOutro = "<|eot_id|>";
    List<string> history = new List<string>();

    [System.Serializable]
    private class RequestBody {
        public int max_context_length = 8048;
        public int max_length = 500;
        public string prompt = personalityPrompt + conversationOutro + userIntro + historyText + promptUserInput + assistantIntro;
        public bool quiet = false;
        public float rep_pen = 1.1f;
        public int rep_pen_range = 256;
        public float rep_pen_slope = 1;
        public float temperature = 0.5f;
        public float tfs = 1;
        public int top_a = 0;
        public int top_k = 100;
        public float top_p = 0.9f;
        public float typical = 1;
    }

    JsonReader jsonReader;
    private void Awake() {
        jsonReader = gameObject.GetComponent<JsonReader>();
    }

    public void StartGeneration() {
        personalityPrompt = jsonReader.ReadJsonFile("SystemPrompt");
        promptUserInput = userInput.text;
        history.Insert(0, userIntro + userInput.text + conversationOutro + Environment.NewLine);
        historyText = outputTMP;
        StartCoroutine(GenerateText());
    }

    [System.Serializable]
    public class ResponseBody {
        public Result[] results;
    }

    [System.Serializable]
    public class Result {
        public string text;
    }

    // Send request to local KoboldCPP API and return answer
    IEnumerator GenerateText() {
        RequestBody requestBody = new RequestBody();
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
            //Debug.Log("Generated Text: " + responseBody.results[0].text);
            AiOutput(responseBody.results[0].text);
        }
        else {
            Debug.LogError("Error: " + request.error);
        }
    }

    // Displays reponse in a textbox and feeds response to history array, so the AI can see past conversations.
    string outputTMP;
    public void AiOutput(string text) {
        history.Insert(0, assistantIntro + text + conversationOutro + Environment.NewLine);
        outputTMP = string.Join("> ", history);
        string cleanText = outputTMP.Replace(assistantIntro, "")
                                        .Replace(userIntro, "")
                                        .Replace(conversationOutro, "\n")
                                        .Trim();
        textMeshProText.text = cleanText;
    }
}

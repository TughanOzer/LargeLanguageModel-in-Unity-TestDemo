using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AIMovement : MonoBehaviour {
    private string apiUrl = "http://localhost:5001/api/v1/generate";
    static public string personalityPrompt;
    static string promptUserInput = "Movement Finished, take your next action.";
    static string assistantIntro = "<|eot_id|><|start_header_id|>assistant<|end_header_id|>";
    static string userIntro = "<|start_header_id|>user<|end_header_id|>";
    static string conversationOutro = "<|eot_id|>";

    [System.Serializable]
    public class RequestBody {
        public int max_context_length = 8048;
        public int max_length = 500;
        public string prompt = personalityPrompt + conversationOutro + userIntro + promptUserInput + assistantIntro;
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
    FirstPersonController firstPersonController;

    void Start() {
        jsonReader = gameObject.GetComponent<JsonReader>();
        personalityPrompt = jsonReader.ReadJsonFile("SystemPromptCharacter");
        firstPersonController = GetComponent<FirstPersonController>();
        StartGeneration();
    }
    
    public void StartGeneration() {
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
            AiOutput(responseBody.results[0].text);
        }
        else {
            Debug.LogError("Error: " + request.error);
        }
    }

    void AiOutput(string text) {
        string cleanedUpText = text.Replace("<|eot_id|><|start_header_id|>assistant<|end_header_id|>", "")
                                   .Replace("<|start_header_id|>user<|end_header_id|>", "")
                                   .Replace("<|eot_id|>", "")
                                   .Trim();
        StartCoroutine(HandleMovement(cleanedUpText));
    }

    public float waitBetweenActions = 1f;
    IEnumerator HandleMovement(string command) {
        switch (command.ToLower()) {
            case "forward":
                MoveForward(); 
                Debug.Log("Moving Forward");
                break;
            case "backward":
                MoveBackward();
                Debug.Log("Moving Backward");
                break;
            case "left":
                RotateCamera(-90); // Rotate camera 90 degrees to the left
                Debug.Log("Moving Left");
                break;
            case "right":
                RotateCamera(90); // Rotate camera 90 degrees to the right
                Debug.Log("Moving Right");
                break;
            case "jump":
                GetComponent<FirstPersonController>().Jump();
                break;
            default:
                Debug.LogWarning("Unknown command: " + command);
                break;
        }
        
        // Pause x seconds before triggering the next action
        yield return new WaitForSeconds(waitBetweenActions);
        StartGeneration();
    }

    void RotateCamera(float angle) {
        transform.Rotate(0, angle, 0);
    }


    [SerializeField] float moveSpeed = 5f;
    public Rigidbody rb;
    void MoveForward() {
        rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.deltaTime);
        Debug.Log("Moving Forward");
    }

    void MoveBackward() {
        rb.MovePosition(rb.position - transform.forward * moveSpeed * Time.deltaTime);
        Debug.Log("Moving Backward");
    }
}


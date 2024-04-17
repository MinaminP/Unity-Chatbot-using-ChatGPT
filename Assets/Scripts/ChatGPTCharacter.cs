using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Networking;

public class ChatGPTCharacter : MonoBehaviour
{
    private const string URL = "https://eduverseadmin.fxwebapps.com/api/public/custom-setting?key=AI_AVATAR_DEMO_SETTING";

    [TextArea(15, 20)]
    public string characterPrompt;

    [SerializeField] private ChatGPTMultipleCharacter gptManager;

    public void SetCharacterPrompt()
    {
        var message = new ChatMessage
        {
            Role = "user",
            Content = characterPrompt
        };

        gptManager.messages.Add(message);
        Debug.Log(message.Content);
    }

    IEnumerator GetPromptData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(URL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                SimpleJSON.JSONNode promptJS = SimpleJSON.JSON.Parse(json);

                characterPrompt = promptJS["data"]["value"];

                var message = new ChatMessage
                {
                    Role = "user",
                    Content = characterPrompt
                };

                gptManager.messages.Add(message);
                Debug.Log(message.Content);
            }
        }
    }
}

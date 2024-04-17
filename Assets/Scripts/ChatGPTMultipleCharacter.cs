using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.UI;

public class ChatGPTMultipleCharacter : MonoBehaviour
{
    private OpenAIApi openAI = new OpenAIApi(Credentials.OPENAI_API_KEY);
    public List<ChatMessage> messages = new List<ChatMessage>();
    public InputField input;

    public string newText;

    public ChatUIManager CU;

    public AzureSpeaker azureSpeaker;

    public ClickHandler clickHandler;

    [SerializeField] private ChatGPTCharacter[] gptCharacter;

    private bool isInitialCharacter = true;

    public AnimationHandler animHandler;

    // Start is called before the first frame update
    void Start()
    {
        ChangePrompt(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AskChatGPT();
        }
    }

    public void SetCharacter()
    {
        isInitialCharacter = !isInitialCharacter;

        if (isInitialCharacter)
        {
            ChangePrompt(0);
        }
        else
        {
            ChangePrompt(1);
        }
    }

    public void ChangePrompt(int index)
    {
        messages.Clear();
        CU.DeleteChat();
        gptCharacter[index].SetCharacterPrompt();
    }

    public async void AskChatGPT()
    {
        if (input.text == "") return;

        animHandler.SetAnimationListening(false);

        CU.loadingObject.SetActive(true);

        clickHandler.StopRecord();

        newText = input.text;

        CU.AppendUser(newText);
        CU.EnableChatButtons(false);
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = newText;
        newMessage.Role = "user";
        messages.Add(newMessage);

        input.text = "";

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";

        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);

            Debug.Log(chatResponse.Content);

            azureSpeaker.Text = chatResponse.Content;
            azureSpeaker.Speak();

        }
    }
}

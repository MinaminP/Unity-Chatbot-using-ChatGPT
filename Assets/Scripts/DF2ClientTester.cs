using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Syrus.Plugins.DFV2Client;
using UnityEngine.UI;
using LMNT;

public class DF2ClientTester : MonoBehaviour
{
	public InputField content;

	//public Text chatbotText;

	private DialogFlowV2Client client;

	//public LMNTSpeech LM;

	//public DialogueTriggerScript DT;

	public bool isDialogflowAvailable;

	public ChatGPTManager CM;

	public ChatUIManager CU;

	public AzureSpeaker azureSpeaker;

    // Start is called before the first frame update
    void Start()
    {
		client = GetComponent<DialogFlowV2Client>();

        // Adjustes session name if it is blank.
        string sessionName = GetSessionName();

        client.ChatbotResponded += LogResponseText;
		client.DetectIntentError += LogError;
		client.ReactToContext("DefaultWelcomeIntent-followup", 
			context => Debug.Log("Reacting to welcome followup"));
		client.SessionCleared += sess => Debug.Log("Cleared session ");
		client.AddInputContext(new DF2Context("userdata", 1, ("name", "George")), sessionName);

		Dictionary<string, object> parameters = new Dictionary<string, object>()
		{
			{ "name", "George" }
		};
		//client.DetectIntentFromEvent("test-inputcontexts", parameters, sessionName);
		CM = FindObjectOfType<ChatGPTManager>();
    }

	private void LogResponseText(DF2Response response)
	{
		Debug.Log(JsonConvert.SerializeObject(response, Formatting.Indented));
		Debug.Log(GetSessionName() + " said: \"" + response.queryResult.fulfillmentText + "\"");
		//chatbotText.text = response.queryResult.fulfillmentText;
		//LM.dialogue = response.queryResult.fulfillmentText;
		//DT.triggerSpeak();

		if (response.queryResult.fulfillmentText == "I didn't get that. Can you say it again?")
		{
			isDialogflowAvailable = false;
			CM.AskChatGPT();
		}
		else
		{
            //chatbotText.text = "Got the Answer";
            //LM.dialogue = response.queryResult.fulfillmentText;
			//DT.triggerSpeak();
			azureSpeaker.Text = response.queryResult.fulfillmentText;
			azureSpeaker.Speak();

            //CU.AppendAI(response.queryResult.fulfillmentText);

            isDialogflowAvailable = true;
		}

    }

	private void LogError(DF2ErrorResponse errorResponse)
	{
		Debug.LogError(string.Format("Error {0}: {1}", errorResponse.error.code.ToString(), 
			errorResponse.error.message));
	}

    
	public void SendText()
	{
		DF2Entity name0 = new DF2Entity("George", "George");
		DF2Entity name1 = new DF2Entity("Greg", "Greg");
		DF2Entity potion = new DF2Entity("Potion", "Potion", "Cure", "Healing potion");
		DF2Entity antidote = new DF2Entity("Antidote", "Antidote", "Poison cure");
		DF2EntityType names = new DF2EntityType("names", DF2EntityType.DF2EntityOverrideMode.ENTITY_OVERRIDE_MODE_SUPPLEMENT,
			new DF2Entity[] { name0, name1 });
		DF2EntityType items = new DF2EntityType("items", DF2EntityType.DF2EntityOverrideMode.ENTITY_OVERRIDE_MODE_SUPPLEMENT,
			new DF2Entity[] { potion, antidote });

        string sessionName = GetSessionName();
        client.AddEntityType(names, sessionName);
		client.AddEntityType(items, sessionName);

		client.DetectIntentFromText(content.text, sessionName);
		CU.AppendUser(content.text);
		//chatbotText.text = "Thinking";
		content.text = "";
	}


	public void SendEvent()
	{
        client.DetectIntentFromEvent(content.text,
			new Dictionary<string, object>(), GetSessionName());
	}

	public void Clear()
	{
        client.ClearSession(GetSessionName());
	}


    private string GetSessionName(string defaultFallback = "DefaultSession")
    {
        string sessionName = "";
        if (sessionName.Trim().Length == 0)
            sessionName = defaultFallback;
        return sessionName;
    }

    private void Update()
    {

    }
}

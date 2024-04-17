using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class FAQButton : MonoBehaviour
{
    [SerializeField] private ChatGPTManager gptManager;
    public InputField chatInput;

    public string[] faqText;
    public TooltipContent[] tooltipContent;

    public void Start()
    {
        StartCoroutine(GetFAQData());
    }

    IEnumerator GetFAQData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(Credentials.FAQ_URL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                SimpleJSON.JSONNode faqJS = SimpleJSON.JSON.Parse(json);

                for (int i = 0; i < faqText.Length; i++)
                {
                    faqText[i] = faqJS["data"]["faq"][i]["question"];

                    tooltipContent[i].description = faqText[i];
                }
            }
        }
    }

    public void AskFAQ(int i)
    {
        chatInput.text = faqText[i];
        gptManager.AskChatGPT();
    }
}
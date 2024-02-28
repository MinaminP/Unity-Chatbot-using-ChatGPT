using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FAQButton : MonoBehaviour
{
    [SerializeField] private ChatGPTManager gptManager;
    public string[] faqText;
    public InputField chatInput;
    public void AskFAQ(int i)
    {
        chatInput.text = faqText[i];
        gptManager.AskChatGPT();
    }
}
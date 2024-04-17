using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Michsky.MUIP;

public class ChatUIManager : MonoBehaviour
{
    [SerializeField] private InputField textInput;
    [SerializeField] private Button sendButton, speechButton;
    [SerializeField] private ButtonManager[] faqButtons;
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private RectTransform sent;
    [SerializeField] private RectTransform received;

    public GameObject loadingObject;

    private float height;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AppendUser(string text)
    {
        var chat = Instantiate(sent, scroll.content);
        chat.GetChild(0).GetChild(0).GetComponent<Text>().text = text;
        chat.anchoredPosition = new Vector2(0, -height);
        LayoutRebuilder.ForceRebuildLayoutImmediate(chat);
        height += chat.sizeDelta.y;
        scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        scroll.verticalNormalizedPosition = 0;
    }

    public void AppendAI(string text)
    {
        var chat = Instantiate(received, scroll.content);
        chat.GetChild(0).GetChild(0).GetComponent<Text>().text = text;
        chat.anchoredPosition = new Vector2(0, -height);
        LayoutRebuilder.ForceRebuildLayoutImmediate(chat);
        height += chat.sizeDelta.y;
        scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        scroll.verticalNormalizedPosition = 0;
    }

    public void DeleteChat()
    {
        for (var i = scroll.content.transform.childCount; i-- > 0;)
        {
            Destroy(scroll.content.transform.GetChild(i).gameObject);
        }
    }

    public void EnableChatButtons(bool isEnabled)
    {
        sendButton.interactable = isEnabled;
        speechButton.interactable = isEnabled;
        textInput.interactable = isEnabled;
        for (int i = 0; i < faqButtons.Length; i++)
        {
            faqButtons[i].Interactable(isEnabled);
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

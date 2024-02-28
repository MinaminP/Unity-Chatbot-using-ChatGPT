using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class ClickHandler : MonoBehaviour
{
    [SerializeField] InputField textField;
    private bool isRecording = false;
    private DictationRecognizer m_DictationRecognizer;
    [SerializeField] AnimationHandler animHandler;

    [SerializeField] private Sprite micSprite, recSprite;
    [SerializeField] private Button speechButton;
    // Start is called before the first frame update
    void Start()
    {
        m_DictationRecognizer = new DictationRecognizer();
        DictationCallbacks();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartRecord();
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            StopRecord();
        }
    }

    public void SetMicrophone()
    {
        isRecording = !isRecording;

        if (isRecording)
        {
            StartRecord();
        }
        else
        {
            StopRecord();
        }
    }

    public void StartRecord()
    {
        m_DictationRecognizer.Start();
        animHandler.SetAnimationListening(true);
        textField.text = "";
        speechButton.GetComponent<Image>().sprite = recSprite;
    }

    public void StopRecord()
    {
        isRecording = false;
        m_DictationRecognizer.Stop();
        animHandler.SetAnimationListening(false);
        speechButton.GetComponent<Image>().sprite = micSprite;
    }

    public void DictationCallbacks()
    {
        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Dictation result: {0}", text);
            textField.text = text;
        };

        /*m_DictationRecognizer.DictationHypothesis += (text) =>
        {
            Debug.LogFormat("Dictation hypothesis: {0}", text);
            textField.text += text;
        };*/

        m_DictationRecognizer.DictationComplete += (completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete)
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
        };

        m_DictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };
    }

}
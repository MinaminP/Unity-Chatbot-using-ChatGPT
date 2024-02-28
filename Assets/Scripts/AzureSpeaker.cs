using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Crosstales.RTVoice;
//using Crosstales.RTVoice.Model;
using System.Text.RegularExpressions;
using UnityEngine.UI;

using System.Threading;
using Microsoft.CognitiveServices.Speech;
using System;

public class AzureSpeaker : MonoBehaviour
{
    //public Voice speakerVoice;
    public AudioSource Audio;
    public string Text;
    //private string uid;
    //public bool UseNative;
    public AnimationHandler animHandler;
    public ChatUIManager CU;

    #region Microsoft Azure Native
    private const string SubscriptionKey = "";
    private const string Region = "";
    private const int SampleRate = 24000;

    private object threadLocker = new object();
    private bool waitingForSpeak;
    private bool audioSourceNeedStop;

    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioSource>();
        //Speaker.Instance.OnSpeakStart += speakStart;
        //Speaker.Instance.OnSpeakComplete += speakComplete;

        MicrosoftSpeechSetup();
    }
    void Update()
    {
        lock (threadLocker)
        {
            if (audioSourceNeedStop)
            {
                Audio.Stop();
                animHandler.SetAnimationTalking(false);
                CU.EnableChatButtons(true);
                Text = "";
                audioSourceNeedStop = false;
            }
        }
    }

    public void Speak()
    {
        /*if (UseNative)
        {
            uid = Speaker.Instance.SpeakNative(Text, speakerVoice);
        }
        else
        {
            uid = Speaker.Instance.Speak(Text, Audio, speakerVoice);
        }*/

        lock (threadLocker)
        {
            waitingForSpeak = true;
        }

        var startTime = DateTime.Now;

        // Starts speech synthesis, and returns once the synthesis is started.
        using (var result = synthesizer.StartSpeakingTextAsync(Text).Result)
        {
            // Native playback is not supported on Unity yet (currently only supported on Windows/Linux Desktop).
            // Use the Unity API to play audio here as a short term solution.
            // Native playback support will be added in the future release.
            var audioDataStream = AudioDataStream.FromResult(result);
            var isFirstAudioChunk = true;
            var audioClip = AudioClip.Create(
                "Speech",
                SampleRate * 600, // Can speak 10mins audio as maximum
                1,
                SampleRate,
                true,
                (float[] audioChunk) =>
                {
                    var chunkSize = audioChunk.Length;
                    var audioChunkBytes = new byte[chunkSize * 2];
                    var readBytes = audioDataStream.ReadData(audioChunkBytes);
                    if (isFirstAudioChunk && readBytes > 0)
                    {
                        var endTime = DateTime.Now;
                        var latency = endTime.Subtract(startTime).TotalMilliseconds;
                        isFirstAudioChunk = false;
                    }

                    for (int i = 0; i < chunkSize; ++i)
                    {
                        if (i < readBytes / 2)
                        {
                            audioChunk[i] = (short)(audioChunkBytes[i * 2 + 1] << 8 | audioChunkBytes[i * 2]) / 32768.0F;
                        }
                        else
                        {
                            audioChunk[i] = 0.0f;
                        }
                    }

                    if (readBytes == 0)
                    {
                        Thread.Sleep(200); // Leave some time for the audioSource to finish playback
                        audioSourceNeedStop = true;
                    }
                });
            Debug.Log("Speech Synthesis Voice Name : " + speechConfig.SpeechSynthesisVoiceName);
            Audio.clip = audioClip;
            Audio.Play();
            CU.AppendAI(Text);
            animHandler.SetAnimationTalking(true);
            CU.loadingObject.SetActive(false);
        }

        lock (threadLocker)
        {
            waitingForSpeak = false;
        }
    }

    public void MicrosoftSpeechSetup()
    {
        speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);

        speechConfig.SpeechSynthesisVoiceName = "en-SG-LunaNeural";
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);

        synthesizer = new SpeechSynthesizer(speechConfig, null);

        synthesizer.SynthesisCanceled += (s, e) =>
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);
        };
    }

    /*private void speakStart(Wrapper wrapper)
    {
        CU.AppendAI(Text);
        animHandler.SetAnimationTalking(true);
        loadingObject.SetActive(false);
    }

    private void speakComplete(Wrapper wrapper)
    {
        animHandler.SetAnimationTalking(false);
        CU.EnableChatButtons(true);
    }*/
}

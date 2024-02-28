using LMNT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LMNT
{

    public class DialogueTriggerScript : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private AudioSource audioSource;
        private LMNTSpeech speech;
        private bool triggered;
        public GameObject VoiceGameObject;

        void Start()
        {
            //animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            speech = GetComponent<LMNTSpeech>();
            //StartCoroutine(speech.Prefetch());
            triggered = false;
            VoiceGameObject = gameObject;
        }

        void Update()
        {
            /*if (Input.GetKeyDown("q") || Input.GetKeyDown("escape")) {
                Application.Quit();
            }*/

            if (!audioSource.isPlaying)
            {
                animator.SetBool("isTalking", false);
                triggered = false;
            }
            if (triggered)
            {
                return;
            }

            if (Input.GetKeyDown("return") || Input.GetKeyDown("enter"))
            {
                //StartCoroutine(speech.Talk());
            }

            if (audioSource.isPlaying)
            {
                animator.SetBool("isTalking", true);
                //animator.SetTrigger("Talk");
                triggered = true;
            }
        }

        public void triggerSpeak()
        {
            VoiceGameObject.GetComponent<AudioSource>().clip = null;
            StartCoroutine(speech.Prefetch());
            StartCoroutine(speech.Talk());
            //updateSpeaker();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAnimationTalking(bool talking)
    {
        animator.SetBool("isTalking", talking);
    }

    public void SetAnimationListening(bool listening)
    {
        animator.SetBool("isListening", listening);
    }
}

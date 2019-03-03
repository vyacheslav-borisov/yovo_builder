using UnityEngine;

public class Fader_SMB : StateMachineBehaviour
{
    private Fader _fader;
    private int _hashFadeOut;
    private int _hashFadeIn;

    private void Awake()
    {
        Debug.Log("Fader_SMB.Awake");

        _hashFadeOut = Animator.StringToHash("FadeOut");
        _hashFadeIn = Animator.StringToHash("FadeIn");
    }

    private bool _doOnce;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Debug.Log("Fader_SMB.OnStateEnter [" + animatorStateInfo + "]");

        if (_fader == null)
        {
            _fader = animator.GetComponent<Fader>();
        }

        _doOnce = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if(animatorStateInfo.shortNameHash == _hashFadeOut && animatorStateInfo.normalizedTime >= 1.0f)
        {
            if(_doOnce)
            {
                _doOnce = false;
                _fader.Event_OnFadeOutComplete();
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Debug.Log("Fader_SMB.OnStateExit [" + animatorStateInfo + "]");

        if (animatorStateInfo.shortNameHash == _hashFadeIn)
        {
            _fader.Event_OnFadeInComplete();
        }
    }
}

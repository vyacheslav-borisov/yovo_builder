using UnityEngine;
using UnityEngine.Events;

public class Fader : MonoBehaviour
{
    public UnityEvent OnFadeInComplete;
    public UnityEvent OnFadeOutComplete;

    private Animator _animator;
    private int _hashFadeOut;
    private int _hashFadeIn;
    private int _hashReset;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _hashFadeOut = Animator.StringToHash("FadeOut");
        _hashFadeIn = Animator.StringToHash("FadeIn");
        _hashReset = Animator.StringToHash("Reset");
    }

    public void FadeIn()
    {
        Debug.Log("Fader.FadeIn");

        _animator.SetTrigger(_hashFadeIn);
    }
    
    public void FadeOut()
    {
        Debug.Log("Fader.FadeOut");

        _animator.SetTrigger(_hashFadeOut);
    }

    public void Reset()
    {
        Debug.Log("Fader.Reset");

        _animator.SetTrigger(_hashReset);
    }

    public void Event_OnFadeInComplete()
    {
        Debug.Log("Fader.Event_OnFadeInComplete");

        if (OnFadeInComplete != null)
        {
            OnFadeInComplete.Invoke();
        }
    }

    public void Event_OnFadeOutComplete()
    {
        Debug.Log("Fader.Event_OnFadeOutComplete");

        if (OnFadeOutComplete != null)
        {
            OnFadeOutComplete.Invoke();
        }
    }
}

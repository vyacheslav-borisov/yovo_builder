using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GUIAnimator : MonoBehaviour
{
    public float hidingDelay = 1.0f;

    public UnityEvent OnShowGUIComplete;
    public UnityEvent OnHideGUIComplete;

    private Animator _animator;

    private int _hash_ShowGUI;
    private int _hash_ShowGUI_2;
    private int _hash_HideGUI;
    private int _hash_Reset;

    private SpriteButton[] _buttons;

    private void Awake()
    {
        Debug.Log(gameObject.name + " GUIAnimator Awake");

        _animator = GetComponent<Animator>();
        
        _hash_ShowGUI = Animator.StringToHash("ShowGUI");
        _hash_ShowGUI_2 = Animator.StringToHash("ShowGUI2");
        _hash_HideGUI = Animator.StringToHash("HideGUI");
        _hash_Reset = Animator.StringToHash("Reset");

        _buttons = gameObject.GetComponentsInChildren<SpriteButton>();
    }

    public virtual void ResetGUI()
    {
        Debug.Log("GUIAnimator.ResetGUI [" + gameObject.name + "]");

        _animator.SetTrigger(_hash_Reset);
    }

    public virtual void ShowGUI(int stage = 0)
    {
        Debug.Log("GUIAnimator.ShowGUI [" + gameObject.name + "]");

        if (stage == 0)
        {
            _animator.SetTrigger(_hash_ShowGUI);
        }else
        {
            _animator.SetTrigger(_hash_ShowGUI_2);
        }
    }

    public virtual void HideGUI()
    {
        Debug.Log("GUIAnimator.HideGUI [" + gameObject.name + "]");

        if (hidingDelay > 0.0f)
        {
            StartCoroutine(Coroutine_DelayHidding());
        }else
        {
            _animator.SetTrigger(_hash_HideGUI);
        }        
    }

    private IEnumerator Coroutine_DelayHidding()
    {
        yield return new WaitForSeconds(hidingDelay);

        _animator.SetTrigger(_hash_HideGUI);
    }

    public void EnableButtons()
    {
        foreach(var button in _buttons)
        {
            button.Interactable = true;
        }
    }

    public void DisableButtons()
    {
        foreach (var button in _buttons)
        {
            button.Interactable = false;
        }
    }

    public void Event_OnShowAnimComplete()
    {
        Debug.Log("GUIAnimator.Event_OnShowAnimComplete [" + gameObject.name + "]");

        if (OnShowGUIComplete != null)
        {
            OnShowGUIComplete.Invoke();
        }
    }

    public void Event_OnHideAnimComplete()
    {
        Debug.Log("GUIAnimator.Event_OnHideAnimComplete [" + gameObject.name + "]");

        if (OnHideGUIComplete != null)
        {
            OnHideGUIComplete.Invoke();
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.Events;

public class Detonator : SlideTrigger, ISceneElement
{
    private Animator _animator;
    private int      _hash_PushHand;

    public UnityEvent OnCaboom;

    public void SceneElement_Init()
    {
        _animator = GetComponent<Animator>();
        _hash_PushHand = Animator.StringToHash("PushHand");

        OnSlideStart.AddListener(PushHand);
    }

    public void SceneElement_Reset()
    {
        _animator.Rebind();
    }

    private void PushHand()
    {
        _animator.SetTrigger(_hash_PushHand);
    }

    public void Event_OnCaboom()
    {
        if(OnCaboom != null)
        {
            OnCaboom.Invoke();
        }
    }    
}

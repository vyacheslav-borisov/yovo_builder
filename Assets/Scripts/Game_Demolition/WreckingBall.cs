using System;
using UnityEngine;

public class WreckingBall : MonoBehaviour, ISceneElement
{
    private Animator _animator;

    private int _hashStart;
    private int _hashStartWreck;
    private int _hashReleaseWreck;

    public void SceneElement_Init()
    {
        _animator = transform.GetComponentInChildren<Animator>();

        _hashStart = Animator.StringToHash("Start");
        _hashStartWreck = Animator.StringToHash("Wreck_Start");
        _hashReleaseWreck = Animator.StringToHash("Wreck_Release");
    }

    public void SceneElement_Reset()
    {
        _animator.Rebind();        
    }

    public void ToEagle()
    {
        _animator.SetTrigger(_hashStart);
    }

    public void StartWreck()
    {
        Debug.Log("StartWreck");
        _animator.SetTrigger(_hashStartWreck);
    }

    public void ReleaseWreck()
    {
        Debug.Log("ReleaseWreck");
        _animator.SetTrigger(_hashReleaseWreck);
    }    
}

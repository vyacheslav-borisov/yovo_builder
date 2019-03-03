using System.Collections;
using UnityEngine;

public class BriefingWindowCharacter : BriefingCharacter
{
    private GameObject _attachPoint;
    private Animator _rootAnimator;
    private int _hashAppear;

    protected override void _onAwake()
    {
        _attachPoint = transform.Find("anim_moving_root").gameObject;
        _rootAnimator = _attachPoint.GetComponent<Animator>();
        _hashAppear = Animator.StringToHash("Appear");
    }

    protected override void _onEnable()
    {
        _animator.gameObject.transform.parent = _attachPoint.transform;
        _rootAnimator.SetTrigger(_hashAppear);        
    }

    public void EventBSM_OnAppear()
    {
        Debug.Log("BriefingWindowCharacter: onAppear");
        UnlockButtons();
    }
}

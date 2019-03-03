using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIAnimator_SMB : StateMachineBehaviour
{
    private GUIAnimator _guiScript;
    private int _hashShowGUI;
    private int _hashHideGUI;
    private int _hashDefault;

    private bool _doOnce;

    private void Awake()
    {
        _hashDefault = Animator.StringToHash("Default");
        _hashShowGUI = Animator.StringToHash("Show GUI");
        _hashHideGUI = Animator.StringToHash("Hide GUI");
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _doOnce = true;

        if (_guiScript == null)
        {
            _guiScript = animator.GetComponent<GUIAnimator>();            
        }
        
        if(stateInfo.shortNameHash == _hashDefault || stateInfo.shortNameHash == _hashHideGUI)
        {
            _guiScript.DisableButtons();
        }                
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if(animatorStateInfo.shortNameHash == _hashHideGUI)
        {
            _guiScript.Event_OnHideAnimComplete();
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if(animatorStateInfo.shortNameHash == _hashShowGUI && animatorStateInfo.normalizedTime >= 1.0f)
        {
            if(_doOnce)
            {
                _doOnce = false;
                _guiScript.EnableButtons();
                _guiScript.Event_OnShowAnimComplete();
            }
        }
    }
}

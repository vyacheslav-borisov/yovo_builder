using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireframeGameUI_SMB : StateMachineBehaviour
{
    private GUIAnimator _guiScript;

    private int _hashState_ShowGUI1;
    private int _hashState_ShowGUI2;
    private int _hashState_HideGUI1;
    private int _hashState_HideGUI2;

    private bool _doOnce = false;

    private void Awake()
    {
        _hashState_ShowGUI1 = Animator.StringToHash("ShowBeams");
        _hashState_ShowGUI2 = Animator.StringToHash("ShowTools");
        _hashState_HideGUI1 = Animator.StringToHash("HideBeams_2");
        _hashState_HideGUI2 = Animator.StringToHash("HideTools");
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        _guiScript = animator.GetComponent<GUIAnimator>();
        _doOnce = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if((animatorStateInfo.shortNameHash == _hashState_ShowGUI1 || animatorStateInfo.shortNameHash == _hashState_ShowGUI2)
            && animatorStateInfo.normalizedTime >= 1.0f)
        {
            if(_doOnce && _guiScript)
            {
                _guiScript.EnableButtons();
                _guiScript.Event_OnShowAnimComplete();
                _doOnce = false;
            }
        }        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if(animatorStateInfo.shortNameHash == _hashState_HideGUI1 || animatorStateInfo.shortNameHash == _hashState_HideGUI2)
        {
            if(_guiScript)
            {
                _guiScript.DisableButtons();
                _guiScript.Event_OnHideAnimComplete();
            }
        }
    }
}

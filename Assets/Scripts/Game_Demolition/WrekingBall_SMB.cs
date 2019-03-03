using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class WrekingBall_SMB : StateMachineBehaviour
{
    private SlideTrigger _button;
    private bool _doOnce = false;  

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("WrekingBallSMB OnStateEnter");

        if (!_button)
        {
            _button = animator.GetComponentInChildren<SlideTrigger>();
        }

        _doOnce = false;
        _button.Lock();        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        if(stateInfo.IsName("Wreck_Release_Floor0"))
        {
            WreckingBall_AET aet = animator.GetComponent<WreckingBall_AET>();
            if(aet)
            {
                aet.Event_OnWreckingFinished();
            }
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime > 1.0f && !_doOnce)
        {
            _doOnce = true;
            _button.Unlock();
        }
    }    
}

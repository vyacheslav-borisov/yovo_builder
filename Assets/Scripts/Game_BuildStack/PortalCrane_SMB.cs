using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCrane_SMB : StateMachineBehaviour
{
    private int _hashStatePutDown;
    private int _hashStatePutUp;
    private bool _doOnce = false;

    private void Awake()
    {
        _hashStatePutDown = Animator.StringToHash("PutDown");
        _hashStatePutUp = Animator.StringToHash("PutUp");
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        _doOnce = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (animatorStateInfo.shortNameHash == _hashStatePutUp)
        {
            var portalCrane = animator.GetComponentInParent<PortalCrane>();
            if (portalCrane != null)
            {
                portalCrane.Event_OnHookUpFinish();
            }            
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if(animatorStateInfo.shortNameHash == _hashStatePutDown
            && animatorStateInfo.normalizedTime >= 1.0f)
        {
            if (_doOnce)
            {
                _doOnce = false;
                var portalCrane = animator.GetComponentInParent<PortalCrane>();
                if(portalCrane != null)
                {
                    portalCrane.Event_OnHookDownFinish();
                }
            }
        }        
    }
}

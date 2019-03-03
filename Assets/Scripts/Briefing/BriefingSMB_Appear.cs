using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefingSMB_Appear : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        BriefingWindowCharacter character = animator.transform.parent.GetComponent<BriefingWindowCharacter>();
        character.EventBSM_OnAppear();
    }
}

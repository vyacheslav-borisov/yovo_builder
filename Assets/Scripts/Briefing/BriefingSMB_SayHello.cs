using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefingSMB_SayHello : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        BriefingTableCharacter character = animator.transform.parent.GetComponent<BriefingTableCharacter>();
        character.EventBSM_OnSayeHelloExit();        
    }
}

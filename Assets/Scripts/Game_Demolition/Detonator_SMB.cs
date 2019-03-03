using UnityEngine;

public class Detonator_SMB : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        var slider = animator.GetComponent<SlideTrigger>();
        if (slider)
        {
            if(animatorStateInfo.IsName("InstallDetonator"))
            {
                slider.Unlock();
            }else
            {
                slider.Lock();
            }
        }
    }
}

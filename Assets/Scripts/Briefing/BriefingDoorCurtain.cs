using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefingDoorCurtain : MonoBehaviour
{
    private Animator _animator;
    private int _hashClickCurtain;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _hashClickCurtain = Animator.StringToHash("ClickCurtain");
    }

    private void OnMouseDown()
    {
        _animator.SetTrigger(_hashClickCurtain);
    }
}

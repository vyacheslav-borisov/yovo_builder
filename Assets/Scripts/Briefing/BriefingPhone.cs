using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefingPhone : MonoBehaviour
{
    public float minPauseDuration = 3;
    public float maxPauseDuration = 6;
    public float ringDuration = 2;

    private Animator _animator;
    private int _ringOnHash;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _ringOnHash = Animator.StringToHash("RingOn");
    }

    private void OnEnable()
    {
        StartCoroutine(PhoneLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator PhoneLoop()
    {
        while(true)
        {
            float awaiting = Random.Range(minPauseDuration, maxPauseDuration);
            yield return new WaitForSeconds(awaiting);

            _animator.SetBool(_ringOnHash, true);
            yield return new WaitForSeconds(ringDuration);
            _animator.SetBool(_ringOnHash, false);
        }
    }	
}

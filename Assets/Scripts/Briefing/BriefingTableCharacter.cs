using System.Collections;
using UnityEngine;

public class BriefingTableCharacter : BriefingCharacter
{
    private int _hashSayHalloStart;
    private int _hashSayHallo;
    private int _hashSpeak;

    public float startDelay = 2.0f;
    public float otherCharAppearDelay = 15.0f;
    public float minSpeakAwait = 10.0f;
    public float maxSpeakAwait = 20.0f;

    private Coroutine awaintingCoroutine;

    protected override void _onEnable()
    {
        _hashSayHalloStart = Animator.StringToHash("SayHalloStart");
        _hashSayHallo = Animator.StringToHash("SayHallo");
        _hashSpeak = Animator.StringToHash("Speak");

        StartCoroutine(Coroutine_CharacterLoop());
        awaintingCoroutine = StartCoroutine(Coroutine_AwaitingActions());

        UnlockButtons();
    }

    protected override void _onDisable()
    {
        StopAllCoroutines();
    }

    protected override void _onMouseDown()
    {
        if (awaintingCoroutine != null)
        {
            StopCoroutine(awaintingCoroutine);
        }
        Debug.Log("window character canceled");
    }

    private void SayHello_Start()
    {
        Debug.Log("Character " + _animator.gameObject.name + ": SayHello_Start animation");
        _animator.SetTrigger(_hashSayHalloStart);
    }

    private void SayHello()
    {
        Debug.Log("Character " + _animator.gameObject.name + ": SayHello animation");
        _animator.SetTrigger(_hashSayHallo);
    }

    private void Speak()
    {
        Debug.Log("Character " + _animator.gameObject.name + ": Speak animation");
        _animator.SetTrigger(_hashSpeak);
    }

    private IEnumerator Coroutine_CharacterLoop()
    {
        yield return new WaitForSeconds(startDelay);

        SayHello_Start();

        yield return new WaitForSeconds(Random.Range(minSpeakAwait, maxSpeakAwait));

        while (true)
        {
            bool bHello =  (Random.Range(0, 100) % 2) == 0;
            if (bHello)
            {
                SayHello();
            }
            else
            {
                Speak();
            }

            yield return new WaitForSeconds(Random.Range(minSpeakAwait, maxSpeakAwait));
        }
    }

    private IEnumerator Coroutine_AwaitingActions()
    {
        yield return new WaitForSeconds(otherCharAppearDelay);

        Debug.Log("window character appearing...");
        otherCharacter.gameObject.SetActive(true);        
    }

    public void EventBSM_OnSayeHelloExit()
    {
        _quoteCloud.SetActive(true);
        UnlockButtons();
    }
}
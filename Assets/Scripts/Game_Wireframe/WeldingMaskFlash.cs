using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldingMaskFlash : MonoBehaviour, ISceneElement
{
    private SpriteRenderer _sprite;
    private bool _flashActive = false; 

    public void SceneElement_Init()
    {
        _sprite = GetComponent<SpriteRenderer>();        
    }

    public void SceneElement_Reset()
    {
        _flashActive = false;

        var color = _sprite.color;
        color.a = 0.0f;
        _sprite.color = color;        
    }

    private Coroutine _flashLoop = null;
    public void FlashOn()
    {
        if (!_flashActive && _flashLoop == null)
        {
            _flashActive = true;
            _flashLoop = StartCoroutine(Coroutine_FadeLoop(true, 10.0f));
        }
    }

    public void FlashOff()
    {
        if (_flashActive && _flashLoop == null)
        {
            _flashActive = false;
            _flashLoop = StartCoroutine(Coroutine_FadeLoop(false, 1.0f));
        }
    }

    private IEnumerator Coroutine_FadeLoop(bool fadein, float speed)
    {
        const float endAlpha = 0.9f;
        float elapsedTime = 0.0f;

        while (elapsedTime < 1.0f)
        {
            elapsedTime += Time.deltaTime * speed;

            var color = _sprite.color;

            if (fadein)
            {
                color.a = Mathf.Lerp(0.0f, endAlpha, elapsedTime);
            }
            else
            {
                color.a = Mathf.Lerp(0.0f, endAlpha, 1 - elapsedTime);
            }
            color.a = Mathf.Clamp01(color.a);

            _sprite.color = color;

            yield return null;
        }

        _flashLoop = null;
    }
}

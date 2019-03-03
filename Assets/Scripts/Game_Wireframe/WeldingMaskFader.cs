using System.Collections;
using UnityEngine;

public class WeldingMaskFader : MonoBehaviour, ISceneElement
{
    private SpriteRenderer _sprite;

    public void SceneElement_Init()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    public void SceneElement_Reset()
    {
        var color = _sprite.color;
        color.a = 0.0f;
        _sprite.color = color;
    }

    public void FadeIn()
    {
        StartCoroutine(Coroutine_FadeLoop(true));
    }

    public void FadeOut()
    {
        StartCoroutine(Coroutine_FadeLoop(false));
    }

    private IEnumerator Coroutine_FadeLoop(bool fadein)
    {
        const float endAlpha = 0.8f;
        const float speed = 2.0f;
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
    }
}

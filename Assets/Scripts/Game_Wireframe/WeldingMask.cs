using System.Collections;
using UnityEngine;

public class WeldingMask : MonoBehaviour, ISceneElement
{
    public WeldingMaskFader _fader;
    public WeldingMaskFlash _flash;

    private GameObject _coloredIcon = null;
    private bool _maskEquipped = false;

    public void SceneElement_Init()
    {
        var scaleNode = transform.Find("scale_node");
        if (scaleNode)
        {
            var child = scaleNode.Find("Arrow_2");
            if (child)
            {
                _coloredIcon = child.gameObject;
            }
        }
    }

    public void SceneElement_Reset()
    {
        _maskEquipped = false;
        if (_coloredIcon != null)
        {
            _coloredIcon.SetActive(false);
        }
    }

    public bool IsEquipped()
    {
        return _maskEquipped;
    }

    private Coroutine _blinkLoop = null;
    private Coroutine _scaleLoop = null;
    public void StartBlink()
    {
        StopBlinkLoops();

        if(!_maskEquipped)
        {
            if (_coloredIcon != null)
            {
                _blinkLoop = StartCoroutine(Coroutine_Blink());
            }

            _scaleLoop = StartCoroutine(Coroutine_Scale());
        }
    }

    public void StopBlink()
    {
        if (!_maskEquipped)
        {
            StopBlinkLoops();

            if (_coloredIcon != null)
            {
                _coloredIcon.SetActive(false);
            }
        }
    }

    private void StopBlinkLoops()
    {
        if (_blinkLoop != null)
        {
            StopCoroutine(_blinkLoop);
            _blinkLoop = null;
        }

        if (_scaleLoop != null)
        {
            StopCoroutine(_scaleLoop);
            _scaleLoop = null;
        }

        transform.localScale = Vector3.one;

    }

    private IEnumerator Coroutine_Blink()
    {
        while(true)
        {
            _coloredIcon.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            _coloredIcon.SetActive(false);
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator Coroutine_Scale()
    {
        const float speed = 3.0f;

        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 0.9f;
        float ellapsedTime = 0.0f;
        float k = 0;
        while (true)
        {
            ellapsedTime += Time.deltaTime * speed;
            k = Mathf.Sin(ellapsedTime);
            k = Mathf.Abs(k);
            transform.localScale = Vector3.Lerp(startScale, endScale, k);

            yield return null;
        }
    }

    public void Equip()
    {
        _maskEquipped = !_maskEquipped;
        _coloredIcon.SetActive(_maskEquipped);

        if (_fader != null)
        {
            if (_maskEquipped)
            {
                _fader.FadeIn();
            }else
            {
                _fader.FadeOut();
            }
        }
    }

    public void Event_OnStartApplyWelding(ToolApplyZone zone)
    {
        if(!_maskEquipped && _flash != null)
        {
            _flash.FlashOn();
        }        
    }

    public void Event_OnStopApplyWelding(ToolApplyZone zone)
    {
        if (!_maskEquipped && _flash != null)
        {
            _flash.FlashOff();
        }
    }
}

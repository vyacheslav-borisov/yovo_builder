using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingToolButton : MonoBehaviour, ISceneElement
{
    private SpriteRenderer _lockIcon;
    private SpriteRenderer _toolIcon;
    private SpriteRenderer _colorIcon;


    public void SceneElement_Init()
    {
        var gameHost = GameFlowManager.Instance
            .GetScene(SceneId.GAME_FENCE_REPAIR) as Game_FenceRepair;
        
        if(gameHost != null)
        {
            gameHost.OnPaintToolsColorSelected += EventHandler_ColorSelectedEvent;
        }

        var lockIcon = transform.Find("lock_icon");
        var toolIcon = transform.Find("tool_icon");
        var colorIcon = transform.Find("tool_icon_color");

        if(lockIcon != null && lockIcon.gameObject.activeSelf)
        {
            _lockIcon = lockIcon.GetComponent<SpriteRenderer>();
        }

        if (toolIcon != null && toolIcon.gameObject.activeSelf)
        {
            _toolIcon = toolIcon.GetComponent<SpriteRenderer>();
        }

        if (colorIcon != null && colorIcon.gameObject.activeSelf)
        {
            _colorIcon = colorIcon.GetComponent<SpriteRenderer>();
        }
    }

    public void SceneElement_Reset()
    {
        var gameHost = GameFlowManager.Instance
            .GetScene(SceneId.GAME_FENCE_REPAIR) as Game_FenceRepair;

        if (gameHost != null && _colorIcon != null)
        {
            _colorIcon.color = gameHost.CurrentPaintToolsColor;            
        }        
    }

    private void EventHandler_ColorSelectedEvent(Color selectedColor)
    {
        if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(Coroutine_SelectedColorAnimation(selectedColor));        
    }

    private Coroutine _coroutine;
    private IEnumerator Coroutine_SelectedColorAnimation(Color selectedColor)
    {
        Vector3 prevColorIconScale, newColorIconScale;
        float toolIconPrevAlpha = 0.0f;
        float lockIconPrevAlpha = 0.0f;

        if (_toolIcon != null)
        {
            var color = _toolIcon.color;
            toolIconPrevAlpha = color.a;
            color.a = 0.0f;
            _toolIcon.color = color;
        }

        if (_lockIcon != null)
        {
            var color = _lockIcon.color;
            lockIconPrevAlpha = color.a;
            color.a = 0.0f;
            _lockIcon.color = color;
        }

        if(_colorIcon != null)
        {
            _colorIcon.color = selectedColor;

            prevColorIconScale = _colorIcon.transform.localScale;
            newColorIconScale = prevColorIconScale * 1.3f;

            float k = 0.0f;
            while(k < 1.0f)
            {
                k += Time.deltaTime;
                var scale = Vector3.Lerp(prevColorIconScale, newColorIconScale, k);
                _colorIcon.transform.localScale = scale;

                yield return null;
            }

            _colorIcon.transform.localScale = newColorIconScale;
            yield return null;

            k = 0.0f;
            while (k < 1.0f)
            {
                k += Time.deltaTime;
                var scale = Vector3.Lerp(newColorIconScale, prevColorIconScale, k);
                _colorIcon.transform.localScale = scale;

                yield return null;
            }

            _colorIcon.transform.localScale = prevColorIconScale;
        }

        if (_toolIcon != null)
        {
            var color = _toolIcon.color;
            color.a = toolIconPrevAlpha;
            _toolIcon.color = color;
        }

        if (_lockIcon != null)
        {
            var color = _lockIcon.color;
            color.a = lockIconPrevAlpha;
            _lockIcon.color = color;
        }
    }
}

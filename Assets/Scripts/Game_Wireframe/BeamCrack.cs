using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamCrack : ToolApplyZone, ISceneElement, IDamagableZone, IProgressContributor
{
    private GameObject _goForeground;
    private GameObject _goBackground;
    private SpriteRenderer _spriteForeground;
    private SpriteRenderer _spriteBackground;

    private Collider2D[] _colliders;
    private static ToolId[] _toolSet_Stage1 = null;
    private static ToolId[] _toolSet_Stage2 = null;
    private static ToolId[] _toolSet_Stage3 = null;

    public float Damage
    {
        set
        {
            var prevDamage = _damage;
            _damage = value;
            _damage = Mathf.Clamp(_damage, 0.0f, 100.0f);

            UpdateCrackView();
            AddGameProgress(prevDamage, _damage);
        }

        get
        {
            return _damage;
        }
    }

    public float ProgressContribution
    {
        get; set;
    }

    private float _damage;
    private short _stage;


    public void SceneElement_Init()
    {
        _goForeground = transform.Find("foreground").gameObject;
        _goBackground = transform.Find("background").gameObject;
        if (_goForeground) _spriteForeground = _goForeground.GetComponent<SpriteRenderer>();
        if (_goBackground) _spriteBackground = _goBackground.GetComponent<SpriteRenderer>();
        _colliders = GetComponents<Collider2D>();

        if(_toolSet_Stage1 == null)
        {
            _toolSet_Stage1 = new ToolId[2];
            _toolSet_Stage1[0] = ToolId.GAME_WIREFRAME_SIMPLE_BRUSH;
            _toolSet_Stage1[1] = ToolId.GAME_WIREFRAME_ELECTRIC_BRUSH;
        }

        if(_toolSet_Stage2 == null)
        {
            _toolSet_Stage2 = new ToolId[1];
            _toolSet_Stage2[0] = ToolId.GAME_WIREFRAME_WELDING;
        }

        if (_toolSet_Stage3 == null)
        {
            _toolSet_Stage3 = new ToolId[1];
            _toolSet_Stage3[0] = ToolId.TOOL_UNKNOWN;
        }
    }

    public void SceneElement_Reset()
    {
        _damage = 0.0f;
        _stage = 0;
        _allowedTools = _toolSet_Stage1;

        foreach (var collider_ in _colliders)
        {
            collider_.enabled = false; 
        }

        if (_goForeground) _goForeground.SetActive(false);
        if (_goBackground) _goBackground.SetActive(false);
    }

    public void Show()
    {
        if (_goForeground) _goForeground.SetActive(true);
        if (_goBackground) _goBackground.SetActive(true);

        if (_spriteForeground)
        {
            var color = _spriteForeground.color;
            color.a = 0.0f;
            _spriteForeground.color = color;
        }

        if (_spriteBackground)
        {
            var color = _spriteBackground.color;
            color.a = 0.0f;
            _spriteBackground.color = color;
        }

        foreach (var collider_ in _colliders)
        {
            collider_.enabled = true;
        }

        if (_spriteForeground)
        {
            StartCoroutine(Coroutine_Show());
        }
    }

    private IEnumerator Coroutine_Show()
    {
        float k = 0.0f;
        while (k < 1.0f)
        {
            k += Time.deltaTime;

            var color = _spriteForeground.color;
            color.a = k;
            _spriteForeground.color = color;

            yield return null;
        }
    }

    private void AddGameProgress(float prevDamage, float newDamage)
    {
        var delta = newDamage - prevDamage;
        var progress = ProgressContribution * (delta / 100.0f);
        GameFlowManager.Instance.AddGameProgress(SceneId.GAME_WIREFRAME, progress);
    }

    private void UpdateCrackView()
    {
        if(_damage < 50.0f)
        {
            float k = _damage / 50.0f;

            var foregroundSpriteColor = _spriteForeground.color;
            foregroundSpriteColor.a = (1 - k);
            _spriteForeground.color = foregroundSpriteColor;

            var backgroundColor = _spriteBackground.color;
            backgroundColor.a = k;
            _spriteBackground.color = backgroundColor;

            return;            
        }

        if (_damage >= 50.0f && _stage == 0)
        {
            _stage++;
            _allowedTools = _toolSet_Stage2;

            var myGame = GameFlowManager.Instance.GetScene(SceneId.GAME_WIREFRAME);
            if(myGame is Game_Wireframe)
            {
                var wireframeGame = myGame as Game_Wireframe;
                wireframeGame.AddClearedCrack(this);
            }

            return;
        }

        if(_damage >= 50.0f && _stage == 1)
        {
            float k = (_damage - 50.0f) / 50.0f;

            var backgroundColor = _spriteBackground.color;
            backgroundColor.a = (1 - k);
            _spriteBackground.color = backgroundColor;

            if(_damage >= 100.0f)
            {
                _stage++;
                _allowedTools = _toolSet_Stage3;
            }

            return;
        }
    }
}

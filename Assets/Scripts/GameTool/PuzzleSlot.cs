using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PuzzleSlot : ToolApplyZone, ISceneElement, IProgressContributor, IPuzzleSlot
{
    public PuzzleSlot[] dependsOn;
    public int difficultyLevel;

    protected SceneId _gameID;
    protected GameObject _frame;
    protected GameObject _beam;
    protected SpriteRenderer _beamSprite;
    protected Collider2D _collider;

    public bool _beamPlaced;

    public float ProgressContribution
    {
        get; set;
    }

    public PuzzleSlot(SceneId gameID)
    {
        _gameID = gameID;
    }

    public virtual void SceneElement_Init()
    {
        _collider = GetComponent<Collider2D>();
        _frame = transform.Find("frame").gameObject;
        _beam = transform.Find("real").gameObject;

        if (_beam)
        {
            _beamSprite = _beam.GetComponent<SpriteRenderer>();
        }
    }

    public virtual void SceneElement_Reset()
    {
        if (_collider)
        {
            _collider.enabled = !_beamPlaced;
        }

        if (_beamSprite)
        {
            var color = _beamSprite.color;
            color.a = _beamPlaced ? 1.0f : 0.0f;
            _beamSprite.color = color;
        }
    }

    public virtual void PlacePuzzle()
    {
        GameFlowManager.Instance.AddGameProgress(_gameID, ProgressContribution);

        if (_collider) _collider.enabled = false;
        if (_frame) _frame.SetActive(false);
        if (_beam) _beam.SetActive(true);

        if (_beamSprite)
        {
            var color = _beamSprite.color;
            color.a = 0.0f;
            _beamSprite.color = color;

            StartCoroutine(Coroutine_Show());
        }

        _beamPlaced = true;        
    }

    public float _blinkAlphaAmplitude = 0.5f;
    public float _blinkSpeed = 1.5f;
    public bool _blinkLoop = true;

    private Coroutine _blinkCoroutine;
    private bool _blinkStopSignal;

    public void StartBlink()
    {
        if (_beamPlaced)
        {
            return;
        }

        Debug.Log("PuzzleSlot.StartBlink");

        if (_blinkCoroutine == null)
        {
            if (_blinkLoop)
            {
                _blinkCoroutine = StartCoroutine(Coroutine_Blink_Loop());
            }
            else
            {
                _blinkCoroutine = StartCoroutine(Coroutine_Blink_Once());
            }
        }
    }

    public void StopBlink(bool force = false)
    {
        Debug.Log("PuzzleSlot.StopBlink");

        if (_blinkCoroutine != null)
        {
            if (force)
            {
                StopCoroutine(_blinkCoroutine);
                _blinkCoroutine = null;

                if (_beamSprite)
                {
                    var color = _beamSprite.color;
                    color.a = 0.0f;
                    _beamSprite.color = color;
                }
            }
            else
            {
                _blinkStopSignal = true;
            }
        }
    }

    private IEnumerator Coroutine_Blink_Once()
    {
        float alpha = 0.0f;
        float elapsedTime = 0.0f;
        float phi = 0.0f;

        while (elapsedTime < Mathf.PI)
        {
            elapsedTime += Time.deltaTime * _blinkSpeed;
            phi = Mathf.Clamp(elapsedTime, 0.0f, Mathf.PI);
            alpha = Mathf.Sin(phi) * _blinkAlphaAmplitude;
            alpha = Mathf.Abs(alpha);

            {
                var color = _beamSprite.color;
                color.a = alpha;
                _beamSprite.color = color;
            }

            yield return null;
        }

        {
            var color = _beamSprite.color;
            color.a = 0.0f;
            _beamSprite.color = color;
        }

        _blinkCoroutine = null;
    }

    private IEnumerator Coroutine_Blink_Loop()
    {
        float alpha = 0.0f;
        float elapsedTime = 0.0f;
        float phi = 0.0f;

        while (true)
        {
            elapsedTime += Time.deltaTime * _blinkSpeed;
            phi = elapsedTime % Mathf.PI;
            alpha = Mathf.Sin(phi) * _blinkAlphaAmplitude;
            alpha = Mathf.Abs(alpha);

            {
                var color = _beamSprite.color;
                color.a = alpha;
                _beamSprite.color = color;
            }

            if (_blinkStopSignal)
            {
                _blinkStopSignal = false;
                break;
            }

            yield return null;
        }

        while (phi < Mathf.PI)
        {
            phi += Time.deltaTime * _blinkSpeed;
            alpha = Mathf.Sin(phi) * _blinkAlphaAmplitude;
            alpha = Mathf.Abs(alpha);

            {
                var color = _beamSprite.color;
                color.a = alpha;
                _beamSprite.color = color;
            }

            yield return null;
        }

        {
            var color = _beamSprite.color;
            color.a = 0.0f;
            _beamSprite.color = color;
        }

        _blinkCoroutine = null;
    }

    private IEnumerator Coroutine_Show()
    {
        float k = 0.0f;
        while (k < 1.0f)
        {
            k += Time.deltaTime;

            var color = _beamSprite.color;
            color.a = k;
            _beamSprite.color = color;

            yield return null;
        }
    }
}

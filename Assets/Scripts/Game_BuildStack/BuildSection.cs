using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSection : MonoBehaviour, ISceneElement
{
    public int blockID;
    public float shift;

    public delegate void GameEvent();
    public GameEvent OnAppear;

    private GameObject _contour;
    private GameObject _actual;
    private GameObject _footing;

    private SpriteRenderer _actualSprite;

    public enum State
    {
        Innactive,
        Catch,
        Complete
    };

    public State CurrentState
    {
        get
        {
            return _currentState;
        }

        set
        {
            OnChangeState(_currentState, value);
            _currentState = value;
        }
    }
    private State _currentState;

    public void SceneElement_Init()
    {
        _contour = transform.Find("contour").gameObject;
        _actual = transform.Find("actual").gameObject;
        _footing = transform.Find("footing").gameObject;

        if(_actual != null)
        {
            _actualSprite = _actual.GetComponent<SpriteRenderer>();
        }
    }

    public void SceneElement_Reset()
    {
        _contour.SetActive(true);
        _actual.SetActive(true);

        if (_actualSprite != null)
        {
            var color = _actualSprite.color;
            color.a = 0.0f;
            _actualSprite.color = color;
        }
    }

    private void OnChangeState(State oldState, State newState)
    {
        if(newState == State.Innactive)
        {
            _footing.SetActive(false);
        }

        if (newState == State.Catch)
        {
            _footing.SetActive(true);
        }

        if (newState == State.Complete)
        {
            _footing.SetActive(false);

            StartCoroutine(Coroutine_AppearActual());
        }
    }

    IEnumerator Coroutine_AppearActual()
    {
        var color = _actualSprite.color;
        
        float k = 0;
        while(k < 1.0f)
        {
            k += Time.deltaTime;

            color.a = k;
            _actualSprite.color = color;

            yield return null;
        }

        color.a = 1.0f;
        _actualSprite.color = color;

        _contour.SetActive(false);

        EventHandler_OnAppear();
    }

    private void EventHandler_OnAppear()
    {
        if(OnAppear != null)
        {
            OnAppear();
        }
    }
}

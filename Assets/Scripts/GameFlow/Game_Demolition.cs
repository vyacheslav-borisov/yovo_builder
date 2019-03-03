using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Demolition : Scene
{
    private WreckingBall _wreckingBall;
    private GUIDecorator _gameUI;       //tool panel + back button;
    private GUIAnimator _savedBackButton;

    private int _stage;
    
    protected override void _OnAwake()
    {
        base._OnAwake();

        _wreckingBall = transform.Find("wrecking_ball").GetComponent<WreckingBall>();
                
        _gameUI = (GUIDecorator)sceneUI;
        if(_gameUI)
        {
            _savedBackButton = _gameUI._decoratedGUI;
        }
    }

    public override void OnFadeInComplete(GameFlowManager gfm)
    {
        base.OnFadeInComplete(gfm);

        if(_gameUI)
        {
            sceneUI = _gameUI;
            _gameUI._decoratedGUI = _savedBackButton;
        }                
    }


    public override void OnSceneStart(GameFlowManager gfm)
    {
        base.OnSceneStart(gfm);

        gfm.ResetGame(sceneID);
        _stage = 0;
    }

    public override bool OnSceneClose(GameFlowManager gfm)
    {
        /*
        if(gfm.GetGameProgress(sceneID) < 200.0f)
        {
            //TODO show fail screen
            return false;
        }*/

        return true;
    }

    public override void OnGameProgress(GameFlowManager gfm, float progress)
    {
        Debug.Log(gameObject.name + " OnGameProgress " + progress + "%");

        if(progress >= 100.0f && _stage == 0)
        {
            //all floors cracked -> hide tools and show wreking ball
            _stage++;

            var tool = Tool.CurrentToolInUse;
            if(tool != null)
            {
                tool.ForceDrop();
            }

            if (_gameUI)
            {
                sceneUI = _savedBackButton;
                _gameUI._decoratedGUI = null;
                _gameUI.HideGUI(); //hide tools without back button
            }

            _wreckingBall.ToEagle();
        }                       
    }

    public void Event_OnWreckingFinished()
    {
        Debug.Log("Event_OnWreckingFinished");
        //building demolited compeletely

        StartCoroutine(Coroutine_OnGameWin(GameFlowManager.Instance));
    }

    private IEnumerator Coroutine_OnGameWin(GameFlowManager gfm)
    {
        gfm.ShowGameWinScreen();

        yield return new WaitForSeconds(3.0f);

        gfm.GoPreviousScene();
    }
}

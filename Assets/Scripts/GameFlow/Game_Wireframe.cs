using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_Wireframe : Scene
{
    public ProgressBar _progressBar;

    private int _stage = 0;
    private float _progressForStage1 = 0.0f;
    private float _progressForStage2 = 0.0f;

    private BeamFrame[] _beamFrames;
    private BeamCrack[] _beamCracks;
    private List<BeamCrack> _clearedCracks = new List<BeamCrack>();

    protected override void _OnAwake()
    {
        base._OnAwake();

        _beamFrames = GetComponentsInChildren<BeamFrame>(true);
        _beamCracks = GetComponentsInChildren<BeamCrack>(true);

        const float sceneItemProgressContribution = 10.0f;

        foreach (var beamFrame in _beamFrames)
        {
            beamFrame.ProgressContribution = sceneItemProgressContribution;
        }

        foreach (var beamCrack in _beamCracks)
        {
            beamCrack.ProgressContribution = sceneItemProgressContribution;
        }

        _progressForStage1 = sceneItemProgressContribution * _beamFrames.Length;
        _progressForStage2 = _progressForStage1 + (sceneItemProgressContribution * _beamCracks.Length);
    }

    public override void ShowToolHint(ToolId id)
    {
        foreach (var beamFrame in _beamFrames)
        {
            if(beamFrame.IsToolAllowed(id))
            {
                beamFrame.StartBlink();
            }           
        }
    }

    public override void StopToolHint(bool force = false)
    {
        foreach (var beamFrame in _beamFrames)
        {
            beamFrame.StopBlink(force);
        }
    }

    public void AddClearedCrack(BeamCrack crack)
    {
        _clearedCracks.Add(crack);

        if (_clearedCracks.Count == _beamCracks.Length)
        {
            var tool = Tool.CurrentToolInUse;
            if (tool != null)
            {
                tool.ForceDrop();
            }

            var electricBrush = Tool.GetById(ToolId.GAME_WIREFRAME_ELECTRIC_BRUSH);
            if (electricBrush != null)
            {
                electricBrush.IsToolActive = false;
            }

            var simpleBrush = Tool.GetById(ToolId.GAME_WIREFRAME_SIMPLE_BRUSH);
            if(simpleBrush != null)
            {
                simpleBrush.IsToolActive = false;
            }

            var welding = Tool.GetById(ToolId.GAME_WIREFRAME_WELDING);
            if(welding != null)
            {
                welding.IsToolActive = true;
            }
        }
    }

    public override void OnSceneStart(GameFlowManager gfm)
    {
        base.OnSceneStart(gfm);

        gfm.ResetGame(SceneId.GAME_WIREFRAME);
        _stage = 0;
        _clearedCracks.Clear();
    }

    public override void OnGameProgress(GameFlowManager gfm, float progress)
    {
        Debug.Log(gameObject.name + " OnGameProgress " + progress + "%");

        if (progress >= _progressForStage1 && _stage == 0)
        {
            //beams placed on frames - show tools, progress bar and welding mask button
            //hide beams UI
            _stage++;

            if (sceneUI)
            {
                sceneUI.ShowGUI();
            }

            return;
        }

        if (progress >= _progressForStage1 && _stage == 1)
        {
            if(_progressBar)
            {
                var stage2Progress = (progress - _progressForStage1) * 100.0f / (_progressForStage2 - _progressForStage1);
                _progressBar.Progress = stage2Progress;
            }            
        }

        if (progress >= _progressForStage2 && _stage == 1)
        {
            //all crackes decorated - game win
            _stage++;

            var tool = Tool.CurrentToolInUse;
            if (tool != null)
            {
                tool.ForceDrop();
            }

            StartCoroutine(Coroutine_OnGameWin(gfm));
            return;
        }
    }

    private IEnumerator Coroutine_OnGameWin(GameFlowManager gfm)
    {
        gfm.ShowGameWinScreen();

        yield return new WaitForSeconds(3.0f);

        gfm.GoPreviousScene();
    }
}

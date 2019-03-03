using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleToolButton : Tool
{
    private SceneId _hostID;
    private ToolApplyZone _currentZone;

    public delegate void PuzzleToolEvent(PuzzleToolButton puzzle);
    public PuzzleToolEvent OnPuzzlePlaced;
    public PuzzleToolEvent OnPuzzleFailed;

    public PuzzleToolButton(SceneId hostID)
    {
        _hostID = hostID;
    }

    protected override void _OnStartDrag()
    {
        base._OnStartDrag();

        if (_toolPrefabInstance != null)
        {
            ToolDragObject tdo = _toolPrefabInstance.GetComponent<ToolDragObject>();
            tdo.OnStartApply += OnStartApply;
            tdo.OnStopApply += OnStopApply;
        }

        var gameHost = GameFlowManager.Instance.GetScene(_hostID);
        if (gameHost != null)
        {
            gameHost.ShowToolHint(_toolId);
        }
    }

    protected override void _OnStopDrag()
    {
        if (_currentZone != null && _currentZone is PuzzleSlot)
        {
            var slot = _currentZone as PuzzleSlot;
            if(CheckDependeces(slot))
            {
                PlacePuzzle(slot);
            }else
            {
                ReturnPuzzleToPad();
            }            
        }
        else
        {
            ReturnPuzzleToPad();
        }
    }

    private void OnStartApply(ToolApplyZone zone)
    {
        _currentZone = zone;
    }

    private void OnStopApply(ToolApplyZone zone)
    {
        _currentZone = null;
    }

    private bool CheckDependeces(PuzzleSlot slot)
    {
        foreach(var depend in slot.dependsOn)
        {
            if(!depend._beamPlaced)
            {
                return false;
            }
        }

        return true;
    }

    private void PlacePuzzle(PuzzleSlot slot)
    {
        //TODO: перенести в делегат
        var gameHost = GameFlowManager.Instance.GetScene(_hostID);
        if (gameHost != null)
        {
            gameHost.StopToolHint(true);
        }

        DestroyPrevTool();
        ((IPuzzleSlot)slot).PlacePuzzle();

        gameObject.SetActive(false);

        if(OnPuzzlePlaced != null)
        {
            OnPuzzlePlaced(this);
        }
    }

    private void ReturnPuzzleToPad()
    {
        //TODO: перенести в делегат
        var gameHost = GameFlowManager.Instance.GetScene(_hostID);
        if (gameHost != null)
        {
            gameHost.StopToolHint(false);
        }

        if(OnPuzzleFailed != null)
        {
            OnPuzzleFailed(this);
        }

        base._OnStopDrag();
    }
}

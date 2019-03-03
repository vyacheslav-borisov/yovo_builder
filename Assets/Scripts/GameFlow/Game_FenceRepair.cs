using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Game_FenceRepair : Scene
{
    public enum Difficutly
    {
        HARD = 1,
        MIDDLE = 2,
        EASY = 3
    };

    public Difficutly difficutly;
    public GameObject fencePuzzlesHard;
    public GameObject fencePuzzlesMiddle;
    public GameObject fencePuzzlesEasy;

    private BrickPuzzleSlot[] _puzzleSlots;
    private SpriteScroller _spriteScroller;

    public delegate void ColorSelectedEvent(Color selectedColor);
    public ColorSelectedEvent OnPaintToolsColorSelected;

    public Color CurrentPaintToolsColor
    {
        get; private set;
    }

    protected override void _OnAwake()
    {
        base._OnAwake();

        _puzzleSlots = GetComponentsInChildren<BrickPuzzleSlot>();
        CurrentPaintToolsColor = Color.red;
    }

    public override void OnSceneStart(GameFlowManager gfm)
    {
        base.OnSceneStart(gfm);

        int diffLevel = (int)difficutly;
        foreach(var puzzle in _puzzleSlots)
        {
            puzzle._beamPlaced = puzzle.difficultyLevel < diffLevel;
            puzzle.SceneElement_Reset();
        }

        fencePuzzlesHard.SetActive(difficutly == Difficutly.HARD);
        fencePuzzlesMiddle.SetActive(difficutly == Difficutly.MIDDLE);
        fencePuzzlesEasy.SetActive(difficutly == Difficutly.EASY);

        switch(difficutly)
        {
            case Difficutly.EASY:
                _spriteScroller = fencePuzzlesEasy.GetComponent<SpriteScroller>();
                break;
            case Difficutly.MIDDLE:
                _spriteScroller = fencePuzzlesMiddle.GetComponent<SpriteScroller>();
                break;
            case Difficutly.HARD:
                _spriteScroller = fencePuzzlesHard.GetComponent<SpriteScroller>();
                break;
            default:
                _spriteScroller = null;
                break;
        }

        if(_spriteScroller != null)
        {
            var puzzles = _spriteScroller.GetComponentsInChildren<PuzzleToolButton>();
            foreach(var puzzle in puzzles)
            {
                puzzle.OnPuzzlePlaced += Event_OnPuzzlePlaced;
            }
        }

        sceneUI.ShowGUI();
    }

    private void Event_OnPuzzlePlaced(PuzzleToolButton puzzle)
    {
        if(puzzle is IScrollableItem && _spriteScroller != null)
        {
            var item = puzzle as IScrollableItem;
            _spriteScroller.RemoveItem(item);
        }

        bool allBeamsPlaced = true;
        foreach (var puzzleSlot in _puzzleSlots)
        {
            if(!puzzleSlot._beamPlaced)
            {
                allBeamsPlaced = false;
                break;
            }
        }

        if(allBeamsPlaced)
        {
            sceneUI.ShowGUI(1);
        }
    }

    public void SetPainToolsColor(string strColor)
    {
        Debug.Log("Game_FenceRepair.SetPainToolsColor[ " + strColor + " ]");

        uint nColor;
        var hex = strColor;

        if (hex.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase) ||
            hex.StartsWith("&H", StringComparison.CurrentCultureIgnoreCase))
        {
            hex = hex.Substring(2);
        }

        bool parsedSuccessfully = uint.TryParse(hex, NumberStyles.HexNumber, 
            CultureInfo.CurrentCulture, out nColor);

        if (parsedSuccessfully)
        {
            var a = (nColor >> 24) / 255.0f;
            var r = ((nColor & 0x00FF0000) >> 16) / 255.0f;
            var g = ((nColor & 0x0000FF00) >> 8) / 255.0f;
            var b = (nColor & 0x0000FF) / 255.0f;

            CurrentPaintToolsColor = new Color(r, g, b, a);

            if(OnPaintToolsColorSelected != null)
            {
                OnPaintToolsColorSelected(CurrentPaintToolsColor);
            }
        }
        else
        {
            Debug.LogWarning("parsing color failed: " + strColor);
        }        
    }

    public override void ShowToolHint(ToolId id)
    {
        foreach(var slot in _puzzleSlots)
        {
            if(slot.IsToolAllowed(id))
            {
                slot.StartBlink();
            }
        }                
    }

    public override void StopToolHint(bool force = false)
    {
        foreach (var slot in _puzzleSlots)
        {
            slot.StopBlink(force);
        }
    }
}

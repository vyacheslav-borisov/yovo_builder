using System;
using System.Collections;
using UnityEngine;

public class BeamFrame : PuzzleSlot
{
    private BeamCrack[] _cracks;

    public BeamFrame()
        : base(SceneId.GAME_WIREFRAME)
    {

    }

    public override void SceneElement_Init()
    {
        base.SceneElement_Init();

        _cracks = GetComponentsInChildren<BeamCrack>();                
    }

    public override void PlacePuzzle()
    {
        base.PlacePuzzle();

        foreach(var crack in _cracks)
        {
            crack.Show();
        }
    }    
}

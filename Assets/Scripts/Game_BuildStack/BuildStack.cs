using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildStack : MonoBehaviour, ISceneElement
{
    public BuildSection[] _sections;

    private int _currentSection;

    public int NumSections
    {
        get { return _sections.Length; }
    }

    public int CurrentSection
    {
        get { return _currentSection; }
    }

    public bool IsComplete
    {
        get
        {
            var isLast = _currentSection + 1 == _sections.Length;
            var isComplete = _sections[_currentSection].CurrentState == BuildSection.State.Complete;

            return isLast && isComplete;
        }
    }

    public void SceneElement_Init()
    {
        
    }

    public void SceneElement_Reset()
    {
        foreach(var section in _sections)
        {
            section.CurrentState = BuildSection.State.Innactive;
        }

        _currentSection = 0;
        _sections[_currentSection].CurrentState = BuildSection.State.Catch;
    }

    public void AdvanceToNextSection(out float shift)
    {
        shift = _sections[_currentSection].shift;

        _currentSection++;
        if(_currentSection < _sections.Length)
        {
            _sections[_currentSection].CurrentState = BuildSection.State.Catch;
        }
    }
}

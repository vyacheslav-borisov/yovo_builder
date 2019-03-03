using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GUIAnimator))]
public class CommonUI : MonoBehaviour
{
    private GUIAnimator _guiAnimator;

    private void Awake()
    {
        _guiAnimator = GetComponent<GUIAnimator>();
    }

    private void OnEnable()
    {
        _guiAnimator.ShowGUI();
    }
}

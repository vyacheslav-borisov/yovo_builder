using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamite : Tool
{
    public Animator _sceneDetonator;

    private int _hashShow;
    private int _hashHide;
    private int _hashInstall;

    protected override void _OnAwake()
    {
        _hashShow = Animator.StringToHash("Show");
        _hashHide =  Animator.StringToHash("Hide");
        _hashInstall = Animator.StringToHash("Install");
    }

    protected override void _OnStartDrag()
    {
        base._OnStartDrag();

        if (!GameFlowManager.Instance.IsToolLocked(_toolId) && _sceneDetonator)
        {
            _sceneDetonator.SetTrigger(_hashShow);            
        }

        if(_toolPrefabInstance != null)
        {
            ToolDragObject tdo = _toolPrefabInstance.GetComponent<ToolDragObject>();
            tdo.OnStartApply += OnApply; 
        }
    }

    protected override void _OnStopDrag()
    {
        base._OnStopDrag();

        if (!GameFlowManager.Instance.IsToolLocked(_toolId) && _sceneDetonator)
        {
            _sceneDetonator.SetTrigger(_hashHide);
        }
    }

    private void OnApply(ToolApplyZone zone)
    {
        DestroyPrevTool();

        _sceneDetonator.SetTrigger(_hashInstall);
        gameObject.SetActive(false);
    }
}

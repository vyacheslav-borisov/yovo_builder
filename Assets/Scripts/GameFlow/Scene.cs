using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneElement
{
    void SceneElement_Init();
    void SceneElement_Reset();
}

public interface IDamagableZone
{
    float Damage { get; set; }
}

public interface IPuzzleSlot
{
    void PlacePuzzle();
}

public interface IProgressContributor
{
    float ProgressContribution { get; set; }
}

public class Scene : MonoBehaviour
{
    public SceneId sceneID;
    public GUIAnimator sceneUI;
    public bool isGame;
    public Sprite gameIcon;

    public virtual void OnSceneStart(GameFlowManager gfm)
    {
        foreach (var sceneElement in _sceneElements)
        {
            sceneElement.SceneElement_Init();            
        }

        foreach (var sceneElement in _sceneElements)
        {
            sceneElement.SceneElement_Reset();
        }
    }

    public virtual bool OnSceneClose(GameFlowManager gfm) { return true; }

    public virtual void OnFadeInComplete(GameFlowManager gfm)
    {
        gfm.HideGameWinScreen();
    }

    public virtual void OnFadeOutComplete(GameFlowManager gfm) { }
    public virtual void OnGameProgress(GameFlowManager gfm, float progress) { }
    public virtual void ShowToolHint(ToolId id) { }
    public virtual void StopToolHint(bool force = false) { }

    private ISceneElement[] _sceneElements;

    protected virtual void _OnAwake()
    {

    }

    protected virtual void _OnEnable()
    {
        if (sceneUI != null)
        {
            sceneUI.gameObject.SetActive(true);
        }
    }

    protected virtual void _OnDisable()
    {
        if (sceneUI != null)
        {
            sceneUI.gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        _sceneElements = GetComponentsInChildren<ISceneElement>(true);

        /*foreach (var sceneElement in _sceneElements)
        {
            sceneElement.SceneElement_Init();
        }*/

        _OnAwake();
    }

    private void OnEnable()
    {
        /*foreach(var sceneElement in _sceneElements)
        {
            sceneElement.SceneElement_Init();
        }*/

        _OnEnable();
    }

    private void OnDisable()
    {
        _OnDisable();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStack: MonoBehaviour
{
    public Fader _fader;
    public bool _cloneScenes;

    protected Dictionary<SceneId, Scene> _sceneLookupTable = new Dictionary<SceneId, Scene>();

    private Stack<SceneId> _scenesStack = new Stack<SceneId>();
    private Scene _currentScene;
    private bool  _isAboutToQuit;

    public void InitSceneStack(Scene[] scenes)
    {
        _fader.OnFadeInComplete.RemoveAllListeners();
        _fader.OnFadeInComplete.AddListener(Event_OnFadeInComplete);
        _fader.OnFadeOutComplete.RemoveAllListeners();
        _fader.OnFadeOutComplete.AddListener(Event_OnFadeOutComplete);

        foreach (var scene in scenes)
        {
            var key = scene.sceneID;
            if (!_sceneLookupTable.ContainsKey(key))
            {
                _sceneLookupTable.Add(key, scene);
                scene.gameObject.SetActive(false);
            }
            else
            {
                Scene scene2 = _sceneLookupTable[key];
                Debug.LogError("Duplicated scene: " + key + ", gameObject " + scene.gameObject.name + " duplicated with " + scene2.gameObject.name);
            }
        }
    }

    public Scene GetScene(SceneId id)
    {
        if(_cloneScenes)
        {
            if(_currentScene && _currentScene.sceneID == id)
            {
                return _currentScene;
            }
        }

        if(_sceneLookupTable.ContainsKey(id))
        {
            return _sceneLookupTable[id];
        }

        return null;
    }

    public void GoNextScene(SceneId nextSceneId)
    {
        Debug.Log("SceneStack.GoNextScene [" + nextSceneId + "]");

        if (!_sceneLookupTable.ContainsKey(nextSceneId))
        {
            Debug.LogError("scene not setup in scenes list: " + nextSceneId);
            return;
        }

        bool startNewScene = false;

        if (_currentScene)
        {
            if (_currentScene.OnSceneClose(GameFlowManager.Instance))
            {
                startNewScene = true;

                GUIAnimator sceneUIAnimator = _currentScene.sceneUI;
                sceneUIAnimator.OnHideGUIComplete.RemoveAllListeners();
                sceneUIAnimator.OnHideGUIComplete.AddListener(_fader.FadeIn);
                sceneUIAnimator.HideGUI();
            }            
        }else
        {
            startNewScene = true;

            StartNewScene(nextSceneId);
        } 
        
        if(startNewScene)
        {
            if (_scenesStack.Count > 0)
            {
                var prevSceneId = _scenesStack.Peek();
                if (prevSceneId == nextSceneId)
                {
                    _scenesStack.Pop();
                }
            }

            _scenesStack.Push(nextSceneId);
        }       
    }

    public void GoPreviousScene()
    {
        Debug.Log("SceneStack.GoPreviousScene");

        if (_scenesStack.Count < 2)
        {
            Debug.LogWarning("flow stack does not contain previous scene!");
            return;
        }

        _scenesStack.Pop();

        if (_currentScene && _currentScene.OnSceneClose(GameFlowManager.Instance))
        {
            GUIAnimator gui = _currentScene.sceneUI;
            gui.OnHideGUIComplete.RemoveAllListeners();
            gui.OnHideGUIComplete.AddListener(_fader.FadeIn);
            gui.HideGUI();            
        }
    }

    private void StartNewScene(SceneId sceneId)
    {
        Debug.Log("start new scene: " + sceneId);

        var scene = _sceneLookupTable[sceneId];
        if (_cloneScenes)
        {
            var clonedSceneObject = GameObject.Instantiate(scene.gameObject);
            _currentScene = clonedSceneObject.GetComponent<Scene>();
        }else
        {
            _currentScene = scene;
        }

        _currentScene.gameObject.SetActive(true);
        _currentScene.OnSceneStart(GameFlowManager.Instance);

        _fader.FadeOut();
    }

    public void QuitGame()
    {
        _isAboutToQuit = true;

        if (_currentScene)
        {
            GUIAnimator gui = _currentScene.sceneUI;
            gui.OnHideGUIComplete.RemoveAllListeners();
            gui.OnHideGUIComplete.AddListener(_fader.FadeIn);
            gui.HideGUI();
        }
    }

    private void Event_OnFadeInComplete()
    {
        if (_isAboutToQuit)
        {
            Application.Quit();
            return;
        }

        if (_currentScene != null)
        {
            _currentScene.OnFadeInComplete(GameFlowManager.Instance);

            GUIAnimator gui = _currentScene.sceneUI;
            gui.OnHideGUIComplete.RemoveAllListeners();

            //Debug.Log(gui.gameObject.name + " listeners removed!");

            if (_cloneScenes)
            {
                Object.Destroy(_currentScene.gameObject);
            }else
            {
                _currentScene.gameObject.SetActive(false);
            }
            _currentScene = null;
        }

        if(_scenesStack.Count > 0)
        {
            StartNewScene(_scenesStack.Peek());
        }
    }

    private void Event_OnFadeOutComplete()
    {
        if(_currentScene)
        {
            _currentScene.OnFadeOutComplete(GameFlowManager.Instance);

            var sceneUIAnimator = _currentScene.sceneUI;
            sceneUIAnimator.ShowGUI();
        }        
    }
}

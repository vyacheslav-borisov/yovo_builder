using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : SceneStack
{
    public static GameFlowManager Instance
    {
        get; private set;
    }

    public static float ScreenAdaptableOffset
    {
        get; private set;
    }

    
    public uint refScreenWidth = 1423;
    public uint refScreenHeight = 800;
    public Scene[] scenes;
    public SceneId initialScene;

    public GameObject winScreen;

    private List<SceneId> _gamePool = new List<SceneId>();
    private Dictionary<SceneId, float> _gameProgress = new Dictionary<SceneId, float>();

    public GameFlowManager()
    {
        Instance = this;
    }

    private void Awake()
    {
        ComputeXPffset();
        InitSceneStack(scenes);

        foreach(var scene in scenes)
        {
            var key = scene.sceneID;
            if (scene.isGame && !_gameProgress.ContainsKey(key))
            {
                _gamePool.Add(key);
                _gameProgress.Add(key, 0.0f);
            }            
        }

        _gamePool.Shuffle();        
    }

    private void Start()
    {
        _fader.Reset();
        GoNextScene(initialScene);
    }
    
    private void ComputeXPffset()
    {
        float refAspectRatio = (refScreenWidth * 1.0f) / refScreenHeight;
        float curAspectRatio = (Screen.width * 1.0f) / Screen.height;
        ScreenAdaptableOffset = Mathf.Abs(refAspectRatio - curAspectRatio);

        Debug.Log("screen resoultion x offset = " + ScreenAdaptableOffset);

        /*
         * не работает для неактивных объектов, придеться извращаться через компоненты-наблюдатели
         * ScreenAdaptaleItem и глобально доступное свойство ScreenAdaptableOffset
        var screenAdaptableItems = GameObject.FindGameObjectsWithTag("ScreenAdaptableItem");
        foreach(var sai in screenAdaptableItems)
        {
            Debug.Log("adapt item: " + sai.name);
            Vector3 position = Vector3.zero;
            
            var parentTransfrom = sai.transform.parent;
            position.x = (parentTransfrom.position.x < 0.0f) ? xOffset : -xOffset;
            sai.transform.localPosition = position;
        }*/
    }

    public bool IsToolLocked(ToolId id)
    {
        //TODO: add code here
        return false;
    }

    public bool GetNextGame(ref SceneId gameID, ref Sprite gameIcon)
    {
        gameID = _gamePool[0];
        _gamePool.RemoveAt(0);
        _gamePool.Add(gameID);

        var scene = GetScene(gameID);
        gameIcon = (scene != null) ? scene.gameIcon : null;

        return true;
    }

    public void ResetGame(SceneId gameId)
    {
        if(_gameProgress.ContainsKey(gameId))
        {
            _gameProgress[gameId] = 0.0f;
        }
    }

    public void AddGameProgress(SceneId gameId, float gameProgress)
    {
        Debug.Log("game " + gameId + " progress: " + gameProgress + "%");

        if (_gameProgress.ContainsKey(gameId))
        {
            _gameProgress[gameId] += gameProgress;

            var scene = GetScene(gameId);
            if (scene)
            {
                scene.OnGameProgress(this, _gameProgress[gameId]);
            }
        }
    }

    public float GetGameProgress(SceneId gameId)
    {
        if (_gameProgress.ContainsKey(gameId))
        {
            return _gameProgress[gameId];
        }

        return 0.0f;
    }
    
    public void ShowGameWinScreen()
    {
        if(winScreen != null)
        {
            winScreen.SetActive(true);
        }
    }

    public void HideGameWinScreen()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(false);
        }
    }
}

public enum SceneId
{
    MAIN_MENU,
    BRIEFING,
    GAME_DEMOLITION,
    GAME_WIREFRAME,
    GAME_BUILD_STACK_1,
    GAME_BUILD_STACK_2,
    GAME_FENCE_REPAIR
}

public enum ToolId
{
    TOOL_UNKNOWN,
    GAME_DEMOLITION_SIMPLE_HUMMER,
    GAME_DEMOLITION_JACK_HUMMER,
    GAME_DEMOLITION_DINAMYTE,
    GAME_WIREFRAME_SIMPLE_BRUSH,
    GAME_WIREFRAME_ELECTRIC_BRUSH,
    GAME_WIREFRAME_WELDING,
    GAME_WIREFRAME_BEAM_1,
    GAME_WIREFRAME_BEAM_2,
    GAME_FENCE_REPAIR_BRICK_PUZZLE_1_1,
    GAME_FENCE_REPAIR_BRICK_PUZZLE_1_2,
    GAME_FENCE_REPAIR_BRICK_PUZZLE_1_3,
    GAME_FENCE_REPAIR_BRICK_PUZZLE_2_1,
    GAME_FENCE_REPAIR_BRICK_PUZZLE_2_2,
    GAME_FENCE_REPAIR_BRICK_PUZZLE_2_3,
    GAME_FENCE_REPAIR_BRICK_PUZZLE_3_1,
    GAME_FENCE_REPAIR_BRICK_PUZZLE_3_2,
    GAME_FENCE_REPAIR_BRICK_PUZZLE_5_1,
    GAME_FENCE_REPAIR_BRICK_PUZZLE_5_2,
    GAME_FENCE_REPAIR_TOP_BEAM,
    GAME_FENCE_REPAIR_PLASTERING_TROWEL,
    GAME_FENCE_REPAIR_COLOR_ROLLER,
    GAME_FENCE_REPAIR_COLOR_SPRAY
}

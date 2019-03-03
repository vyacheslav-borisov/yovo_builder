using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_BuildStack2 : Scene
{
    public enum BuildStage
    {
        FOUNDATION,
        MIDDLE,
        ROOF
    };

    public BuildStage CurrentState { get; set; }

    public PortalCrane _crane;

    public GameObject _foundationBlock;
    public GameObject _roofBlock;
    public GameObject[] _middleBlocks;
    public bool _randomizeBlocks;

    public Transform[] _layersToShift;
    [Range(0.1f, 2.0f)]
    public float _layersShiftSpeed = 1.0f;
    public float _craneFinalShift;
    public float _layersFinalShift;

    private float _nextShift;
    private float _currentShift;

    private GameObject _currentBlock;
    private List<GameObject> _middleBlocksRandomShuffled = new List<GameObject>();
    private List<Rigidbody2D> _completedBlocks = new List<Rigidbody2D>();

    protected override void _OnAwake()
    {
        base._OnAwake();

        if (_crane == null)
        {
            Debug.LogWarning("portal crane ref is not set");
            return;
        }

        _crane.OnDropBlock += EventHandler_OnDropBlock;
        _crane.OnCraneOnLeftPosition += EventHandler_HookOnLeftPosition;
        _crane.OnCraneOnRightPosition += EventHandler_HookOnRightPosition;

        if (_layersShiftSpeed == 0.0f)
        {
            Debug.LogWarning("layers shift speed is not setup!");
            _layersShiftSpeed = 1.0f;
        }

        _middleBlocksRandomShuffled.AddRange(_middleBlocks);
        if (_randomizeBlocks)
        {
            _middleBlocksRandomShuffled.Shuffle();
        }
    }

    public override void OnSceneStart(GameFlowManager gfm)
    {
        base.OnSceneStart(gfm);

        CurrentState = BuildStage.FOUNDATION;
        _currentShift = -_layersFinalShift;
        _nextShift = 0;

        _currentBlock = null;
        _completedBlocks.Clear();
    }


    public override void OnFadeOutComplete(GameFlowManager gfm)
    {
        base.OnFadeOutComplete(gfm);

        _crane.MoveHookLeft();
    }

    private void EventHandler_HookOnLeftPosition()
    {
        if (_currentBlock == null)
        {
            GenerateNextSection();
        }

        _crane.MoveHookRight();
    }

    private void EventHandler_HookOnRightPosition()
    {
        if (_currentBlock == null)
        {
            GenerateNextSection();
        }

        _crane.MoveHookLeft();
    }

    private void EventHandler_OnDropBlock()
    {
        _crane.Pause();
    }

    private void EventHandler_OnDropBlock_Success()
    {
        var rigidBody = _currentBlock.GetComponent<Rigidbody2D>();
        _completedBlocks.Add(rigidBody);

        _currentBlock.transform.parent = _layersToShift[0];
        _currentBlock = null;

        switch (CurrentState)
        {
            case BuildStage.FOUNDATION:
                CurrentState = BuildStage.MIDDLE;
                break;
            case BuildStage.MIDDLE:
                if(_completedBlocks.Count == (_middleBlocks.Length + 1))
                {
                    CurrentState = BuildStage.ROOF;
                }
                break;
            case BuildStage.ROOF:
                EventHandler_OnGameWin();
                return;

            default:
                break;
        }

        if (_nextShift > 0.0f)
        {
            StartCoroutine(Coroutine_ShiftDownLayers(_layersToShift, _nextShift, () => { _crane.Resume(); }));
        }
        else
        {
            _crane.Resume();
        }
    }

    private delegate void OnCoroutineComplete();
    private IEnumerator Coroutine_ShiftDownLayers(Transform[] layers, float shift,
                                                  OnCoroutineComplete eventHandler = null)
    {
        float totalTime = Mathf.Abs(shift) / _layersShiftSpeed;
        float sign = Mathf.Sign(shift);
        float elapsedTime = 0.0f;

        foreach(var rigidBody in _completedBlocks)
        {
            rigidBody.bodyType = RigidbodyType2D.Kinematic;
        }

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;

            foreach (var layer in layers)
            {
                var position = layer.localPosition;
                position.y -= sign * _layersShiftSpeed * Time.deltaTime;
                layer.localPosition = position;
            }

            yield return null;
        }

        foreach (var rigidBody in _completedBlocks)
        {
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
        }

        if (eventHandler != null)
        {
            eventHandler();
        }
    }

    private void EventHandler_OnDropBlock_Failed()
    {
        _currentBlock = null;
        GameFlowManager.Instance.GoNextScene(sceneID);
    }

    private void EventHandler_OnGameWin()
    {
        GameFlowManager.Instance.ShowGameWinScreen();

        StartCoroutine(Coroutine_ShiftDownLayers(new[] { _crane.transform }, -_craneFinalShift, () =>
        {
            StartCoroutine(Coroutine_ShiftDownLayers(_layersToShift, -_currentShift, () =>
            {
                GameFlowManager.Instance.GoPreviousScene();
            }));
        }));
    }

    private void GenerateNextSection()
    {
        if (_currentBlock != null)
        {
            Destroy(_currentBlock);
        }

        if (CurrentState == BuildStage.FOUNDATION)
        {
            _currentBlock = Instantiate(_foundationBlock);
        }

        if (CurrentState == BuildStage.MIDDLE)
        {
            var nextBlock = _middleBlocksRandomShuffled[0];
            _middleBlocksRandomShuffled.RemoveAt(0);
            _middleBlocksRandomShuffled.Add(nextBlock);
            _currentBlock = Instantiate(nextBlock);
        }

        if (CurrentState == BuildStage.ROOF)
        {
            _currentBlock = Instantiate(_roofBlock);
        }

        if (_currentBlock != null)
        {
            var carringBlock = _currentBlock.GetComponent<CarryingBuildBlock2>();
            if (carringBlock != null)
            {
                carringBlock.OnDropSuccess += EventHandler_OnDropBlock_Success;
                carringBlock.OnDropFailed += EventHandler_OnDropBlock_Failed;

                _nextShift = carringBlock.shift;
                _currentShift += _nextShift;
            }

            _crane.AttachBuildBlock(_currentBlock.transform);
        }
    }
}

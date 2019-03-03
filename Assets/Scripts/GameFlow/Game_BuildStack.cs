using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_BuildStack : Scene
{
    public enum BuildStage
    {
        FOUNDATION,
        MIDDLE,
        ROOF
    };

    public BuildStage CurrentState { get; set; }

    public PortalCrane _crane;
    public BuildStack _stack;

    public GameObject _foundationBlock;
    public GameObject _roofBlock;
    public GameObject[] _middleBlocks;

    public Transform[] _layersToShift;
    [Range(0.1f, 2.0f)]
    public float _layersShiftSpeed = 1.0f;
    public float _craneFinalShift;
    public float _layersFinalShift;

    private float _currentShift;

    private GameObject _currentBlock;
    private List<GameObject> _middleBlocksRandomShuffled = new List<GameObject>();

    protected override void _OnAwake()
    {
        base._OnAwake();

        if(_crane == null)
        {
            Debug.LogWarning("portal crane ref is not set");
            return;
        }

        _crane.OnDropBlock += EventHandler_OnDropBlock;
        _crane.OnCraneOnLeftPosition += EventHandler_HookOnLeftPosition;
        _crane.OnCraneOnRightPosition += EventHandler_HookOnRightPosition;

        CurrentState = BuildStage.FOUNDATION;

        if(_layersShiftSpeed == 0.0f)
        {
            Debug.LogWarning("layers shift speed is not setup!");
            _layersShiftSpeed = 1.0f;
        }

        _middleBlocksRandomShuffled.AddRange(_middleBlocks);
        _middleBlocksRandomShuffled.Shuffle();
    }

    public override void OnSceneStart(GameFlowManager gfm)
    {
        base.OnSceneStart(gfm);

        _currentShift = -_layersFinalShift;
    }


    public override void OnFadeOutComplete(GameFlowManager gfm)
    {
        base.OnFadeOutComplete(gfm);

        _crane.MoveHookLeft();
    }

    private void EventHandler_HookOnLeftPosition()
    {
        GenerateNextSection();
        _crane.MoveHookRight();
    }

    private void EventHandler_HookOnRightPosition()
    {
        GenerateNextSection();
        _crane.MoveHookLeft();
    }

    private void EventHandler_OnDropBlock()
    {
        _currentBlock = null;
        _crane.Pause();        
    }
    
    private void EventHandler_OnDropBlock_Success()
    {
        if(_stack.IsComplete)
        {
            StartCoroutine(Coroutine_ShiftDownLayers(new[] { _crane.transform }, -_craneFinalShift, () =>
            {
                StartCoroutine(Coroutine_ShiftDownLayers(_layersToShift, -_currentShift, () => {
                    GameFlowManager.Instance.GoPreviousScene();
                }));
            }));
            
            return;
        }

        float shift;
        _stack.AdvanceToNextSection(out shift);

        _currentShift += shift;

        if (_stack.CurrentSection + 1 == _stack.NumSections)
        {
            CurrentState = BuildStage.ROOF;            
        }else
        {
            CurrentState = BuildStage.MIDDLE;
        }

        if(shift > 0.0f)
        {
            StartCoroutine(Coroutine_ShiftDownLayers(_layersToShift, shift, () => { _crane.Resume(); }));
        }else
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

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;

            foreach(var layer in layers)
            {
                var position = layer.localPosition;
                position.y -= sign * _layersShiftSpeed * Time.deltaTime;
                layer.localPosition = position;
            }

            yield return null;
        }

        if(eventHandler != null)
        {
            eventHandler();
        }
    }
    
    private void EventHandler_OnDropBlock_Failed()
    {
        GameFlowManager.Instance.GoNextScene(sceneID);
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

        if(CurrentState == BuildStage.MIDDLE)
        {
            var nextBlock = _middleBlocksRandomShuffled[0];
            _middleBlocksRandomShuffled.RemoveAt(0);
            _middleBlocksRandomShuffled.Add(nextBlock);
            _currentBlock = Instantiate(nextBlock);
        }

        if(CurrentState == BuildStage.ROOF)
        {
            _currentBlock = Instantiate(_roofBlock);
        }

        if(_currentBlock != null)
        {
            var carringBlock = _currentBlock.GetComponent<CarryingBuildBlock>();
            if (carringBlock != null)
            {
                carringBlock.OnDropSuccess += EventHandler_OnDropBlock_Success;
                carringBlock.OnDropFailed += EventHandler_OnDropBlock_Failed;
            }

            _crane.AttachBuildBlock(_currentBlock.transform);
        }
    }
}

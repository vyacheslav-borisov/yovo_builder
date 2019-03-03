using System.Collections;
using UnityEngine;

public class PortalCrane : MonoBehaviour, ISceneElement
{
    public float movingSpeed = 0.5f;

    public delegate void EventHandler();

    public EventHandler OnCraneOnLeftPosition;
    public EventHandler OnCraneOnRightPosition;
    public EventHandler OnHookDownFinish;
    public EventHandler OnHookUpFinish;
    public EventHandler OnDropBlock;

    private Transform _leftLimitPoint;
    private Transform _rightLimitPoint;

    private Transform _carrier;
    private Transform _attacmentPoint;
    private Transform _attacment;

    private Animator _hookAnimator;
    private int _hashReset;
    private int _hashPutDown;
    private int _hashPutUp;   

    public void SceneElement_Init()
    {
        _leftLimitPoint = transform.Find("left_arrow_point");
        _rightLimitPoint = transform.Find("right_arrow_point");

        _carrier = transform.Find("carryer");
        _attacmentPoint = transform.Find("carryer/carryer/hook/attacment");
        _attacment = null;

        _hookAnimator = GetComponentInChildren<Animator>();
        _hashPutDown = Animator.StringToHash("PutDown");
        _hashPutUp = Animator.StringToHash("PutUp");
        _hashReset = Animator.StringToHash("Reset");
    }

    public void SceneElement_Reset()
    {
        if(_attacment != null)
        {
            _attacment.parent = null;
            _attacment = null;
        }
        
        if(_hookAnimator != null)
        {
            _hookAnimator.SetTrigger(_hashReset);
        }
        
        if(_movingCoroutine != null)
        {
            StopCoroutine(_movingCoroutine);
        }      
    }

    
    private Coroutine _movingCoroutine = null;
    private bool _dropAttacmentSignal = false;
    private bool _paused = false;
    
    private enum MoveDirection
    {
        MOVE_LEFT,
        MOVE_RIGHT        
    };

    private IEnumerator Coroutine_CraneMove(MoveDirection moveDirection)
    {
        Vector3 startPoint = (moveDirection == MoveDirection.MOVE_LEFT) ? _rightLimitPoint.position : _leftLimitPoint.position;
        Vector3 endPoint = (moveDirection == MoveDirection.MOVE_LEFT) ? _leftLimitPoint.position : _rightLimitPoint.position;
        float k = (_carrier.position - startPoint).magnitude / (endPoint - startPoint).magnitude;

        while (k < 1.0f)
        {
            k += Time.deltaTime * movingSpeed;
            _carrier.position = Vector3.Lerp(startPoint, endPoint, k);

            if(_dropAttacmentSignal)
            {
                _dropAttacmentSignal = false;
                _DropCarriedBlock();
                
                yield return new WaitForSeconds(1.0f);
            }

            while (_paused)
            {
                yield return new WaitForSeconds(0.5f);
            }

            yield return null;
        }

        _movingCoroutine = null;

        if (moveDirection == MoveDirection.MOVE_LEFT)
        {
            Event_OnCraneOnLeftPosition();
        }else
        {
            Event_OnCraneOnRightPosition();
        }       
    }      

    public void MoveHookLeft()
    {
        if(_movingCoroutine == null)
        {
            _movingCoroutine = StartCoroutine(Coroutine_CraneMove(MoveDirection.MOVE_LEFT));
        }else
        {
            Debug.LogWarning("crane already move!");
        }
    }

    public void MoveHookRight()
    {
        if (_movingCoroutine == null)
        {
            _movingCoroutine = StartCoroutine(Coroutine_CraneMove(MoveDirection.MOVE_RIGHT));
        }
        else
        {
            Debug.LogWarning("crane already move!");
        }
    }

    public void Pause()
    {
        if (_movingCoroutine != null)
        {
            _paused = true;
        }
    }

    public void Resume()
    {
        _paused = false;
    }

    public void PutDownHook()
    {
        if(_hookAnimator != null)
        {
            _hookAnimator.SetTrigger(_hashPutDown);
        }else
        {
            Debug.LogWarning("hook animator component not found!");
        }
    }

    public void PutUpHook()
    {
        if (_hookAnimator != null)
        {
            _hookAnimator.SetTrigger(_hashPutUp);
        }
        else
        {
            Debug.LogWarning("hook animator component not found!");
        }
    }

    public void AttachBuildBlock(Transform block)
    {
        if (_attacment != null)
        {
            _attacment.parent = null;
            _attacment = null;
        }

        _attacment = block;
        
        if(_attacment != null && _attacmentPoint != null)
        {
            _attacment.parent = _attacmentPoint;

            var hookNode = _attacment.Find("hook_node");
            if (hookNode != null)
            {
                var localPosition = -hookNode.localPosition;
                localPosition.z = -0.1f;
                _attacment.localPosition = localPosition;
            }
            else
            {
                var localPosition = Vector3.zero;
                localPosition.z = -0.1f;
                _attacment.localPosition = localPosition;
            }

            {
                var buildBlock = _attacment.GetComponent<CarryingBuildBlock>();
                if (buildBlock != null)
                {
                    buildBlock.OnAttach(this);
                }
            }

            {
                var buildBlock = _attacment.GetComponent<CarryingBuildBlock2>();
                if (buildBlock != null)
                {
                    buildBlock.OnAttach(this);
                }
            }
        }
        else
        {
            if (_attacment == null)
            {
                Debug.LogWarning("crane carrying block is null");
            }

            if(_attacmentPoint == null)
            {
                Debug.LogWarning("crane hook attachment node not found");
            }
        }
    }

    public Transform DettachBuildBlock()
    {
        if (_attacment != null)
        {
            var result = _attacment;

            _attacment.parent = null;
            _attacment = null;

            return result;
        }

        return null;
    }

    public void DropCarriedBlock()
    {
        if (_movingCoroutine != null)
        {
            _dropAttacmentSignal = true;
        }
    }

    public void _DropCarriedBlock()
    {
        if (_attacment != null)
        {
            _attacment.parent = null;

            {
                var buildBlock = _attacment.GetComponent<CarryingBuildBlock>();
                if (buildBlock)
                {
                    buildBlock.OnDrop();
                }
            }

            {
                var buildBlock = _attacment.GetComponent<CarryingBuildBlock2>();
                if (buildBlock)
                {
                    buildBlock.OnDrop();
                }
            }

            _attacment = null;

            Event_OnDropBlock();
        }
    }

    public void Event_OnCraneOnLeftPosition()
    {
        Debug.Log("Event_OnCraneOnLeftPosition");

        if(OnCraneOnLeftPosition != null)
        {
            OnCraneOnLeftPosition();
        }
    }

    public void Event_OnCraneOnRightPosition()
    {
        Debug.Log("Event_OnCraneOnRightPosition");

        if (OnCraneOnRightPosition != null)
        {
            OnCraneOnRightPosition();
        }
    }

    public void Event_OnHookDownFinish()
    {
        Debug.Log("Event_OnHookDownFinish");

        if (OnHookDownFinish != null)
        {
            OnHookDownFinish();
        }
    }

    public void Event_OnHookUpFinish()
    {
        Debug.Log("Event_OnHookUpFinish");

        if (OnHookUpFinish != null)
        {
            OnHookUpFinish();
        }
    }

    public void Event_OnDropBlock()
    {
        Debug.Log("Event_OnDropBlock");

        if(OnDropBlock != null)
        {
            OnDropBlock();
        }
    }
}

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class SpriteButton : MonoBehaviour
{
    public float _animationSpeed = 3.0f;
    public float _endScaleMultiplier = 0.9f;

    public UnityEvent OnClick;

    public bool Interactable
    {
        get
        {
            return _interactable;
        }
        set
        {
            _interactable = value;
        }
    }
    private bool _interactable = true;

    private Animator _animator;
    private Transform _scaleNode;

    private int _hashPointerClick;
    private int _hashPointerEnter;
    private int _hashPointerExit;

    private void Awake()
    {
        _hashPointerClick = Animator.StringToHash("PointerClick");
        _hashPointerEnter = Animator.StringToHash("PointerEnter");
        _hashPointerExit = Animator.StringToHash("PointerExit");

        _animator = GetComponent<Animator>();
        if(_animator == null)
        {
            _scaleNode = transform.Find("scale_node");
            if(_scaleNode == null)
            {
                _scaleNode = transform;
            }
        }
    }

    private void OnMouseDown()
    {
        if(!Interactable)
        {
            Debug.Log("Button " + gameObject.name + ": disabled");
            return;
        }

        Debug.Log("Button " + gameObject.name + ": OnMouseDown");

        PlayAnimation(_hashPointerClick);

        if(OnClick != null)
        {
            OnClick.Invoke();
        }
    }   
    

    private void OnMouseEnter()
    {
        if (!Interactable)
        {
            Debug.Log("Button " + gameObject.name + ": disabled");
            return;
        }
        Debug.Log("Button " + gameObject.name + ": OnMouseEnter");

        PlayAnimation(_hashPointerEnter);
    }

    private void OnMouseExit()
    {
        if (!Interactable)
        {
            Debug.Log("Button " + gameObject.name + ": disabled");
            return;
        }
        Debug.Log("Button " + gameObject.name + ": OnMouseExit");

        PlayAnimation(_hashPointerExit);
    }
    
    private void PlayAnimation(int animation)
    {
        if (_animator != null)
        {
            _animator.SetTrigger(animation);
        }else
        {
            if (animation == _hashPointerEnter || animation == _hashPointerClick)
            {
                if (_manualAnimationLoop == null)
                {
                    _manualAnimationLoop = StartCoroutine(Coroutine_ManualAnimation());
                }
            }
        }
    }

    private Coroutine _manualAnimationLoop = null;
    private IEnumerator Coroutine_ManualAnimation()
    {
        Vector3 startScale = Vector3.one;
        Vector3 endScale = startScale * _endScaleMultiplier;
        float ellapsedTime = 0.0f;
        float k = 0;

        while (ellapsedTime < Mathf.PI)
        {
            ellapsedTime += Time.deltaTime * _animationSpeed;
            k = Mathf.Sin(ellapsedTime);
            k = Mathf.Abs(k);
            _scaleNode.localScale = Vector3.Lerp(startScale, endScale, k);

            yield return null;
        }

        _manualAnimationLoop = null;
        _scaleNode.localScale = Vector3.one;
    }    
}

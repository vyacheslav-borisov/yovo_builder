using UnityEngine;
using System.Collections;
using System;

public class ToolDragObject : MonoBehaviour
{
    public ToolId ToolID
    {
        get; set;
    }

    protected Animator _animator;
    protected int _hashIsActive;

    protected ParticleSystem[] _emmiters;

    private Collider2D _toolCollider = null;
    private Collider2D[] _overlapResults = new Collider2D[3];
    private ContactFilter2D _filter = new ContactFilter2D();

    private ToolApplyZone _currentToolApplyZone = null;

    public delegate void Delegate(ToolApplyZone zone);
    public Delegate OnStartApply;
    public Delegate OnStopApply;

    protected virtual void _OnAwake() { }
    protected virtual void _OnStart() { }

    private void Awake()
    {
        _emmiters = GetComponentsInChildren<ParticleSystem>();
        _animator = GetComponentInChildren<Animator>();
        _hashIsActive = Animator.StringToHash("IsActive");

        _toolCollider = GetComponentInChildren<Collider2D>();

        _OnAwake();
    }

    private void Start()
    {
        Debug.Log(gameObject.name + ": Start");

        foreach (var emmiter in _emmiters)
        {
            emmiter.Stop();
        }

        StartCoroutine(Coroutine_ToEagle());
        _OnStart();
    }

    private IEnumerator Coroutine_ToEagle()
    {
        Transform root = transform.Find("root");
        Vector3 startPosition = root.localPosition;
        Vector3 startScale = root.localScale;
        Vector3 endPosition = Vector3.zero;
        Vector3 endScale = Vector3.one;
        Quaternion startRotation = root.localRotation;
        Quaternion endRotation = Quaternion.identity;

        float k = 0.0f;
        const float k_speed = 2.0f;

        while (k <= 1.0f)
        {
            root.localPosition = Vector3.Lerp(startPosition, endPosition, k);
            root.localScale = Vector3.Lerp(startScale, endScale, k);
            root.localRotation = Quaternion.Lerp(startRotation, endRotation, k);
            k += Time.deltaTime * k_speed;

            yield return null;
        }

        root.localPosition = endPosition;
        root.localScale = endScale;
    }

    public virtual void StartApplyTool(ToolApplyZone zone)
    {
        Debug.Log(gameObject.name + ": StartApplyTool");

        if (_animator)
        {
            _animator.SetBool(_hashIsActive, true);
        }

        if (_emmiters != null)
        {
            foreach (var emmiter in _emmiters)
            {
                emmiter.Play();
            }
        }

        if(OnStartApply != null)
        {
            OnStartApply(zone);
        }
    }

    public virtual void StopApplyTool(ToolApplyZone zone)
    {
        Debug.Log(gameObject.name + ": StopApplyTool");

        if (_animator)
        {
            _animator.SetBool(_hashIsActive, false);
        }

        if (_emmiters != null)
        {
            foreach (var emmiter in _emmiters)
            {
                emmiter.Stop();
            }
        }

        if (OnStopApply != null)
        {
            OnStopApply(zone);
        }
    }

    public void CheckZoneOverlapping()
    {
        int numResults = Physics2D.OverlapCollider(_toolCollider, _filter, _overlapResults);
        bool applyZoneFound = false;
        for (int i = 0; i < numResults; i++)
        {
            var collider = _overlapResults[i];

            if (collider.gameObject.tag == "ToolApplyZone")
            {
                var toolApplyZone = collider.GetComponent<ToolApplyZone>();
                if (toolApplyZone && toolApplyZone.IsToolAllowed(ToolID))
                {
                    applyZoneFound = true;

                    if (_currentToolApplyZone != null)
                    {
                        if (collider.gameObject.GetInstanceID() != _currentToolApplyZone.gameObject.GetInstanceID())
                        {
                            _currentToolApplyZone = toolApplyZone;
                            StartApplyTool(_currentToolApplyZone);
                        }
                    } else
                    {
                        _currentToolApplyZone = toolApplyZone;
                        StartApplyTool(_currentToolApplyZone);
                    }

                    break;
                }
            }//if (collider.gameObject.tag == "ToolApplyZone")            

        }//for (int i = 0; i < numResults; i++)
        
        if (!applyZoneFound && _currentToolApplyZone != null)
        {
            StopApplyTool(_currentToolApplyZone);
            _currentToolApplyZone = null;
        }        
    }    
}

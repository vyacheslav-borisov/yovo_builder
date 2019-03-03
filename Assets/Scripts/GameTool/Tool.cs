using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Tool : MonoBehaviour, IDragSource, ISceneElement
{
    public ToolId     _toolId;   
    public GameObject _toolPrefab;
    public bool _isActive = true; 

    public bool IsToolActive
    {
        set
        {
            if(!_isActive && value)
            {
                EnableTool();
            }

            if(_isActive && !value)
            {
                DisableTool();
            }

            _isActive = value;
        }

        get
        {
            return _isActive;
        }
    }

    public static Tool CurrentToolInUse
    {
        get;
        private set;
    }
           
    protected GameObject _toolPrefabInstance;
    protected ToolDragObject _toolDragObject;

    protected GameObject _toolIcon;
    protected GameObject _lockIcon;
    protected SpriteRenderer _toolIconSprite;
     

    protected virtual void _OnAwake() { }
    protected virtual void _OnEnable() { }

    private static Dictionary<ToolId, Tool> s_toolLookupTable;
    
    public static Tool GetById(ToolId id)
    {
        if(s_toolLookupTable != null)
        {
            if(s_toolLookupTable.ContainsKey(id))
            {
                return s_toolLookupTable[id];
            }
        }

        return null;
    }

    public void SceneElement_Init()
    {
        if (s_toolLookupTable == null)
        {
            s_toolLookupTable = new Dictionary<ToolId, Tool>();
        }

        var transformToolIcon = transform.Find("tool_icon");
        var transformToolLocked = transform.Find("lock_icon");
        if (transformToolIcon) _toolIcon = transformToolIcon.gameObject;
        if (transformToolLocked) _lockIcon = transformToolLocked.gameObject;

        if(_toolIcon != null)
        {
            _toolIconSprite = _toolIcon.GetComponent<SpriteRenderer>();
        }

        _OnAwake();
    }

    public void SceneElement_Reset()
    {
        if (!s_toolLookupTable.ContainsKey(_toolId))
        {
            s_toolLookupTable.Add(_toolId, this);
        }
        else
        {
            s_toolLookupTable[_toolId] = this;
        }

        gameObject.SetActive(true);

        if (_toolIcon != null)
        {
            _toolIcon.SetActive(true);
        }

        if (_lockIcon != null)
        {
            bool locked = GameFlowManager.Instance.IsToolLocked(_toolId);
            _lockIcon.SetActive(locked);
        }

        if(_toolIconSprite != null)
        {
            _toolIconSprite.color = _isActive ? Color.white : _darkenColor;
        }

        DestroyPrevTool();

        _OnEnable();
    }

    private void OnDestroy()
    {
        s_toolLookupTable.Remove(_toolId);
    }

    private readonly Color _darkenColor = new Color(0.1f, 0.1f, 0.1f);

    public void EnableTool()
    {
        if(_toolIconSprite != null)
        {
            StartCoroutine(Coroutine_DarkenColor(_darkenColor, Color.white));
        }
    }

    public void DisableTool()
    {
        if (_toolIconSprite != null)
        {
            StartCoroutine(Coroutine_DarkenColor(Color.white, _darkenColor));
        }
    }

    public void UnlockTool()
    {
        if(_lockIcon != null)
        {
            _lockIcon.SetActive(false);
        }
    }

    public void ForceDrop()
    {
        if (_toolPrefabInstance != null)
        {
            _toolDragObject.StopApplyTool(null);

            StartCoroutine(Coroutine_BackToHome(_toolPrefabInstance));

            CurrentToolInUse = null;
            _toolPrefabInstance = null;
        }
    }

    protected virtual void _OnStartDrag()
    {
        Debug.Log(gameObject.name + ": _OnStartDrag");

        if(!IsToolActive)
        {
            Debug.Log(gameObject.name + "tool disabled");
            return;
        }

        DestroyPrevTool();

        if (!GameFlowManager.Instance.IsToolLocked(_toolId))
        {
            if (_toolPrefab)
            {
                Vector3 position = Vector3.zero;
                position.z -= 0.2f;

                _toolPrefabInstance = Instantiate(_toolPrefab, transform);
                _toolPrefabInstance.transform.localPosition = position;

                _toolDragObject = _toolPrefabInstance.GetComponent<ToolDragObject>();
                _toolDragObject.ToolID = _toolId;

                if (_toolIcon)
                {
                    _toolIcon.SetActive(false);
                }

                CurrentToolInUse = this;
            }else
            {
                Debug.LogError("Prefab for tool " + _toolId + " is not specified");
            }
        }else
        {
            //TODO: show InGame Purchasing Window
        }
    }

    protected virtual void _OnStopDrag()
    {
        Debug.Log(gameObject.name + ": _OnStopDrag");

        ForceDrop();
    }

    protected virtual void _OnDrag()
    {
        if (_toolPrefabInstance != null)
        {
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 objectPosition = _toolPrefabInstance.transform.position;
            objectPosition.x = cursorPosition.x;
            objectPosition.y = cursorPosition.y;
            _toolPrefabInstance.transform.position = objectPosition;

            _toolDragObject.CheckZoneOverlapping();
        }        
    }

    private IEnumerator Coroutine_BackToHome(GameObject toolInstance)
    {
        const float speed = 2.0f;

        Vector3 startPosition = toolInstance.transform.position;
        Vector3 endPosition = transform.position;
        endPosition.z = toolInstance.transform.position.z;

        float pathLength = (endPosition - startPosition).magnitude;

        if (pathLength > 0.0f)
        {
            float totalTime = pathLength / speed;
            float elapsedTime = 0.0f;
            float k = 0.0f;

            while (elapsedTime < totalTime)
            {
                elapsedTime += Time.deltaTime;
                k = elapsedTime / totalTime;
                toolInstance.transform.position = Vector3.Lerp(startPosition, endPosition, k);

                yield return null;
            }
        }

        Destroy(toolInstance);

        if (_toolIcon)
        {
            _toolIcon.SetActive(true);
        }
    }

    private IEnumerator Coroutine_DarkenColor(Color from, Color to)
    {
        float k = 0.0f;

        while(k < 1.0f)
        {
            k += Time.deltaTime;
            _toolIconSprite.color = Color.Lerp(from, to, k);

            yield return null;
        }

        _toolIconSprite.color = to;
    }

    protected void DestroyPrevTool()
    {
        StopAllCoroutines();

        if(_toolPrefabInstance != null)
        {
            Destroy(_toolPrefabInstance);
            _toolPrefabInstance = null;
        }
    }

    void IDragSource.OnStartDrag()
    {
        _OnStartDrag();
    }

    void IDragSource.OnStopDrag()
    {
        _OnStopDrag();
    }

    void IDragSource.OnDrag()
    {
        _OnDrag();
    }

    void IDragSource.OnDropZone_Enter(IDropZone dropZone)
    {
        //throw new NotImplementedException();
    }

    void IDragSource.OnDropZone_Exit(IDropZone dropZone)
    {
        //throw new NotImplementedException();
    }

    void IDragSource.OnDropZone_Over(IDropZone dropZone)
    {
        //throw new NotImplementedException();
    }

    void IDragSource.OnDropZone_Drop(IDropZone dropZone)
    {
        //throw new NotImplementedException();
    }
}



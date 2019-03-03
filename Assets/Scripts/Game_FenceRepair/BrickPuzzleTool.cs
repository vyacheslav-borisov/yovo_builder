using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickPuzzleTool : PuzzleToolButton, IScrollableItem
{
    Transform IScrollableItem.ItemTransform
    {
        get
        {
            return transform;
        }
    }

    Collider2D IScrollableItem.ItemBoundBox
    {
        get
        {
            return _collider;
        }
    }

    Collider2D _collider;

    public BrickPuzzleTool() 
        : base(SceneId.GAME_FENCE_REPAIR)
    {
    }

    protected override void _OnAwake()
    {
        base._OnAwake();
        _collider = GetComponent<Collider2D>();
    }

    private IUIScroller _scrollContext;
    public void SetContext(IUIScroller context)
    {
        _scrollContext = context;
    }

    private Vector3 prevCoords;
    private void OnMouseDown()
    {
        Debug.Log(gameObject.name + ": OnMouseDown");

        prevCoords = Input.mousePosition;

        _scrollContext.StartScroll(this);
    }

    private void OnMouseDrag()
    {
        Debug.Log(gameObject.name + ": OnMouseDrag");

        var delta = Input.mousePosition - prevCoords;
        prevCoords = Input.mousePosition;

        _scrollContext.Scroll(this, delta);        
    }

    private void OnMouseUp()
    {
        _scrollContext.StopScroll(this);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScrollableItem
{
    void SetContext(IUIScroller context);

    Transform ItemTransform { get; }  
    Collider2D ItemBoundBox { get; }
}

public interface IUIScroller
{
    void StartScroll(IScrollableItem sender);
    void StopScroll(IScrollableItem sender);
    void Scroll(IScrollableItem sender, Vector2 mouseCoordsDelta);
}

public class SpriteScroller : MonoBehaviour, IUIScroller
{
    private IScrollableItem[] _items;
    private float _coordsScaler;
    private float _averageMargin;

    private List<IScrollableItem> _itemQueue = new List<IScrollableItem>();
    private Collider2D _myColider;

    private class ScrollableItemXComparer : IComparer<IScrollableItem>
    {
        int IComparer<IScrollableItem>.Compare(IScrollableItem left, IScrollableItem right)
        {
            var posLeft = left.ItemTransform.localPosition;
            var posRight = right.ItemTransform.localPosition;

            if(posLeft.x < posRight.x)
            {
                return -1;
            }

            if (posLeft.x > posRight.x)
            {
                return 1;
            }

            return 0;            
        }
    }

    private void Start()
    {
        _myColider = GetComponent<Collider2D>();

        _items = GetComponentsInChildren<IScrollableItem>();
        Array.Sort(_items, new ScrollableItemXComparer());
              
        
        List<float> margins = new List<float>();
        Bounds? prevBoundBox = null;
        foreach (var item in _items)
        {
            item.SetContext(this);

            var boundBox = item.ItemBoundBox.bounds;

            //var name = item.ItemTransform.gameObject.name;
            //var min = boundBox.min;
            //var max = boundBox.max;
            //Debug.Log(name + "min:[x = " + min.x +", y = " + min.y +"], max:[x = " + max.x + ", y = " + max.y + "]");

            if(prevBoundBox.HasValue)
            {
                var delta = boundBox.min - prevBoundBox.Value.max;
                margins.Add(delta.x);

                //Debug.Log("delta [x = " + delta.x + ",y = " + delta.y + "]");
            }
            prevBoundBox = boundBox;

            _itemQueue.Add(item);
        }

        if (margins.Count > 0)
        {
            float totalMargin = 0.0f;
            margins.ForEach((float margin) => { totalMargin += margin; });
            _averageMargin = totalMargin / margins.Count;
        }

        _coordsScaler = 2.0f / Screen.height;
    }


    private Vector3 prevCoords;
    private void OnMouseDown()
    {
        Debug.Log(gameObject.name + ": OnMouseDown");

        prevCoords = Input.mousePosition;        
    }

    private void OnMouseDrag()
    {
        Debug.Log(gameObject.name + ": OnMouseDrag");

        var delta = Input.mousePosition - prevCoords;
        prevCoords = Input.mousePosition;

        //Debug.Log("delta: " + delta.x);

        _Scroll(delta);
    }

    private void CheckFirstItemBound()
    {
        var myBoundMin = _myColider.bounds.min;
        var myBoundMax = _myColider.bounds.max;

        if(_itemQueue.Count < 2)
        {
            return;
        }

        var firtsItem = _itemQueue[0];
        var lastItem = _itemQueue[_itemQueue.Count - 1];
        var firtsMin = firtsItem.ItemBoundBox.bounds.min;
        var lastMax = lastItem.ItemBoundBox.bounds.max;

        if (firtsMin.x > myBoundMin.x)
        {
            var positionX = firtsMin.x;
            positionX -= _averageMargin;
            positionX -= lastItem.ItemBoundBox.bounds.extents.x;
            positionX -= lastItem.ItemBoundBox.offset.x;

            var vPosition = lastItem.ItemTransform.position;
            vPosition.x = positionX;
            lastItem.ItemTransform.position = vPosition;

            _itemQueue.RemoveAt(_itemQueue.Count - 1);
            _itemQueue.Insert(0, lastItem);
        }
    }

    private void CheckLastItemBound()
    {
        var myBoundMin = _myColider.bounds.min;
        var myBoundMax = _myColider.bounds.max;

        if (_itemQueue.Count < 2)
        {
            return;
        }

        var firtsItem = _itemQueue[0];
        var lastItem = _itemQueue[_itemQueue.Count - 1];
        var firtsMin = firtsItem.ItemBoundBox.bounds.min;
        var lastMax = lastItem.ItemBoundBox.bounds.max;

        if (lastMax.x < myBoundMax.x)
        {
            var positionX = lastMax.x;
            positionX += _averageMargin;
            positionX += firtsItem.ItemBoundBox.bounds.extents.x;
            positionX -= firtsItem.ItemBoundBox.offset.x;

            var vPosition = firtsItem.ItemTransform.position;
            vPosition.x = positionX;
            firtsItem.ItemTransform.position = vPosition;

            _itemQueue.RemoveAt(0);
            _itemQueue.Add(firtsItem);
        }
    }

    IScrollableItem _sender;
    IDragSource _dragSource;
    Coroutine _coroutineCheckDrag;
    public void StartScroll(IScrollableItem sender)
    {
        var collider = sender.ItemBoundBox;
        var min = collider.bounds.min;
        var max = collider.bounds.max;
        var center = collider.bounds.center;

        //if (_myColider.OverlapPoint(min) && _myColider.OverlapPoint(max))
        if(_myColider.OverlapPoint(center))
        {
            _sender = sender;
            _coroutineCheckDrag = StartCoroutine(Coroutine_CheckDrag(Input.mousePosition));
        }
    }

    public void RemoveItem(IScrollableItem item)
    {
        var index = _itemQueue.FindIndex((IScrollableItem itemInQueue) => 
        {
            return item == itemInQueue;
        });

        if(index < _itemQueue.Count - 1)
        {
            var nextItem = _itemQueue[index + 1];
            var currentMin = item.ItemBoundBox.bounds.min;
            var nextMin = nextItem.ItemBoundBox.bounds.min;
            var delta = currentMin - nextMin;

            Debug.LogWarning("current min x: " + currentMin.x);
            Debug.LogWarning("next min x: " + nextMin.x);
            Debug.LogWarning("delta x:" + delta.x);

            var test = nextItem.ItemTransform.position;
            test.x += delta.x;
            nextItem.ItemTransform.position = test;

            nextMin = nextItem.ItemBoundBox.bounds.min;
            Debug.LogWarning("next min + delta: " + nextMin.x);

            /*for (int i = index + 1; i < _itemQueue.Count; i++)
            {
                nextItem = _itemQueue[i];
                var position = nextItem.ItemTransform.position;
                position.x += delta.x * 1.5f;
                nextItem.ItemTransform.position = position;
            }*/
        }

        _itemQueue.Remove(item);

        CheckFirstItemBound();
        CheckLastItemBound();
    }

    public void StopScroll(IScrollableItem sender)
    {
        if(_coroutineCheckDrag != null)
        {
            StopCoroutine(_coroutineCheckDrag);
            _coroutineCheckDrag = null;
        }

        if(_dragSource != null)
        {
            _dragSource.OnStopDrag();
            _dragSource = null;
        }
        _sender = null;        
    }

    public void Scroll(IScrollableItem sender, Vector2 mouseCoordsDelta)
    {
        if(sender == _sender)
        {
            if (_dragSource != null)
            {
                _dragSource.OnDrag();
            }
            else
            {
                _Scroll(mouseCoordsDelta);
            }
        }
    }

    private void _Scroll(Vector2 mouseCoordsDelta)
    {
        foreach (var item in _items)
        {
            var position = item.ItemTransform.position;
            position.x += mouseCoordsDelta.x * _coordsScaler;
            item.ItemTransform.position = position;
        }

        CheckFirstItemBound();
        CheckLastItemBound();
    }

    private IEnumerator Coroutine_CheckDrag(Vector3 startCoords)
    {
        yield return new WaitForSeconds(0.5f);

        var delta = Input.mousePosition - startCoords;
        if(delta.magnitude < 10.0f && _sender != null && _sender is IDragSource)
        {
            _dragSource = _sender as IDragSource;
            _dragSource.OnStartDrag();
        }

        _coroutineCheckDrag = null;
    }
}

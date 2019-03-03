using UnityEngine;
using UnityEngine.Events;

public class SlideTrigger : MonoBehaviour
{
    public enum SlideDirection
    {
        SLIDE_LEFT,
        SLIDE_RIGHT,
        SLIDE_TOP,
        SLIDE_BOTTOM
    }

    public SlideDirection _slideDirection;
    public float _tresshold = 5;

    private Vector3 _startPos;
    private bool    _slideStarted = false;
    private bool    _blocked = false;

    public UnityEvent OnSlideStart;
    public UnityEvent OnSlideStop;

    private void OnMouseDown()
    {
        if (_blocked) return;

        _startPos = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        if (_blocked) return;

        if (!_slideStarted)
        {
            var delta = Input.mousePosition - _startPos;
            if (CheckSlide(delta))
            {
                _slideStarted = true;

                if(OnSlideStart != null)
                {
                    OnSlideStart.Invoke();
                }
            }
        }//if (!_slideStarted)
    }//private void OnMouseDrag()

    private void OnMouseUp()
    {
        if (_blocked) return;

        if (_slideStarted)
        {
            _slideStarted = false;

            if(OnSlideStop != null)
            {
                OnSlideStop.Invoke();
            }
        }
    }//private void OnMouseUp()

    public void Lock()
    {
        Debug.Log("SlideTrigger " + gameObject.name + " is locked");

        _blocked = true;
    }

    public void Unlock()
    {
        Debug.Log("SlideTrigger " + gameObject.name + " is unlocked");

        _blocked = false;
    }

    private bool CheckSlide(Vector3 delta)
    {
        if(_slideDirection == SlideDirection.SLIDE_TOP)
        {
            return (Mathf.Abs(delta.y) >= _tresshold) && (delta.y > 0);
        }

        if (_slideDirection == SlideDirection.SLIDE_BOTTOM)
        {
            return (Mathf.Abs(delta.y) >= _tresshold) && (delta.y < 0);
        }

        if (_slideDirection == SlideDirection.SLIDE_LEFT)
        {
            return (Mathf.Abs(delta.x) >= _tresshold) && (delta.x < 0);
        }

        if (_slideDirection == SlideDirection.SLIDE_RIGHT)
        {
            return (Mathf.Abs(delta.x) >= _tresshold) && (delta.x > 0);
        }

        return false;
    }
}

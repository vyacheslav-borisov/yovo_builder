using UnityEngine;

[RequireComponent(typeof(IDragSource))]
[RequireComponent(typeof(Collider2D))]
public class DragSource : MonoBehaviour
{
    private IDragSource _dragSource;

    private void Awake()
    {
        _dragSource = GetComponent<IDragSource>();
    }

    private void OnMouseDown()
    {
        DragDropManager.Instance.OnStartDrag(_dragSource);

        _dragSource.OnStartDrag();
    }

    private void OnMouseUp()
    {
        _dragSource.OnStopDrag();

        DragDropManager.Instance.OnEndDrag(_dragSource);
    }

    private void OnMouseDrag()
    {
        _dragSource.OnDrag();        
    }
}

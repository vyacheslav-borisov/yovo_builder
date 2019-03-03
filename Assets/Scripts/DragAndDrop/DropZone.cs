using UnityEngine;

[RequireComponent(typeof(IDropZone))]
[RequireComponent(typeof(Collider2D))]
public class DropZone : MonoBehaviour
{
    private IDropZone _dropZone;

    private void Awake()
    {
        _dropZone = GetComponent<IDropZone>();
    }

    private void OnMouseEnter()
    {
        DragDropManager.Instance.OnEnterDropZone(_dropZone);                
    }

    private void OnMouseExit()
    {
        DragDropManager.Instance.OnExitDropZone(_dropZone);
    }

    private void OnMouseOver()
    {
        DragDropManager.Instance.OnDropZoneOver(_dropZone);        
    }
}

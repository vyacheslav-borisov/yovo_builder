using UnityEngine;

public interface IDragSource
{
    void OnStartDrag();
    void OnStopDrag();
    void OnDrag();
    void OnDropZone_Enter(IDropZone dropZone);
    void OnDropZone_Exit(IDropZone dropZone);
    void OnDropZone_Over(IDropZone dropZone);
    void OnDropZone_Drop(IDropZone dropZone);
}

public interface IDropZone
{
    void OnDragSource_Over(IDragSource dragSource);
    void OnDragSource_Drop(IDragSource dragSource);
}

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager Instance
    {
        get; private set;
    }

    public IDragSource CurrentDragSource
    {
        get; private set;
    }

    public IDropZone CurrentDropZone
    {
        get; private set;
    }

    public DragDropManager()
    {
        Instance = this;
    }
    
    public void OnStartDrag(IDragSource dragSource)
    {
        CurrentDragSource = dragSource;
    }

    public void OnEndDrag(IDragSource dragSource)
    {
        if(CurrentDropZone != null)
        {
            dragSource.OnDropZone_Drop(CurrentDropZone);
            CurrentDropZone.OnDragSource_Drop(dragSource);
        }

        CurrentDragSource = null;
    }

    public void OnEnterDropZone(IDropZone dropZone)
    {
        CurrentDropZone = dropZone;

        if (CurrentDragSource != null)
        {
            CurrentDragSource.OnDropZone_Enter(dropZone);
        }
    }

    public void OnExitDropZone(IDropZone dropZone)
    {
        if (CurrentDragSource != null)
        {
            CurrentDragSource.OnDropZone_Exit(dropZone);
        }

        CurrentDropZone = null;
    }

    public void OnDropZoneOver(IDropZone dropZone)
    {
        if(CurrentDragSource != null)
        {
            CurrentDragSource.OnDropZone_Over(dropZone);
            dropZone.OnDragSource_Over(CurrentDragSource);
        }
    }
}

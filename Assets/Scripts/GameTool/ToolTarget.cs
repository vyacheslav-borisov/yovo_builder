using UnityEngine;

public class ToolTarget : MonoBehaviour
{
    private ToolDragObject _owner;
    private ToolApplyZone  _toolApplyZone = null;

    private void Awake()
    {
        _owner = GetComponentInParent<ToolDragObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D");

        if(collision.gameObject.tag == "ToolApplyZone")
        {
            Debug.Log(gameObject.name + ": OnTriggerEnter2D");

            _toolApplyZone = collision.GetComponent<ToolApplyZone>();
            if (_toolApplyZone && _toolApplyZone.IsToolAllowed(_owner.ToolID))
            {
                _owner.StartApplyTool(_toolApplyZone);
            }            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_toolApplyZone != null)
        {
            Debug.Log(gameObject.name + ": OnTriggerExit2D");

            if (collision.gameObject.GetInstanceID() == _toolApplyZone.gameObject.GetInstanceID())
            {
                _owner.StopApplyTool(_toolApplyZone);
                _toolApplyZone = null;
            }
        }
    }
}



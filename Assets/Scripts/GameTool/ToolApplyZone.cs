using UnityEngine;

public class ToolApplyZone : MonoBehaviour
{
    [SerializeField]
    protected ToolId[] _allowedTools;
    
    public bool IsToolAllowed(ToolId id)
    {
        foreach(var allowedToolId in _allowedTools)
        {
            if(allowedToolId == id)
            {
                return true;
            }
        }

        return false;
    }        
}

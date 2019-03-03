using UnityEngine;

public class SimpleHummer_AET : MonoBehaviour
{
    private SimpleHummer _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<SimpleHummer>();
    }

    public void Event_OnHit()
    {
        if(_owner)
        {
            _owner.OnHit();
        }        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHummer : ToolDragObject
{
    public float _damagePerHit = 10.0f;

    private Floor _currentFloor;

    public override void StartApplyTool(ToolApplyZone zone)
    {
        base.StartApplyTool(zone);

        _currentFloor = (Floor)zone;
        if(_currentFloor == null || _currentFloor.Damage >= 100.0f)
        {
            _currentFloor = null;
            StopApplyTool(zone);
        }
    }

    public override void StopApplyTool(ToolApplyZone zone)
    {
        base.StopApplyTool(zone);

        _currentFloor = null;
    }

    public void OnHit()
    {
        if(_currentFloor)
        {
            _currentFloor.Damage += _damagePerHit;
            if(_currentFloor.Damage >= 100.0f)
            {
                StopApplyTool(_currentFloor);
            }
        }
    }
}

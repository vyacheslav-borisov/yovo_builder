using System.Collections;
using UnityEngine;

public class JackHummer : ToolDragObject
{
    public float _damagePerSecond = 20.0f;

    private Coroutine _coroutine = null;

    public override void StartApplyTool(ToolApplyZone zone)
    {
        base.StartApplyTool(zone);

        if(zone is Floor)
        {
            var floor = zone as Floor;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);                
            }
            _coroutine = StartCoroutine(Coroutine_ContinousDamage(floor));
        }
    }

    public override void StopApplyTool(ToolApplyZone zone)
    {
        base.StopApplyTool(zone);

        if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator Coroutine_ContinousDamage(Floor floor)
    {
        while (floor.Damage < 100.0f)
        {
            floor.Damage += _damagePerSecond * Time.deltaTime;
            yield return null;
        }

        StopApplyTool(floor);
    }
}

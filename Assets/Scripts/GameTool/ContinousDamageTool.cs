using System.Collections;
using UnityEngine;

public class ContinousDamageTool : ToolDragObject
{
    public float _damagePerSecond = 20.0f;

    private Coroutine _coroutine = null;

    public override void StartApplyTool(ToolApplyZone zone)
    {
        base.StartApplyTool(zone);

        if (zone is IDamagableZone)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            var floor = zone as IDamagableZone;
            _coroutine = StartCoroutine(Coroutine_ContinousDamage(floor));
        }
    }

    public override void StopApplyTool(ToolApplyZone zone)
    {
        base.StopApplyTool(zone);

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator Coroutine_ContinousDamage(IDamagableZone floor)
    {
        while (floor.Damage < 100.0f)
        {
            floor.Damage += _damagePerSecond * Time.deltaTime;
            yield return null;
        }

        StopApplyTool(floor as ToolApplyZone);
    }
}

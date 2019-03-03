using UnityEngine;

public class ManualBrushTool : ToolDragObject
{
    [Range(0, 10)]
    public float pixPerSecond = 0.5f;
    [Range(0, 1)]
    public float effectDelay = 0.3f;
    public float damagePerSecond = 20;

    public GameObject instantApplyEffect;
    public Transform toolTarget;

    public override void StartApplyTool(ToolApplyZone zone)
    {
        base.StartApplyTool(zone);

        if (zone is IDamagableZone)
        {
            _brushZone = zone as IDamagableZone;
            _prevCoords = transform.position;
            _ellapsedTime = 0.0f;
            //_effectTime = 0.0f;
            _ellapsedPath = 0.0f;
        }
    }

    public override void StopApplyTool(ToolApplyZone zone)
    {
        base.StopApplyTool(zone);
        _brushZone = null;
    }

    private IDamagableZone _brushZone = null;
    private Vector3 _prevCoords;
    private float _ellapsedTime = 0.0f;
    private float _effectTime = 0.0f;
    private float _ellapsedPath = 0.0f;

    private void Update()
    {
        if(_brushZone != null)
        {
            _ellapsedTime += Time.deltaTime;
            _effectTime += Time.deltaTime;

            var deltaCoords = transform.position - _prevCoords;
            _ellapsedPath += deltaCoords.magnitude;

            if(_ellapsedTime >= 0.0f)
            {
                var averageSpeed = _ellapsedPath / _ellapsedTime;
                if(averageSpeed >= pixPerSecond)
                {
                    _brushZone.Damage += damagePerSecond * Time.deltaTime;

                    if(instantApplyEffect != null && _effectTime >= effectDelay)
                    {
                        _effectTime = 0.0f;

                        var effect = Instantiate(instantApplyEffect, toolTarget.position, Quaternion.identity);
                        DestroyObject(effect, 1.0f);
                    }
                }
            }

           _prevCoords = transform.position;
        }
    }   
        
}

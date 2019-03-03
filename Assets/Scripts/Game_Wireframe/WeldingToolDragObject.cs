using UnityEngine;

public class WeldingToolDragObject : ContinousDamageTool
{
    public ParticleSystem _flame;
    public ParticleSystem _sparks;

    protected override void _OnStart()
    {
        base._OnStart();

        if(_flame)
        {
            _flame.Play();
        }
    }

    public void SnuffFlame()
    {
        if (_flame)
        {
            _flame.Stop();
        }
    }

    public override void StartApplyTool(ToolApplyZone zone)
    {
        base.StartApplyTool(zone);

        if(_sparks)
        {
            _sparks.Play();
        }
    }

    public override void StopApplyTool(ToolApplyZone zone)
    {
        base.StopApplyTool(zone);

        if (_flame)
        {
            _flame.Play();
        }

        if (_sparks)
        {
            _sparks.Stop();
        }
    }
}

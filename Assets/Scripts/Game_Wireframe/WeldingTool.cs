
public class WeldingTool : Tool
{
    public WeldingMask _weldingMask;
    public WeldingToolDragObject _wtdo;

    protected override void _OnStartDrag()
    {
        base._OnStartDrag();

        if (_toolPrefabInstance != null && _weldingMask != null)
        {
            if (!_weldingMask.IsEquipped())
            {
                _weldingMask.StartBlink();

                var cdt = _toolPrefabInstance.GetComponent<ContinousDamageTool>();
                if (cdt != null)
                {
                    //если не надели маску - показываем слепящую вспышку, а трещина не заваривается
                    //(отключаем меру воздействия  у инструмента)
                    cdt._damagePerSecond = 0.0f;
                }
            }

            _wtdo = _toolPrefabInstance.GetComponent<WeldingToolDragObject>();
            _wtdo.OnStartApply += _weldingMask.Event_OnStartApplyWelding;
            _wtdo.OnStopApply += _weldingMask.Event_OnStopApplyWelding;
        }
    }

    protected override void _OnStopDrag()
    {
        base._OnStopDrag();

        if (_weldingMask != null)
        {
            _weldingMask.StopBlink();
        }

        if(_wtdo != null)
        {
            _wtdo.SnuffFlame();
        }       
    }   
}

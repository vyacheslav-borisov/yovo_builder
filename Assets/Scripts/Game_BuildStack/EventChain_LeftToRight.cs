using UnityEngine;

public class EventChain_LeftToRight 
{
    public bool Active
    {
        set
        {
            if (!_active && value)
            {
                Launch();
            }

            if (_active && !value)
            {
                Stop();
            }

            _active = value;
        }

        get { return _active; }
    }

    private PortalCrane _crane;
    private AssemblyLine _leftLine;
    private AssemblyLine _rightLine;

    private bool _active;

    public EventChain_LeftToRight(PortalCrane crane, AssemblyLine leftLine, AssemblyLine rightLine)
    {
        _crane = crane;
        _leftLine = leftLine;
        _rightLine = rightLine;
    }

    private void Launch()
    {
        //launch event chane 1:
        //step 1: left line produce new block
        //step 2: crane hook moving left
        //step 3: crane hook put down
        //step 4: crane hook catch block from left line
        //step 5: crane hook put up
        //step 6: crane hook moving right
        // PLAYER WOULD DROP BLOCK HERE - if so start chain 2
        //step 7: crane hook put down
        //step 8: block placed onto right line
        //step 9: crane hook put up
        //step 10: line move away and eleminate block

        _crane.OnCraneOnLeftPosition += EventHandler_OnCraneHookLeft;
        _crane.OnCraneOnRightPosition += EventHandler_OnCraneHookRight;
        _crane.OnHookDownFinish += EventHandler_OnCraneHookDown;
        _crane.OnHookUpFinish += EventHandler_OnCraneHookUp;
        _leftLine.OnNewBlockReady += EventHandler_OnNewBlockReady;

        _leftLine.ProduceNewBlock();
    }

    private void Stop()
    {
        _currentBlock = null;

        _crane.OnCraneOnLeftPosition -= EventHandler_OnCraneHookLeft;
        _crane.OnCraneOnRightPosition -= EventHandler_OnCraneHookRight;
        _crane.OnHookDownFinish -= EventHandler_OnCraneHookDown;
        _crane.OnHookUpFinish -= EventHandler_OnCraneHookUp;
        _leftLine.OnNewBlockReady -= EventHandler_OnNewBlockReady;
    }

    private Transform _currentBlock;
    private bool _hookOnLeftPosition;

    private void EventHandler_OnNewBlockReady(Transform newBlock)
    {
        _currentBlock = newBlock;
        _crane.MoveHookLeft();
    }

    private void EventHandler_OnCraneHookLeft()
    {
        _hookOnLeftPosition = true;
        _crane.PutDownHook();
    }

    private void EventHandler_OnCraneHookRight()
    {
        _hookOnLeftPosition = false;
        _crane.PutDownHook();
    }

    private void EventHandler_OnCraneHookDown()
    {
        if(_hookOnLeftPosition)
        {
            //attach block
            _crane.AttachBuildBlock(_currentBlock);            
        }else
        {
            //dettach block
            _currentBlock = _crane.DettachBuildBlock();
        }
        _crane.PutUpHook();
    }

    private void EventHandler_OnCraneHookUp()
    {
        if (_hookOnLeftPosition)
        {
            _crane.MoveHookRight();
        }else
        {
            if(_currentBlock != null)
            {
                _leftLine.ProduceNewBlock();
                _rightLine.EleminateBlock(_currentBlock);
                _currentBlock = null;
            }
        }
    }
    
}

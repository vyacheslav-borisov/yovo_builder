using UnityEngine;
using UnityEngine.Events;

public class WreckingBall_AET : MonoBehaviour
{
    public UnityEvent OnWreckFloor4;
    public UnityEvent OnWreckFloor3;
    public UnityEvent OnWreckFloor2;
    public UnityEvent OnWreckFloor1;
    public UnityEvent OnWreckFloor0;
    public UnityEvent OnWreckingFinished;

    public void Event_WreckFloor4()
    {
        if(OnWreckFloor4 != null)
        {
            OnWreckFloor4.Invoke();
        }
    }

    public void Event_WreckFloor3()
    {
        if (OnWreckFloor3 != null)
        {
            OnWreckFloor3.Invoke();
        }
    }

    public void Event_WreckFloor2()
    {
        if (OnWreckFloor2 != null)
        {
            OnWreckFloor2.Invoke();
        }
    }

    public void Event_WreckFloor1()
    {
        if (OnWreckFloor1 != null)
        {
            OnWreckFloor1.Invoke();
        }
    }

    public void Event_WreckFloor0()
    {
        if (OnWreckFloor0 != null)
        {
            OnWreckFloor0.Invoke();
        }
    }

    public void Event_OnWreckingFinished()
    {
        if(OnWreckingFinished != null)
        {
            OnWreckingFinished.Invoke();
        }
    }
}

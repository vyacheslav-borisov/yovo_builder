using UnityEngine;

public class GUIDecorator : GUIAnimator
{
    public GUIAnimator _decoratedGUI;

    public override void ResetGUI()
    {
        Debug.Log("GUIDecorator.ResetGUI [" + gameObject.name + "]");

        base.ResetGUI();

        if (_decoratedGUI)
        {
            _decoratedGUI.ResetGUI();
        }
    }

    public override void ShowGUI(int stage = 0)
    {
        Debug.Log("GUIDecorator.ShowGUI [" + gameObject.name + "]");

        base.ShowGUI(stage);

        if(_decoratedGUI)
        {
            _decoratedGUI.ShowGUI(stage);
        }
    }

    public override void HideGUI()
    {
        Debug.Log("GUIDecorator.HideGUI [" + gameObject.name + "]");

        base.HideGUI();
        
        if(_decoratedGUI)
        {
            _decoratedGUI.HideGUI();
        }        
    }

    private void OnEnable()
    {
        if (_decoratedGUI != null)
        {
            _decoratedGUI.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (_decoratedGUI != null)
        {
            _decoratedGUI.gameObject.SetActive(false);
        }
    }
}

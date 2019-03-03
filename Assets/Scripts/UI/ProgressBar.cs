using UnityEngine;

public class ProgressBar : MonoBehaviour, ISceneElement
{
    public float Progress
    {
        set
        {
            _progress = value;
            _progress = Mathf.Clamp(_progress, 0.0f, 100.0f);
            UpdateBarView();
        }

        get
        {
            return _progress;
        }
    }

    private float _progress;
    private Transform _bar;
    private Vector3 _fullBarScale;
    private Vector3 _emptyBarScale;

    public void SceneElement_Init()
    {
        _bar = transform.Find("bar");
        if(_bar)
        {
            _fullBarScale = _bar.localScale;
            _emptyBarScale = Vector3.one;
            _emptyBarScale.x = 0.0f;
        }               
    }

    public void SceneElement_Reset()
    {
        _progress = 0.0f;
        if(_bar)
        {
            _bar.localScale = _emptyBarScale;
        }                
    }

    private void UpdateBarView()
    {
        if (_bar)
        {
            var k = _progress / 100.0f;
            _bar.localScale = Vector3.Lerp(_emptyBarScale, _fullBarScale, k);
        }
    }    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : ToolApplyZone, ISceneElement, IDamagableZone
{
    public float _gameProgress;

    public float Damage
    {
        set
        {
            _damage = value;
            UpdateCracks();
        }

        get
        {
            return _damage;
        }
    }

    public void Wreck()
    {
        GameFlowManager.Instance.AddGameProgress(SceneId.GAME_DEMOLITION, _gameProgress);

        //TODO: instantiate particle effect
        foreach(var effect in _wreckEffects)
        {
            effect.Play();
        }
        StartCoroutine(Coroutine_Disappear());        
    }

    private IEnumerator Coroutine_Disappear()
    {
        var sprites = GetComponentsInChildren<SpriteRenderer>();
        float k = 0.0f;

        while (k < 1.0f)
        {
            k += Time.deltaTime;
            
            foreach(var renderer in sprites)
            {
                var color = renderer.color;
                color.a = Mathf.Clamp01(1.0f - k);
                renderer.color = color;
            }

            yield return null;
        }
    }

    private float _damage;
    private bool _cracked;

    private SpriteRenderer _floor;     
    private List<GameObject> _cracks = new List<GameObject>();
    private List<GameObject> _hiddenCracks =  new List<GameObject>();
    private ParticleSystem[] _wreckEffects;
    
    public void SceneElement_Init()
    {
        _floor = GetComponent<SpriteRenderer>();
        _wreckEffects = GetComponentsInChildren<ParticleSystem>();

        var childs = GetComponentsInChildren<SpriteRenderer>(true);
        for (int i = 0; i < childs.Length; i++)
        {
            if (childs[i].gameObject.GetInstanceID() == gameObject.GetInstanceID())
            {
                continue;
            }

            _cracks.Add(childs[i].gameObject);
        }
    }

    public void SceneElement_Reset()
    {
        _damage = 0.0f;
        _cracked = false;
        _hiddenCracks.Clear();
        _hiddenCracks.AddRange(_cracks);
        _hiddenCracks.Shuffle();

        foreach (var crack in _cracks)
        {
            crack.SetActive(false);
        }

        var color = _floor.color;
        color.a = 1.0f;
        _floor.color = color;
    }

    private void UpdateCracks()
    {
        int numCracks = (int)Mathf.Floor(_damage * 0.01f * _cracks.Count);
        int showedCracks = _cracks.Count - _hiddenCracks.Count;

        Debug.Log(gameObject.name + ", damage = " + _damage);
        Debug.Log(gameObject.name + ", num cracks = " + numCracks);
        
        while(numCracks > showedCracks && _hiddenCracks.Count > 0)
        {
            GameObject crack = _hiddenCracks[0];
            _hiddenCracks.RemoveAt(0);
            crack.SetActive(true);
            showedCracks++;
        }
        
        if(_damage >= 100.0f && !_cracked)
        {
            _cracked = true;
            GameFlowManager.Instance.AddGameProgress(SceneId.GAME_DEMOLITION, _gameProgress);
        }                            
    }     
}

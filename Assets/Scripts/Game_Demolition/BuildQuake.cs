using UnityEngine;
using System.Collections;

public class BuildQuake : MonoBehaviour, ISceneElement
{
    public float _effectDuration = 5.0f;
    public GameObject _particleRoot;

    private Animator _animator;
    private int _hash_Quake;
    private Floor[] _floors;
    private ParticleSystem[] _smokes;

    public void SceneElement_Init()
    {
        _smokes = _particleRoot.GetComponentsInChildren<ParticleSystem>();
        _animator = GetComponent<Animator>();
        _hash_Quake = Animator.StringToHash("Quake");
        _floors = GetComponentsInChildren<Floor>();

        if (_floors.Length > 0)
        {
            float progressPerFloor = 100.0f / _floors.Length;
            foreach (var floor in _floors)
            {
                floor._gameProgress = progressPerFloor;
            }
        }
    }

    public void SceneElement_Reset()
    {
        _animator.Rebind();
    }

    public void StartQuake()
    {
        StartCoroutine(Coroutine_Quake());
    }
    
    private IEnumerator Coroutine_Quake()
    {
        _animator.SetBool(_hash_Quake, true);

        foreach(var smoke in _smokes)
        {
            smoke.Play();
        }

        float damagePerSecond = 110.0f / _effectDuration;
        float deltaTime = 0.0f;
        while (deltaTime < _effectDuration)
        {
            deltaTime += Time.deltaTime;

            foreach(var floor in _floors)
            {
                floor.Damage += damagePerSecond * Time.deltaTime;
            }

            yield return null;
        }

        _animator.SetBool(_hash_Quake, false);

        foreach (var smoke in _smokes)
        {
            smoke.Stop();
        }
    }    
}

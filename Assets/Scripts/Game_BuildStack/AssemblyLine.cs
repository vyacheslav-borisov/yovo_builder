using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyLine : MonoBehaviour, ISceneElement
{
    public delegate void EventHandler1();
    public delegate void EventHandler2(Transform block);

    public EventHandler2 OnNewBlockReady;
    public EventHandler1 OnBlockDestroyed;

    public float movingSpeed = 1.0f;
    public GameObject[] buildBlocks;

    private Transform _offScreenNode;
    private Transform _onScreenNode;

    private Animator _animator;
    private int _hashBandOnline;

    private List<GameObject> _randomBlockList = new List<GameObject>();

    public void SceneElement_Init()
    {
        _onScreenNode = transform.Find("on_screen_node");
        _offScreenNode = transform.Find("off_screen_node");

        _animator = GetComponent<Animator>();
        _hashBandOnline = Animator.StringToHash("BandOnline");

        for(var i = 0; i < 3; i++)
        {
            foreach(var block in buildBlocks)
            {
                _randomBlockList.Add(block);
            }
        }        
    }

    public void SceneElement_Reset()
    {
        _randomBlockList.Shuffle();
        if(_animator)
        {
            _animator.SetBool(_hashBandOnline, false);
        }
    }

    public void ProduceNewBlock()
    {
        if(_randomBlockList.Count == 0)
        {
            Debug.LogWarning("Build block prefabs is not setup!");
            return;
        }

        if(_onScreenNode == null || _offScreenNode == null)
        {
            Debug.LogWarning("assemly line band nodes not found!");
            return;
        }

        if (_animator == null)
        {
            Debug.LogWarning("Animator component not found!");
            return;
        }

        var prefab = _randomBlockList[0];
        _randomBlockList.RemoveAt(0);
        _randomBlockList.Add(prefab);

        var block = Instantiate(prefab, _offScreenNode.position, Quaternion.identity);

        StartCoroutine(Coroutine_MoveBlockViaBand(block.transform, true));
    }

    public void EleminateBlock(Transform block)
    {
        if (_onScreenNode == null || _offScreenNode == null)
        {
            Debug.LogWarning("assemly line band nodes not found!");
            return;
        }

        if (_animator == null)
        {
            Debug.LogWarning("Animator component not found!");
            return;
        }

        StartCoroutine(Coroutine_MoveBlockViaBand(block, false));
    }

    private IEnumerator Coroutine_MoveBlockViaBand(Transform block, bool appear)
    {
        _animator.SetBool(_hashBandOnline, true);

        var startPosition = appear ? _offScreenNode.position : _onScreenNode.position;
        var endPosition = appear ? _onScreenNode.position : _offScreenNode.position;

        float k = 0.0f;
        while (k < 1.0f)
        {
            k += Time.deltaTime * movingSpeed;
            block.position = Vector3.Lerp(startPosition, endPosition, k);

            yield return null;
        }

        _animator.SetBool(_hashBandOnline, false);

        if(appear)
        {
            if(OnNewBlockReady != null)
            {
                OnNewBlockReady(block);
            }
        }else
        {
            Destroy(block.gameObject);

            if(OnBlockDestroyed != null)
            {
                OnBlockDestroyed();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefingCharacter : MonoBehaviour
{
    protected GameObject _currentCharacter;
    protected Animator _animator;

    protected GameObject _quoteCloud;
    protected SpriteRenderer _icon;
    protected bool _buttonsLocked = true;   

    public GameObject[] characterPrefabs;
    public BriefingCharacter otherCharacter;

    private SceneId _gameToLaunch;

    protected virtual void _onAwake() { }
    protected virtual void _onEnable() { }
    protected virtual void _onDisable() { }
    protected virtual void _onMouseDown() { }

    private void Awake()
    {
        _quoteCloud = transform.Find("quoteCloud").gameObject;
        _icon = _quoteCloud.transform.Find("icon").GetComponent<SpriteRenderer>();

        _quoteCloud.SetActive(false);

        _onAwake();
    }

    private void OnEnable()
    {
        _quoteCloud.SetActive(false);
        _buttonsLocked = true;

        int index = Random.Range(0, characterPrefabs.Length);

        _currentCharacter = Instantiate(characterPrefabs[index], transform);
        _currentCharacter.transform.localPosition = Vector3.zero;
        _currentCharacter.transform.localRotation = Quaternion.identity;

        _animator = _currentCharacter.GetComponent<Animator>();

        Sprite gameIcon = null;
        if(GameFlowManager.Instance.GetNextGame(ref _gameToLaunch, ref gameIcon))
        {
            _icon.sprite = gameIcon;
        }

        _onEnable();
    }

    private void OnDisable()
    {
        _onDisable();

        if (_currentCharacter)
        {
            Destroy(_currentCharacter);
        }
    }

    private void OnMouseDown()
    {
        if(_buttonsLocked)
        {
            return;
        }

        if(!_quoteCloud.activeSelf)
        {
            _quoteCloud.SetActive(true);
            otherCharacter.Switch();            

            //TODO: animation
        }else
        {
            LaunchGame();
        }

        _onMouseDown();
    }

    public void Switch()
    {
        if (_quoteCloud != null)
        {
            _quoteCloud.SetActive(!_quoteCloud.activeSelf);
        }
    }

    public void LockButtons()
    {
        _buttonsLocked = true;

        if (_quoteCloud != null)
        {
            SpriteButton button = _quoteCloud.GetComponent<SpriteButton>();
            button.OnClick.RemoveAllListeners();
        }
    }

    public void UnlockButtons()
    {
        _buttonsLocked = false;

        if (_quoteCloud != null)
        {
            SpriteButton button = _quoteCloud.GetComponent<SpriteButton>();
            button.OnClick.AddListener(LaunchGame);
        }
    }

    private void LaunchGame()
    {
        this.LockButtons();
        otherCharacter.LockButtons();

        GameFlowManager.Instance.GoNextScene(_gameToLaunch);
    }
}

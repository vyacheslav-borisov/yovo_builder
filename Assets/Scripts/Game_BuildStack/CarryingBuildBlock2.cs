using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryingBuildBlock2 : MonoBehaviour
{
    public float shift;

    public delegate void GameEvent();
    public GameEvent OnDropSuccess;
    public GameEvent OnDropFailed;

    private Rigidbody2D _rigidBody;
    private PortalCrane _crane;
    private SpriteButton _button;

    private Transform _hookNode;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.isKinematic = true;

        _button = GetComponent<SpriteButton>();
        if (_button != null)
        {
            _button.OnClick.AddListener(_OnClick);
            _button.Interactable = false;
        }

        _hookNode = transform.Find("hook_node");
        if (_hookNode == null)
        {
            Debug.LogWarning("CarryingBuildBlock hook_node not found!");
            Destroy(gameObject);
        }
    }

    public void OnAttach(PortalCrane crane)
    {
        _crane = crane;
        if (_button != null)
        {
            _button.Interactable = true;
        }
    }

    public void OnDrop()
    {
        _rigidBody.isKinematic = false;
        StartCoroutine(Coroutine_Fall());
    }

    private void _OnClick()
    {
        if (_crane != null)
        {
            _crane.DropCarriedBlock();
        }

        if (_button != null)
        {
            _button.Interactable = false;
        }
    }

    private IEnumerator Coroutine_Fall()
    {
        bool isFailing = false;
        while (true)
        {
            if (transform.position.y < -10.0f)
            {
                EventHandler_OnDropFailed();
                break;
            }

            if (!isFailing && _rigidBody.velocity.magnitude > 0.0f)
            {
                isFailing = true;
            }

            if (isFailing && _rigidBody.velocity == Vector2.zero)
            {
                yield return new WaitForSeconds(1.0f);

                if (_rigidBody.velocity == Vector2.zero)
                {
                    EventHandler_OnDropSuccess();                    
                }

                break;
            }

            yield return null;
        }
    }

    private void EventHandler_OnDropFailed()
    {
        Debug.Log("EventHandler_OnDropFailed");

        if (OnDropFailed != null)
        {
            OnDropFailed();
        }

        Destroy(gameObject);
    }

    private void EventHandler_OnDropSuccess()
    {
        Debug.Log("EventHandler_OnDropSuccess");

        if (OnDropSuccess != null)
        {
            OnDropSuccess();
        }
    }
}

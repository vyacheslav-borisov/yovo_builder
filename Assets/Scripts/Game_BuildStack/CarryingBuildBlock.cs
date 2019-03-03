using System.Collections;
using UnityEngine;

public class CarryingBuildBlock : MonoBehaviour
{
    public int blockID;

    public delegate void GameEvent();
    public GameEvent OnDropSuccess;
    public GameEvent OnDropFailed;

    private Rigidbody2D _rigidBody;
    private PortalCrane  _crane;
    private SpriteButton _button;

    private Transform _hookNode;   

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.isKinematic = true;

        _button = GetComponent<SpriteButton>();
        if(_button != null)
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
        if(_crane != null)
        {
            _crane.DropCarriedBlock();
        }        
    }

    private IEnumerator Coroutine_Fall()
    {
        bool isFailing = false;
        while(true)
        {
            if(transform.position.y < -10.0f)
            {
                EventHandler_OnDropFailed();
                break;
            }

            if(!isFailing && _rigidBody.velocity.magnitude > 0.0f)
            {
                isFailing = true;
            }

            if(isFailing && _rigidBody.velocity == Vector2.zero)
            {
                yield return new WaitForSeconds(1.0f);

                if (_rigidBody.velocity == Vector2.zero)
                {
                    var buildSection = CheckBuildSection();
                    if (buildSection != null)
                    {
                        EventHandler_OnDropSuccess(buildSection);
                    }else
                    {
                        EventHandler_OnDropFailed();
                    }
                }

                break;
            }

            yield return null;
        }        
    }

    private void EventHandler_OnDropFailed()
    {
        Debug.Log("EventHandler_OnDropFailed");

        if(OnDropFailed != null)
        {
            OnDropFailed();
        }

        Destroy(gameObject);
    }

    private void EventHandler_OnDropSuccess(BuildSection section)
    {
        Debug.Log("EventHandler_OnDropSuccess");

        section.OnAppear += EventHandler_OnTargetBlockAppeared;
        section.CurrentState = BuildSection.State.Complete;

        var renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = false;
    }

    private void EventHandler_OnTargetBlockAppeared()
    {
        if (OnDropSuccess != null)
        {
            OnDropSuccess();
        }

        Destroy(gameObject);
    }

    private BuildSection CheckBuildSection()
    {
        var point3D = transform.position + (_hookNode.position - transform.position) * 0.5f;
        var point2D = new Vector2();
        point2D.x = point3D.x;
        point2D.y = point3D.y;

        var collider = Physics2D.OverlapPoint(point2D, 1 << LayerMask.NameToLayer("Raycast"));
        if (collider != null)
        {
            Debug.Log("hitted: " + collider.gameObject.name);

            if(collider.gameObject.tag != "BuildSection")
            {
                Debug.Log("hitted object is not build section");
                return null;
            }

            Debug.Log("build section: " + collider.transform.parent.gameObject.name);

            var buildSection = collider.GetComponentInParent<BuildSection>();
            if (buildSection == null)
            {
                Debug.Log("raycasted object does not contain BuildSection component");
                return null;
            }

            if (buildSection.blockID != blockID)
            {
                Debug.Log("target build section ID is not match with one in carryng block");
                Debug.Log("carring block ID = " + blockID);
                Debug.Log("build section ID = " + buildSection.blockID);

                return null;
            }

            return buildSection;
        }
        else
        {
            Debug.Log("no objects raycasted");
        }

        return null;
    }
}

using UnityEngine;

public class SceneAdaptableItem : MonoBehaviour
{
	void Start ()
    {
        Debug.Log("adapt item: " + gameObject.name);

        Vector3 position = transform.localPosition;
        position.x += (transform.parent.position.x < 0.0f) ? GameFlowManager.ScreenAdaptableOffset : -GameFlowManager.ScreenAdaptableOffset;
        transform.localPosition = position;
    }
}

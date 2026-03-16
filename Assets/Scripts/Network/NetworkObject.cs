using UnityEngine;

[System.Serializable]
public class NetworkObject
{
    public string id;
    public GameObject gameObject;
    public Vector3 position;
    public Quaternion rotation;
    public bool isVisible;
    
    public NetworkObject(string objectId, GameObject obj)
    {
        id = objectId;
        gameObject = obj;
        position = obj.transform.position;
        rotation = obj.transform.rotation;
        isVisible = obj.activeSelf;
    }
    
    public void SetVisible(bool visible)
    {
        isVisible = visible;
        if (gameObject != null)
            gameObject.SetActive(visible);
    }
    
    public void UpdatePosition(Vector3 newPosition)
    {
        position = newPosition;
        if (gameObject != null)
            gameObject.transform.position = newPosition;
    }
    
    public void UpdateRotation(Quaternion newRotation)
    {
        rotation = newRotation;
        if (gameObject != null)
            gameObject.transform.rotation = newRotation;
    }
}
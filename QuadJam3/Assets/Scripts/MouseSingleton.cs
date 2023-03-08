using UnityEngine;

public class MouseSingleton : MonoBehaviour
{
    //4:01 in this video, establishing for combat https://www.youtube.com/watch?v=AXkaqW3E9OI

    public static MouseSingleton mouseSingleton { get; private set; }

    void Awake()
    {
        if (mouseSingleton != null && mouseSingleton != this)
        {
            Destroy(this);
        }
        else
        {
            mouseSingleton = this;
        }
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }

    public static Vector3 GetMousePosition() => Input.mousePosition;

    public static Vector2 GetMouseWheelValue() => Input.mouseScrollDelta;

    public static Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }

    public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }

    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
}
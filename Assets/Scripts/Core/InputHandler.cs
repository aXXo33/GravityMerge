using UnityEngine;
public static class InputHandler
{
    public static bool GetInputDown() { return Input.GetMouseButtonDown(0); }
    public static bool GetInput() { return Input.GetMouseButton(0); }
    public static bool GetInputUp() { return Input.GetMouseButtonUp(0); }
    public static Vector3 GetWorldPosition(Camera cam=null) { if(!cam)cam=Camera.main; Vector3 p=Input.mousePosition; p.z=Mathf.Abs(cam.transform.position.z); return cam.ScreenToWorldPoint(p); }
}
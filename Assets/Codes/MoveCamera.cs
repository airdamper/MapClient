using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour 
{
    public Vector3 direction 
    {
        get
        {
            return deltaPosition.normalized * 3;
        }
    }
    Vector3 deltaPosition = Vector3.zero;
 
    Transform cameraTransform;
    Vector3 lastPosition = Vector3.zero;
    public bool dragging = false;
    float minX, minY, maxX, maxY;
    float speed;
   

	void Start () 
    {
        cameraTransform = transform;
        speed = GetComponent<Camera>().orthographicSize * 2 / Screen.height;
        ResetRange();
	}
	void Update () 
    {
        Down();
        Up();
        if (dragging)
        {
            deltaPosition = Input.mousePosition - lastPosition;
            lastPosition = Input.mousePosition;
        }
	}
    void LateUpdate()
    {
        if (dragging)
        {
            Vector3 cameraPosition = cameraTransform.position;
            cameraPosition -= new Vector3(deltaPosition.x, deltaPosition.y, 0) * speed;
            cameraPosition.x = Mathf.Clamp(cameraPosition.x, minX, maxX);
            cameraPosition.y = Mathf.Clamp(cameraPosition.y, minY, maxY);
            cameraTransform.position = cameraPosition;
        }
    }
    void ResetRange()
    {
        //0.5-99.5
        float h = GetComponent<Camera>().orthographicSize;
        float w = h * GetComponent<Camera>().aspect;
        minX = -0.5f + w;
        maxX = 99.5f - w;
        minY = -0.5f + h;
        maxY = 99.5f - h;
    }
    void Down()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastPosition = Input.mousePosition;
            dragging = true;
        }
    }
    void Up()
    {
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }
    }
}

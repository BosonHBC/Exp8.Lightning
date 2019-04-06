using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement instance;    
    private void Awake()
    {
        if(instance !=this || instance == null)
        instance = this;
    }

    public LightningBody lightningBody;
    [SerializeField] float zFactor;
    Vector3 nearPoint;
    Vector2 mousePos;
    Camera cam;
    Vector3 worldPoint;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 destPos = lightningBody.GetEndPosition() + new Vector3(0, 0, -10 - zFactor * lightningBody.GetABDistance());

        transform.position = destPos;//Vector3.Lerp(transform.position, destPos, 2f * Time.deltaTime);
    }

    public Vector3 GetCursorInWorldPoint()
    {
        return worldPoint;
    }

    private void OnGUI()
    {
        nearPoint = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
   Input.mousePosition.y, Camera.main.nearClipPlane));

        Ray ray = new Ray(transform.position, (nearPoint - transform.position).normalized);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, -transform.position.z + 5f))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            worldPoint = hit.point;
        }


        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + cam.pixelWidth + ":" + cam.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + nearPoint.ToString("F3"));
        GUILayout.EndArea();
    }
}

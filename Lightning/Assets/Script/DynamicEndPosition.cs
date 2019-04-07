using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicEndPosition : MonoBehaviour
{
    [SerializeField] LightningBody ltBody;
    private bool bMoving;
    [SerializeField] private float fMoveSpd;
    private float fDistThreshold = 1f;

    // Start is called before the first frame update
    void Start()
    {
        ltBody.SetEndPosition(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //ltBody.destPosition = transform.position;
        MovePoint();

        if (bMoving)
        {
            Vector3 destPos = CameraMovement.instance.GetCursorInWorldPoint();
            transform.position = Vector3.Lerp(transform.position, destPos, fMoveSpd* Time.deltaTime);
            ltBody.SetEndPosition(transform.position);
            ltBody.SetLineTangent();
            Debug.DrawLine(transform.position, destPos, Color.red);
            if (Vector3.Distance(transform.position, destPos) < fDistThreshold)
                bMoving = false;
        }
    }
    void MovePoint()
    {
        float hori= Input.GetAxisRaw("Mouse X");
        float vert = Input.GetAxisRaw("Mouse Y");

       
        if (hori != 0 || vert != 0)
        {
            bMoving = true;
           
        }
    }

   
}

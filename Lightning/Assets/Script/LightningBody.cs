using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBody : MonoBehaviour
{
    LineRenderer lr;
    public int iLevel;
    public Vector3 headPosition;
    public Vector3 destPosition;
    [SerializeField] private int[] iTurnCountRange;
    [SerializeField] private float[] fTurnDistRange;
    private bool[] ba_canMove;
    private int iTurnCount;
    [SerializeField] float[] fLightingRefeshRateRange;
    // Start is called before the first frame update
    private void OnEnable()
    {
        lr = GetComponent<LineRenderer>();

    }
    void Start()
    {
        if (headPosition == destPosition)
        {
            Debug.LogError("this is same point, no meaning");
            return;
        }
        
        ba_canMove = new bool[30] ;
        for (int i = 0; i < ba_canMove.Length; i++)
        {
            ba_canMove[i] = true;
        }
        SetLineTangent();
    }

    public void SetLineTangent()
    {
        Vector3 deltaV3 = destPosition - headPosition;
        iTurnCount = (int)(deltaV3.magnitude / Random.Range(iTurnCountRange[0], iTurnCountRange[1]));
        iTurnCount = Mathf.Clamp(iTurnCount, 4, 25);
        lr.positionCount = iTurnCount;
        // Init turn position in line;
        
        Vector3 dir = deltaV3.normalized;
        float deltaDist = deltaV3.magnitude / iTurnCount;
        Vector3 collpasePos = headPosition;
        lr.SetPosition(0, collpasePos);
        
        for (int i = 1; i < iTurnCount-2; i++)
        {
            collpasePos += dir * deltaDist;
//            lr.SetPosition(i, collpasePos);
            Vector3 newPosX = /*lr.GetPosition(Random.Range(0, 2) == 0 ? i : i - 1)*/collpasePos + Random.Range(-1, 2) * Random.Range(fTurnDistRange[0], fTurnDistRange[1]) * Vector3.right;
            Vector3 newPos = new Vector3(newPosX.x, collpasePos.y, 0);
            if (ba_canMove[i])
            {
                StartCoroutine(ie_LrPointMover(i, newPos, Random.Range(fLightingRefeshRateRange[0], fLightingRefeshRateRange[1])));
            }

        }
        lr.SetPosition(iTurnCount - 2, destPosition - dir * deltaDist);
        lr.SetPosition(iTurnCount - 1, destPosition);
    }

         
    public Vector3 GetEndPosition()
    {
        return (lr.GetPosition(0) + lr.GetPosition(iTurnCount-1))/2f;
    }
    public float GetABDistance()
    {
        return Vector3.Distance(headPosition, destPosition);
    }

    IEnumerator ie_LrPointMover(int i, Vector3 _dest, float _duration)
    {
        ba_canMove[i] = false;
        Vector3 startPos = lr.GetPosition(i);
        float startTime = Time.time;
        float currentTime = startTime;

        float lerp = (currentTime - startTime) / _duration;
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        while (lerp <= 1)
        {
            currentTime = Time.time;
            lerp = (currentTime - startTime) / _duration;
            if (i < iTurnCount)
                lr.SetPosition(i, Vector3.Lerp(startPos, _dest, lerp));
            else
                lerp = 1;
            yield return wait;
        }
        ba_canMove[i] = true;
    }
}

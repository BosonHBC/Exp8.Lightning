using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBody : MonoBehaviour
{
    LineRenderer lr;
    public int iLevel;
    [Header("Point Ref")]
    public Vector3[] refPositions = new Vector3[2];
    [SerializeField] private Vector3 middlePointOffset;
    [SerializeField] private Vector3 endPointOffset;
    [Header("Range Ref")]
    [SerializeField] private int[] iTurnCountRange;
    [SerializeField] private float[] fTurnDistRange;
    private bool[] ba_canMove;
    private int iTurnCount;
    private int iPrevTurnCount;
    [SerializeField] float[] fLightingRefeshRateRange;
    private int maxPoints = 30;
    public int score;

    [Header("Child")]
    [SerializeField] private GameObject lightningPrefab;
    private List<LightningBody> childLightnings = new List<LightningBody>();
    private int iMaxChildCount = 3;
    private int iCurrentChildCount = 0;
    private int[] turnToNewChild;
    public int allowedChildCount = 0;

    [Header("Rendering")]
    public float fEmmisiveIntensity = 1;
    private float fIntensityOffset = 0;
    Vector4 emColor;
    Material m_Lightning;
    Animator anim;
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        lr = GetComponent<LineRenderer>();
        anim = GetComponent<Animator>();
        
    }
    void Start()
    {
        iPrevTurnCount = 0;
        iTurnCount = 0;
        turnToNewChild = new int[iMaxChildCount];
        Cursor.visible = false;
        //for (int i = 0; i < iMaxChildCount; i++)
        //{
        //    int ave = maxPoints * (i + 1) / (iMaxChildCount * 2);
        //    turnToNewChild[i] = Random.Range(ave - 5, ave-3);
        //    Debug.Log("trun to new: " + turnToNewChild[i]);
        //}

        turnToNewChild[0] = Random.Range(5, 9);
        turnToNewChild[1] = Random.Range(11, 15);
        turnToNewChild[2] = Random.Range(17, 21);


        ba_canMove = new bool[maxPoints];
        for (int i = 0; i < ba_canMove.Length; i++)
        {
            ba_canMove[i] = true;
        }
        SetLineTangent();


        m_Lightning = GetComponent<LineRenderer>().material;
        emColor = m_Lightning.GetVector("_EmissionColor");
        PlayShinning();
        
    }

    private void Update()
    {
        if (m_Lightning)
            m_Lightning.SetVector("_EmissionColor", emColor * (fEmmisiveIntensity + fIntensityOffset));
    }

    public void AccquireAtom()
    {
        fIntensityOffset += 0.1f;
    }

    public int ChargeToCloud()
    {
        fIntensityOffset = 0;
        int res = score;
        score = 0;
        return res;
    }

    public void SetLineTangent()
    {
        Vector3 deltaV3 = refPositions[refPositions.Length - 1] - refPositions[0];
        iPrevTurnCount = iTurnCount;
        iTurnCount = (int)(deltaV3.magnitude / Random.Range(iTurnCountRange[0], iTurnCountRange[1]));
        iTurnCount = Mathf.Clamp(iTurnCount, 4, maxPoints);
        lr.positionCount = iTurnCount;
        // Init turn position in line;

        Vector3 dir = deltaV3.normalized;
        float deltaDist = deltaV3.magnitude / iTurnCount;
        Vector3 collpasePos = refPositions[0];
        lr.SetPosition(0, collpasePos);

        for (int i = 1; i < iTurnCount - 2; i++)
        {
            collpasePos += dir * deltaDist;
            Vector3 besierPoint = CalculateQuadraticBezierPoint((float)i / (float)(iTurnCount - 2), refPositions[0], refPositions[1], refPositions[2]);

            Vector3 newPosX = /*lr.GetPosition(Random.Range(0, 2) == 0 ? i : i - 1)collpasePos*/besierPoint + Random.Range(-1, 2) * Random.Range(fTurnDistRange[0], fTurnDistRange[1]) * Vector3.right;
            Vector3 newPos = new Vector3(newPosX.x, besierPoint.y, 0);
            if (ba_canMove[i])
            {
                StartCoroutine(ie_LrPointMover(i, newPos, Random.Range(fLightingRefeshRateRange[0], fLightingRefeshRateRange[1])));
            }

        }
        lr.SetPosition(iTurnCount - 2, refPositions[refPositions.Length - 1] - dir * deltaDist);
        lr.SetPosition(iTurnCount - 1, refPositions[refPositions.Length - 1]);

        if (iLevel >= 1)
            return;

        for (int i = 0; i < iMaxChildCount; i++)
        {
            if (iPrevTurnCount < iTurnCount)
            {
                // add 
                if (iTurnCount == turnToNewChild[i] && iCurrentChildCount == i && iCurrentChildCount < allowedChildCount)
                    AddNewChild();
            }
            else if (iPrevTurnCount > iTurnCount)
            {
                // remove
                if (iTurnCount == turnToNewChild[i] && iCurrentChildCount == i + 1)
                    RemoveChild();
            }
        }


    }

    public void PlayShinning()
    {
        anim.Play("Shining");
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        //res = (1-t)^2 * p0 + 2(1-t)t * p1 + t*2 * p2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }

    public void SetEndPosition(Vector3 _newDest)
    {
        refPositions[refPositions.Length - 1] = _newDest + endPointOffset;
        refPositions[1] = (_newDest + refPositions[0]) / 2f + middlePointOffset;

        if (childLightnings.Count > 0 && iLevel < 1)
            for (int i = 0; i < childLightnings.Count; i++)
            {
                int _requireID = turnToNewChild[i] - 1;
                // Debug.Log("Required: " + _requireID + ", Actual Count: " + lr.positionCount);
                if (_requireID < lr.positionCount)
                {
                    childLightnings[i].SetStartPosition(lr.GetPosition(_requireID));
                    float _value = turnToNewChild[i] / (float)iTurnCount;
                    childLightnings[i].SetCurve(_value);
                }
                childLightnings[i].SetEndPosition(_newDest);
                childLightnings[i].SetLineTangent();

            }
    }
    public void SetStartPosition(Vector3 _newDest)
    {
        refPositions[0] = _newDest;
        refPositions[1] = (_newDest + refPositions[refPositions.Length - 1]) / 2f + middlePointOffset;
    }

    public void SetCurve(float _value)
    {
        lr.startWidth = _value;
    }

    public void SetMiddlePointOffset(int _parentCurrentChild)
    {
        bool isEven = _parentCurrentChild % 2 == 0;
        switch (isEven)
        {
            case true:
                middlePointOffset = Vector3.left * Random.Range(30f, 45f) / (float)(iLevel + 1f);
                //endPointOffset = Vector3.left * Random.Range(0.2f, 0.4f);
                break;
            case false:
                middlePointOffset = Vector3.right * Random.Range(30f, 45f) / (float)(iLevel + 1f);
                // endPointOffset = Vector3.right * Random.Range(0.2f, 0.4f);
                break;
            default:
                middlePointOffset = Vector3.zero;
                break;
        }

    }

    private void AddNewChild()
    {
        if (iCurrentChildCount < iMaxChildCount)
        {
            LightningBody _body = Instantiate(lightningPrefab).GetComponent<LightningBody>();

            _body.transform.parent = transform;
            childLightnings.Add(_body);

            Debug.Log(name + " currentChild: " + iCurrentChildCount);
            iCurrentChildCount++;
            // set start position
            _body.SetStartPosition(lr.GetPosition(iTurnCount - 1));
            _body.SetEndPosition(refPositions[refPositions.Length - 1]);
            _body.iLevel = iLevel + 1;
            _body.SetMiddlePointOffset(iCurrentChildCount);
            _body.name = name + "_Level" + _body.iLevel + "_Child" + iCurrentChildCount;


        }

    }
    private void RemoveChild()
    {
        if (iCurrentChildCount > 0)
        {
            LightningBody _poped = childLightnings[childLightnings.Count - 1];
            childLightnings.Remove(_poped);
            Destroy(_poped.gameObject);
            iCurrentChildCount--;
        }

    }

    public Vector3 GetEndPosition()
    {
        return (lr.GetPosition(0) + lr.GetPosition(iTurnCount - 1)) / 2f;
    }
    public float GetABDistance()
    {
        return Vector3.Distance(refPositions[0], refPositions[refPositions.Length - 1]);
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

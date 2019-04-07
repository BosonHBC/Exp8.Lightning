using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] int maxCharge;
    int currentCharge;
    Vector4 _color;
    Material m_cloud;
    // Start is called before the first frame update
    void Start()
    {
        m_cloud = GetComponent<MeshRenderer>().material;
        _color = m_cloud.GetVector("_Color");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            ParticleAttractor attractor = other.GetComponent<ParticleAttractor>();
            currentCharge += attractor.body.ChargeToCloud();

            float percent = (float)currentCharge / (float)maxCharge;

            if (percent >= 1)
                percent = 1;
            else
            {
                _color = new Vector4(1f - percent, 1f - percent, 1f - percent, _color.w);
                //Debug.Log("percent:" + percent + " color " + _color);
                m_cloud.SetVector("_Color", _color);
            }

           // GetComponent<MeshRenderer>().material = m_cloud;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAttractor : MonoBehaviour
{
    [SerializeField] ParticleSystem emmiter;
    ParticleSystem.Particle[] particles;
    // Start is called before the first frame update

    [SerializeField] private float affectDistance = 3;
    public LightningBody body;
    bool bInit;
    int particlesCount;
    void Start()
    {
        particlesCount = (int)(emmiter.emission.rateOverTime.constant * emmiter.main.duration);
        particles = new ParticleSystem.Particle[particlesCount];
        // body.score = -particlesCount;
        bInit = false;

        Invoke("CanCheck", 1f);
    }

    void CanCheck()
    {
        bInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(bInit)
        {
            int currentCount = emmiter.GetParticles(particles);

            float dist;
            for (int i = 0; i < currentCount; i++)
            {
                dist = Vector3.Distance(transform.position, particles[i].position);
                if (dist < affectDistance)
                {
                    particles[i].position = Vector3.Lerp(particles[i].position, transform.position, 2 * Time.deltaTime);
                    float XYDist = Vector2.Distance(transform.position, particles[i].position);
                    if (XYDist < 1f)
                    {
                        body.score++;
                        if (body.score >= 0)
                        {
                            Debug.Log("score: " + body.score);
                            body.PlayShinning();
                            body.AccquireAtom();
                            if (body.score == 10 && body.allowedChildCount == 0)
                                body.allowedChildCount = 1;
                            else if (body.score == 20 && body.allowedChildCount == 1)
                                body.allowedChildCount = 2;
                            else if (body.score == 30 && body.allowedChildCount == 2)
                                body.allowedChildCount = 2;
                        }

                        particles[i].remainingLifetime = 0f;
                    }
                }
            }
            emmiter.SetParticles(particles);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, affectDistance);
    }

}

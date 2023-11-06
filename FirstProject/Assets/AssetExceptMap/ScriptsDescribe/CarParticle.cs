using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarParticle : MonoBehaviour
{
    public bool letPlay = false;
    ParticleSystem carPS;
    // Start is called before the first frame update
    void Start()
    {
        carPS = GetComponent<ParticleSystem>();
    }

    public void setParticleActive()
    {
        if (letPlay == true)
        {
            if (!carPS.isPlaying)
                carPS.Play();
        }
        else
        {
            if (carPS.isPlaying)
                carPS.Stop();
        }
    }
}

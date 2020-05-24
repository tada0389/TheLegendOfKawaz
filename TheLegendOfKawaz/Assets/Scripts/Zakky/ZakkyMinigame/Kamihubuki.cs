using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamihubuki : MonoBehaviour
{
    ParticleSystem particle;
    // Start is called before the first frame update
    void Start()
    {
        particle = this.GetComponent<ParticleSystem>();
        particle.Stop();
    }

    // Update is called once per frame
    void Update()
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinYure : MonoBehaviour
{
    // Start is called before the first frame update
    public float ω;
    public float A;
    private float θ;
    void Start()
    {
        θ = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        θ += ω * Time.deltaTime;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, A * Mathf.Sin(Mathf.Sin(θ))));
    }

    
}

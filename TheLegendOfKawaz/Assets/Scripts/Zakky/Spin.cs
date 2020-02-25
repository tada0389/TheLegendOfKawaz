using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float ω;
    public float A;
    private float θ;
    private Vector3 vec;
    // Start is called before the first frame update
    void Start()
    {
        θ = 0f;
        vec = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        θ += ω + Time.deltaTime;
        transform.position = new Vector3(vec.x + A * Mathf.Cos(θ), vec.y + A * Mathf.Sin(θ), 0f);
    }
}

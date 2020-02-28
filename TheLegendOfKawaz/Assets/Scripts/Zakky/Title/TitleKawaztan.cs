using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleKawaztan : MonoBehaviour
{
    public float ω;
    public float A;
    private float θ;
    // Start is called before the first frame update
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

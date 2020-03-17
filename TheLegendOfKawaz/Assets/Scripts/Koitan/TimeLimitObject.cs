using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLimitObject : MonoBehaviour
{
    public float lifeTime;
    private float time = 0;

    private void Awake()
    {
        Init(Vector3.zero);
    }

    public void Init(Vector3 pos)
    {
        time = 0;
        transform.position = pos;
    }

    private void Update()
    {
        if (time >= lifeTime)
        {
            gameObject.SetActive(false);
        }
        time += Time.deltaTime;
    }
}

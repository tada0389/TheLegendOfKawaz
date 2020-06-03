using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haikei : MonoBehaviour
{
    GameObject camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vec = camera.transform.position;
        vec.z = 0f;
        transform.position = vec;
    }
}

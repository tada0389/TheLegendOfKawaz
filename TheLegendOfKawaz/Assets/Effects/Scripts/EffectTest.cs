using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTest : MonoBehaviour
{
    [SerializeField]
    ParticleSystem shotEff;
    [SerializeField]
    ParticleSystem hitEff;
    [SerializeField]
    GameObject tama;
    [SerializeField]
    private float lifeTime = 1f;
    [SerializeField]
    private float vx = 0;
    private float time;
    private bool isActive;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {        
        if (time > 0)
        {
            time -= Time.deltaTime;            
        }
        else if(isActive)
        {
            isActive = false;
            tama.SetActive(false);
            hitEff.transform.position = tama.transform.position;
            hitEff.Play();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isActive = true;
            tama.SetActive(true);            
            tama.transform.position = Vector3.zero;
            time = lifeTime;
            shotEff.Play();
        }
        tama.transform.Translate(vx * Time.deltaTime, 0, 0);        
    }
}

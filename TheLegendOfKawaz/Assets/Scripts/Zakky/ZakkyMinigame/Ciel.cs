using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ciel : MonoBehaviour
{
    [SerializeField]
    BombSpawner m_bombSpawnerScript;
    //private float fiverTime;
    [SerializeField]
    GameObject[] ciels;
    List<Renderer> cielsRenderers;
    // Start is called before the first frame update
    void Start()
    {
        cielsRenderers = new List<Renderer>();
        //fiverTime = 0f;
        for (int i = 0; i < ciels.Length; i++)
        {
            cielsRenderers.Add(ciels[i].GetComponent<Renderer>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        //fiverTime -= Time.deltaTime;
        //if (m_bombSpawnerScript.brokenBombsSum >= 20)
        //{
        //    
        //}
        if (Game.instance.state == Game.STATE.MOVE)
        {
            for (int i = 0; i < ciels.Length; i++)
            {
                cielsRenderers[i].material.color = new Color(0.6f, 0.6f, 1f);
            }
        }
        else if (Game.instance.state == Game.STATE.FEVER)
        {
            for (int i = 0; i < ciels.Length; i++)
            {
                cielsRenderers[i].material.color = Color.yellow;
            }
        }
    }

    public void CielRespawner()
    {
        //m_bombSpawnerScript.brokenBombsSum = 0;
        for (int i = 0; i < ciels.Length; i++)
        {
            ciels[i].SetActive(true);
        }
        //fiverTime = 10f;
    }
}

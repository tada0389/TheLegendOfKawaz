using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ciel : MonoBehaviour
{
    [SerializeField]
    private BombSpawner m_bombSpawnerScript;
    //private float fiverTime;
    [SerializeField]
    private GameObject[] ciels;
    // Start is called before the first frame update
    void Start()
    {
        //fiverTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //fiverTime -= Time.deltaTime;
        //if (m_bombSpawnerScript.brokenBombsSum >= 20)
        //{
        //    
        //}
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

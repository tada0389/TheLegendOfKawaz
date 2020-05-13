using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPostVol : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    //[SerializeField]
    //CanvasGroup group = null;

    [SerializeField]
    Fade fade = null;

    //public GameObject titleState;

    // Start is called before the first frame update
    //void Start()
    //{
    //}

    // Update is called once per frame
    //void Update()
    //{
    //}

    public void Fadeout(string nextScene)
    {
        fade.FadeIn(1, () =>
        {
            if (nextScene == null)
            {
                Application.Quit();
                return;
            }
                
            //scene遷移
            SceneManager.LoadScene(nextScene);
        });
    }
}

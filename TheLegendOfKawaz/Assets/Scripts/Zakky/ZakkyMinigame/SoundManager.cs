using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WallDefence;

public class SoundManager : MonoBehaviour
{
    AudioSource audioSource;
    Game.STATE prevState;

    [SerializeField]
    AudioClip[] audioClips = new AudioClip[2];

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        prevState = Game.instance.state;
    }

    // Update is called once per frame
    void Update()
    {
        if (prevState != Game.instance.state)
        {
            audioSetter(Game.instance.state);
            prevState = Game.instance.state;
        }
    }

    void audioSetter(Game.STATE state)
    {
        if (state == Game.STATE.FEVER) audioSource.clip = audioClips[1];
        else audioSource.clip = audioClips[0];
        audioSource.Play();
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WallDefence;

public class SoundManager : MonoBehaviour
{
    AudioSource audioSource;
    Game.STATE prevState;
    Dictionary<int, int> soundDictionary;

    [SerializeField]
    AudioClip[] audioClips = new AudioClip[2];
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        prevState = Game.instance.state;
        soundDictionary = new Dictionary<int, int>();
        soundDictionary.Add((int)Game.STATE.MOVE, 0);
        soundDictionary.Add((int)Game.STATE.FEVER, 1);
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
        audioSource.Stop();
        audioSource.clip = audioClips[soundDictionary[(int)state]];
        audioSource.Play();
    }
}

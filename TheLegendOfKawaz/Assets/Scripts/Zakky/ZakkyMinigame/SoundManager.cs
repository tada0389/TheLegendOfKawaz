using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WallDefence;

public class SoundManager : MonoBehaviour
{
    AudioSource audioSource;
    //別のステートに切り替わったときに処理するための変数
    Game.STATE prevState;

    Dictionary<int, int> soundDictionary;

    //音声データ(もっとインデックスをわかりやすくしたい)
    [SerializeField]
    AudioClip[] audioClips = new AudioClip[2];
    
    // Start is called before the first frame update
    void Start()
    {
        //必要なもんぶち込む
        audioSource = GetComponent<AudioSource>();
        prevState = Game.instance.state;
        
        //Dictionaryのインスタンス生成
        soundDictionary = new Dictionary<int, int>();

        //audioClipsのインデックスをStateと関連づける
        soundDictionary.Add((int)Game.STATE.MOVE, 0);
        soundDictionary.Add((int)Game.STATE.FEVER, 1);
    }

    // Update is called once per frame
    void Update()
    {
        //別のステートに切り替わったとき
        if (prevState != Game.instance.state)
        {
            //新しくAudioClipを入れて再生
            audioSetter(Game.instance.state);
            //前のステートを更新
            prevState = Game.instance.state;
        }
    }

    void audioSetter(Game.STATE state)
    {
        // by tada
        if (!soundDictionary.ContainsKey((int)state)) return;

        //一旦再生を止める
        audioSource.Stop();
        //ここDictionaryのKeyになかった時の処理書いた方がいいかも
        audioSource.clip = audioClips[soundDictionary[(int)state]];
        //新しいの再生
        audioSource.Play();
    }
}

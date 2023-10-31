using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private AudioSource _audioSource;

    
    // 몬스터 사운드
    public Dictionary<string, AudioClip> playSound = new Dictionary<string, AudioClip> ();

    public AudioClip buttonSound;

    void Awake()
    {
        // 이미 인스턴스가 있는지 확인합니다.
        if (Instance == null)
        {
            Instance = this;
            this._audioSource = this.GetComponent<AudioSource>();
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // 중복되는 인스턴스가 있는 경우, 이 게임 객체를 파괴합니다.
            Destroy(this.gameObject);
        }

    }

    void Start() {
        buttonSound = Resources.Load<AudioClip>("wav/background/button_push");
      
        soundAdd("teller","app");
        soundAdd("teller","death");
        soundAdd("teller","move");
        soundAdd("teller","shoot");
        soundAdd("wanderer","att");
        soundAdd("wanderer","death");
        soundAdd("wanderer","move");
        soundAdd("wanderer","shoot");
       
    }

    void soundAdd(string spritesName, string soundType) {
        string wavName = "wav/monster/"+spritesName+"/"+spritesName+"_"+soundType;
        playSound.Add(spritesName+"_"+soundType, Resources.Load<AudioClip>(wavName));
    }

    public void BackGroundPlay(string newAudioName)
    {
        string audioClipPath = "wav/background/"+newAudioName;
        AudioClip clip = Resources.Load<AudioClip>(audioClipPath);
       
        this._audioSource.clip = clip;
        this._audioSource.loop = true;
        this._audioSource.Play();
    }

    public void playSoundEffect(AudioClip clip, AudioSource inAudioSource)
    {
        inAudioSource.clip = clip;
        inAudioSource.Play();
    }
}

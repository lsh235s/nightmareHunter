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
        backSound();
        monsterSound();
        summonSound();   
    }

    void backSound() {
        backSoundAdd("gem");
        backSoundAdd("coin");
        backSoundAdd("place");
        backSoundAdd("get");
        backSoundAdd("over");
        backSoundAdd("win");
    }

    void monsterSound() {
        monsterSoundAdd("bombflower","att");
        monsterSoundAdd("bombflower","death");
        monsterSoundAdd("bombflower","move");
        monsterSoundAdd("bombflower","shoot");

        monsterSoundAdd("darkone","att");
        monsterSoundAdd("darkone","death");
        monsterSoundAdd("darkone","move");
        monsterSoundAdd("darkone","shoot");

        monsterSoundAdd("evil","att");
        monsterSoundAdd("evil","death");
        monsterSoundAdd("evil","shoot");

        monsterSoundAdd("flydra","att");
        monsterSoundAdd("flydra","div");
        monsterSoundAdd("flydra","move");
        monsterSoundAdd("flydra","shoot");

        monsterSoundAdd("helldog","app");
        monsterSoundAdd("helldog","death");
        monsterSoundAdd("helldog","move");
        monsterSoundAdd("helldog","shoot");

        monsterSoundAdd("stiller","att");
        monsterSoundAdd("stiller","move");
        monsterSoundAdd("stiller","shoot");

        monsterSoundAdd("teller","app");
        monsterSoundAdd("teller","death");
        monsterSoundAdd("teller","move");
        monsterSoundAdd("teller","shoot");

        monsterSoundAdd("wanderer","att");
        monsterSoundAdd("wanderer","death");
        monsterSoundAdd("wanderer","move");
        monsterSoundAdd("wanderer","shoot");
    }



    void summonSound() {
        summonSoundAdd("archer","att");
        summonSoundAdd("exorcist","att");
        summonSoundAdd("gostbuste","att");
        summonSoundAdd("hunter","att");
        summonSoundAdd("icewitch","att");
        summonSoundAdd("monk","att");
        summonSoundAdd("priest","att");
        summonSoundAdd("shaman","att");
    }

    void monsterSoundAdd(string spritesName, string soundType) {
        string wavName = "wav/monster/"+spritesName+"/"+spritesName+"_"+soundType;
  
        playSound.Add(spritesName+"_"+soundType, Resources.Load<AudioClip>(wavName));
    }

    void summonSoundAdd(string spritesName, string soundType) {
        string wavName = "wav/summor/"+spritesName+"/"+spritesName+"_"+soundType;
  
        playSound.Add(spritesName+"_"+soundType, Resources.Load<AudioClip>(wavName));
    }

    void backSoundAdd(string spritesName) {
        string wavName = "wav/background/"+spritesName;
  
        playSound.Add(spritesName, Resources.Load<AudioClip>(wavName));
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

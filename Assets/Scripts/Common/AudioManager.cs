using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private AudioSource _audioSource;

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

        // 병경 음악 정리
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
    }

    public void BackGroundPlay(string newAudioName)
    {
        string audioClipPath = "wav/background/"+newAudioName;
        AudioClip clip = Resources.Load<AudioClip>(audioClipPath);
       
        this._audioSource.clip = clip;
        this._audioSource.loop = true;
        this._audioSource.Play();
    }
}

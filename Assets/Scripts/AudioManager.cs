using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class AudioData
    {
        [SerializeField]
        private string mKey = null;
        public string Key
        {
            get { return mKey; }
            set { mKey = value; }
        }
        [SerializeField]
        private AudioClip mClip = null;
        public AudioClip Clip
        {
            get { return mClip; }
            set { mClip = value; }
        }
    }

    private static AudioManager mInstance = null;
    public static AudioManager Inst
    {
        get
        {
            if(mInstance == null)
            {
                mInstance = FindObjectOfType<AudioManager>();
                DontDestroyOnLoad(mInstance.gameObject);
            }
            return mInstance;
        }
    }

    public List<AudioData> SEList = new List<AudioData>();
    public List<AudioData> BGMList = new List<AudioData>();

    private Dictionary<string, AudioClip> mCachedSEList = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> mCachedBGMList = new Dictionary<string, AudioClip>();

    private AudioSource mCachedAudioSource = null;
    private AudioSource mAudioSource
    {
        get
        {
            if(mCachedAudioSource == null)
            {
                mCachedAudioSource = GetComponent<AudioSource>();
            }
            return mCachedAudioSource;
        }
    }

    private void Awake()
    {
        if(mInstance == null)
        {
            mInstance = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach(var data in SEList)
        {
            if (string.IsNullOrEmpty(data.Key) == false)
            {
                mCachedSEList[data.Key] = data.Clip;
            }
        }
        foreach (var data in BGMList)
        {
            if (string.IsNullOrEmpty(data.Key) == false)
            {
                mCachedBGMList[data.Key] = data.Clip;
            }
        }
    }

    public void PlaySE(string key)
    {
        if(mCachedSEList.ContainsKey(key))
        {
            mAudioSource.PlayOneShot(mCachedSEList[key]);
        }
    }

    public void PlayBGM(string key)
    {
        if (mCachedBGMList.ContainsKey(key))
        {
            mAudioSource.clip = mCachedBGMList[key];
            mAudioSource.DOFade(1, 0);
            mAudioSource.Play();
        }
    }

    public void StopBGM()
    {
        mAudioSource.DOFade(0, 0.3f)
            .OnComplete(()=>mAudioSource.Stop());
    }
}

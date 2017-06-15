using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDataObject : ScriptableObject
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

    public List<AudioData> SEList = new List<AudioData>();
    public List<AudioData> BGMList = new List<AudioData>();

    private Dictionary<string, AudioClip> mCachedSEList;
    private Dictionary<string, AudioClip> mCachedBGMList;


    public void Init()
    {
        mCachedSEList = new Dictionary<string, AudioClip>();
        mCachedBGMList = new Dictionary<string, AudioClip>();

        foreach (var data in SEList)
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

    public bool TryGetSE(string key,out AudioClip clip)
    {
        if(mCachedSEList.ContainsKey(key))
        {
            clip = mCachedSEList[key];
            return true;
        }
        clip = null;
        return false;
    }
    public bool TryGetBGM(string key, out AudioClip clip)
    {
        if (mCachedBGMList.ContainsKey(key))
        {
            clip = mCachedBGMList[key];
            return true;
        }
        clip = null;
        return false;
    }
}

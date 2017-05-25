using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BitStrap;

[RequireComponent(typeof(AudioSource))]
public class BeatLine : MonoBehaviour {

    [System.Serializable]
    public struct BeatAction
    {
        public float second;
        public bool isPlaySound;

        public BeatAction(float tSecond, bool tIsPlaySound)
        {
            second = tSecond;
            isPlaySound = tIsPlaySound;

        }
        public BeatAction(float tSecond)
        {
            second = tSecond;
            isPlaySound = false;
        }
    }

    public int BPM = 0;

    private float BPS = 0.0f;

    public AudioClip BGMClip = null;
    public AudioClip SEClip = null;

    private AudioSource mCachedAudioSource = null;
    public AudioSource CachedAudioSource
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

    public float BeatRatio = 1.0f;

    public List<BeatAction> BeatSecond = new List<BeatAction>();

    private int mCurrentActionIndex = 0;
    // Use this for initialization
    void Start()
    {
        if(BPM <= 0)
        {
            return;
        }

        if (BeatSecond.Count == 0)
        {
            RefrashLine();
        }
    }

    [Button]
    public void Play()
    {
        if(CachedAudioSource.isPlaying == false)
        {
            CachedAudioSource.Play();
        }
    }

    [Button]
    public void RefrashLine()
    {
        CachedAudioSource.clip = BGMClip;

        BPS = 60.0f / (float)BPM;

        Debug.Log(string.Format("BPS : {0}", BPS));

        BeatSecond.Clear();
        for (int i = 0; i < CachedAudioSource.clip.length / BPS; i++)
        {
            BeatSecond.Add(new BeatAction((BPS * i) * BeatRatio));
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < BeatSecond.Count; i++)
        {
            Vector2 top = new Vector2(BeatSecond[i].second, 0);
            Vector2 bot = new Vector2(BeatSecond[i].second, -0.8f);
            Debug.DrawLine(top, bot, Color.white);
        }

        if (CachedAudioSource.isPlaying)
        {
            float crntX = CachedAudioSource.time * BeatRatio;

            Vector2 crntTop = new Vector2(crntX, 0);
            Vector2 crntBot = new Vector2(crntX, -0.8f);


            if (BeatSecond[mCurrentActionIndex].second - crntX < float.Epsilon)
            {
                if(BeatSecond[mCurrentActionIndex].isPlaySound)
                {
                    CachedAudioSource.PlayOneShot(SEClip);
                }
                mCurrentActionIndex++;
            }
            crntTop.y = 0.8f;
            crntBot.y = 0;
            Debug.DrawLine(crntTop, crntBot, Color.green);
        }


    }

}

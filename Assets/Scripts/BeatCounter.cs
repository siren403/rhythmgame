using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 비트의 흐름을 제어하는 기본 클래스
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BeatCounter : MonoBehaviour
{
    [System.Serializable]
    public struct ActionInfo
    {
        public int index;
        public float time;
        public InputCode input;
        public int soundCode;
        public int actionCode;

        public ActionInfo(int tIndex,float tTime)
        {
            index = tIndex;
            time = tTime;
            actionCode = 0;
            soundCode = -1;
            input = InputCode.None;
        }
    }

    public enum InputResult
    {
        None, 
        Fast, Perfect, Late,
        Fail,
    }
    public enum InputCode
    {
        None,
        SpaceDown, SpaceUp,
    }


    public float BPM = 129;
    private float BPS = 0.0f;

    public float StartBeatOffset = 0.0f;

    private float mCurrentTime = 0;

    private bool mIsPlaying = false;
    public bool mIsMusicPlay = false;

    private AudioSource mAudioSource = null;
    public List<AudioClip> SEList = new List<AudioClip>();

    private float CurrentTrackTime
    {
        get
        {
            float tTime = 0;
            if (mAudioSource.isPlaying)
            {
                tTime = mAudioSource.time;
            }
            else
            {
                tTime = mCurrentTime;
            }

            tTime += BPS * StartBeatOffset;

            if(tTime < 0)
            {
                tTime = 0;
            }

            return tTime;
        }
    }

    private float mBeatProgress
    {
        get
        {
            return CurrentTrackTime / BPS;
        }
    }

    public Transform[] mCubeArray = null;
    public Renderer BeatPanel = null;
    public Renderer CheckTimingPanel = null;

    public List<ActionInfo> PushActionInfo = new List<ActionInfo>();
    public List<ActionInfo> mActionInfoList = new List<ActionInfo>();
   
    //private int mBeatIndex
    //{
    //    get
    //    {
    //        if(mBeatProgress > 1)
    //        {
    //            int index = Mathf.RoundToInt(mBeatProgress) - 1;
    //            return index - ((int)(index / mActionInfoList.Count)) * mActionInfoList.Count;
    //        }
    //        return 0;
    //    }
    //}

    private float mPerfectRatio = 0.1f;
    private int mPrevRoundProgress = 0;
    private int mActionIndex = 0;

    public GameObject PFBall = null;
    public Transform BallStartPoint = null;
    public Transform BallEndPoint = null;
    public Transform BallPerpectPoint = null;
    private GameObject CurrentBall = null;
    // Use this for initialization
    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
        BPS = 60.0f / (float)BPM;
        Debug.Log("BPS : " + BPS);
        for (int i = 0; i < mAudioSource.clip.length / BPS; i++)
        {
            mActionInfoList.Add(new ActionInfo(i, i * BPS));
        }
        Debug.Log("ActionInfo Count : " + mActionInfoList.Count);

        foreach(var info in PushActionInfo)
        {
            var tBaseInfo = mActionInfoList[info.index];
            tBaseInfo.input = info.input;
            tBaseInfo.soundCode = info.soundCode;
            tBaseInfo.actionCode = info.actionCode;
            mActionInfoList[info.index] = tBaseInfo;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(mIsPlaying == false && Input.GetKeyUp(KeyCode.Space))
        {
            mIsPlaying = true;
            if (mIsMusicPlay)
            {
                mAudioSource.Play();
            }
        }   
        if (mIsPlaying)
        {
            if(mIsMusicPlay && mAudioSource.isPlaying == false)
            {
                Stop();
            }

            if (mActionInfoList[mActionIndex].time - CurrentTrackTime <= 0.001f)
            {
                BeatPanel.material.DOColor(Color.white, BPS * 0.5f).From();

                if (mActionInfoList[mActionIndex].soundCode != -1)
                {
                    mAudioSource.PlayOneShot(SEList[mActionInfoList[mActionIndex].soundCode]);
                }
                if(mActionInfoList[mActionIndex].actionCode != 0)
                {
                    if (CurrentBall == null)
                    {
                        CurrentBall = Instantiate(PFBall, BallStartPoint.position, Quaternion.identity);
                    }

                    CurrentBall.transform.position = BallStartPoint.position;
                    CurrentBall.SetActive(true);
                    CurrentBall.transform.DOJump(BallEndPoint.position, 2, 1, BPS)
                        .SetEase(Ease.Linear);
                }

                mActionIndex++;
            }

            InputCode tInputCode = InputCode.None;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                tInputCode = InputCode.SpaceDown;
                Debug.Log(mActionIndex);

            }
            else if(Input.GetKeyUp(KeyCode.Space))
            {
                tInputCode = InputCode.SpaceUp;
            }

            InputResult tResult = CheckInputTiming(tInputCode);
            switch (tResult)
            {
                case InputResult.Fast:
                    CheckTimingPanel.material.DOColor(Color.red, BPS * 0.8f).From();
                    break;
                case InputResult.Perfect:
                    CheckTimingPanel.material.DOColor(Color.green, BPS * 0.8f).From();
                    CurrentBall.transform.DOJump(BallPerpectPoint.position, 2, 1, BPS)
                       .OnComplete(() => CurrentBall.SetActive(false));
                    //CurrentBall.SetActive(false);
                    break;
                case InputResult.Late:
                    CheckTimingPanel.material.DOColor(Color.blue, BPS * 0.8f).From();
                    break;
                case InputResult.Fail:
                    //CheckTimingPanel.material.DOColor(Color.black, BPS * 0.5f).From();
                    break;
            }


           

            if (mIsMusicPlay == false)
            {
                mCurrentTime += Time.deltaTime;
            }
        }
    }
    


    /// <summary>
    /// 입력에 따른 판정을 실시한다
    /// </summary>
    /// <param name="tInputCode">입력 타입</param>
    /// <param name="tRoundProgress">현재 비트</param>
    /// <param name="tBeatProgress">입력 시 비트 타이밍</param>
    /// <returns>판정 결과</returns>
    private InputResult CheckInputTiming(InputCode tInputCode)
    {

        int tRoundProgress = Mathf.RoundToInt(mBeatProgress);

        InputResult tResult = InputResult.Fail;

        if (mActionInfoList[mActionIndex].input != InputCode.None)
        {

            //((tInputCode == InputCode.SpaceDown && mBeatData[mBeatIndex] == 1) ||
            //(tInputCode == InputCode.SpaceUp && mBeatData[mBeatIndex] == 2))
            if (mActionInfoList[mActionIndex].input == tInputCode)
            {
                if (tRoundProgress - mPerfectRatio > mBeatProgress)
                {
                    Debug.Log("Too fast");
                    tResult = InputResult.Fast;
                }
                else if (tRoundProgress + mPerfectRatio < mBeatProgress)
                {
                    Debug.Log("Too Late");
                    tResult = InputResult.Late;
                }
                else
                {
                    Debug.Log("Perfect!!");
                    tResult = InputResult.Perfect;
                   
                }
                Debug.Log(tResult.ToString() + " / " + string.Format("{0} [{1}] {2}",
                    tRoundProgress - mPerfectRatio,
                    mBeatProgress,
                    tRoundProgress + mPerfectRatio));
            }
            
        }

        return tResult;
    }

    private void ChangeCubeColor(Color tColor)
    {
        int tCubeIndex = (int)Mathf.Repeat(mActionIndex, 8);
        var mat = mCubeArray[tCubeIndex].GetComponent<Renderer>().material;
        mat.color = tColor;
        mat.DOColor(Color.white, 0.0f).SetDelay(BPS);
    }

    /// <summary>
    /// 정지
    /// </summary>
    public void Stop()
    {
        mIsPlaying = false;
        mCurrentTime = 0;
    }

    /**
    * @brief
    * 두 파라메터를 입력받아 더한 값을 리턴1
    * @warning 주의 사항
    * @param aa 더할 값 11
    * @param bb 더할 값 21
    * @return a + b를 리턴한다.1
    * @bug 존재하는 버그에 대한 설명
    * @todo. 해야할 목록에 대하여.@
    **/
    public void TestMethod()
    {

    }

    /// <summary>
    /// 테스트 2
    /// </summary>
    /// <Warning>주의사항</Warning>
    /// <param name="none">파라매터</param>
    /// <returns>리턴</returns>
    /// @code{.cs}
    /// public void Sample()
    /// {
    ///     Debug.Log("sads");
    /// }
    /// 
    /// int tA = 0;
    /// @endcode
    /// <Bug>버그</Bug>
    /// <Todo>해야할 일</Todo>
    public void TestMethod2()
    {

    }
}

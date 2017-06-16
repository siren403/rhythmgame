using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using BitStrap;
using ScenePresenter;
using UniRx;

public class CUIPlayGame : UIBase
{
    public enum EvaluationType
    {
        None,
        Fail, Normal, Good,
    }

    [SerializeField]
    private Text mInstTxtEvalTitle = null;
    [SerializeField]
    private Text mInstTxtEvalComment = null;
    [SerializeField]
    private GameObject mInstEvalFailResult = null;
    [SerializeField]
    private GameObject mInstEvalNormalResult = null;
    [SerializeField]
    private GameObject mInstEvalGoodResult = null;

    [SerializeField]
    private AudioClip SEBeep = null;
    [SerializeField]
    private AudioClip SEBeeBeep = null;

    [SerializeField]
    private AudioClip BGMFail = null;
    [SerializeField]
    private AudioClip BGMNormal = null;
    [SerializeField]
    private AudioClip BGMGood = null;

    private EvaluationType mEvaluationType = EvaluationType.None;

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

    public void ShowEvaluation(string tTitle, string tComment, EvaluationType tType)
    {
        mInstTxtEvalTitle.text = tTitle;
        mInstTxtEvalComment.text = tComment;
        mEvaluationType = tType;
        StartCoroutine(SeqShowEvaluation());
    }
    private IEnumerator SeqShowEvaluation()
    {
        yield return new WaitForSeconds(2.0f);

        mInstTxtEvalTitle.transform.parent.gameObject.SetActive(true);
        mAudioSource.PlayOneShot(SEBeep);
        yield return new WaitForSeconds(1.1f);
        mInstTxtEvalComment.gameObject.SetActive(true);
        mAudioSource.PlayOneShot(SEBeeBeep);
        yield return new WaitForSeconds(1.4f);

        GameObject tEvalObject = null;
        AudioClip tBGM = null;

        switch (mEvaluationType)
        {
            case EvaluationType.Fail:
                tEvalObject = mInstEvalFailResult;
                tBGM = BGMFail;
                break;
            case EvaluationType.Normal:
                tEvalObject = mInstEvalNormalResult;
                tBGM = BGMNormal;
                break;
            case EvaluationType.Good:
                tEvalObject = mInstEvalGoodResult;
                tBGM = BGMGood;
                break;
        }

        tEvalObject.SetActive(true);
        mAudioSource.PlayOneShot(SEBeep);
        mAudioSource.clip = tBGM;
        mAudioSource.Play();

        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => InputManager.GetKey(InputCode.SingleDown));

        mAudioSource.Stop();
        mAudioSource.PlayOneShot(SEBeeBeep);
        mInstTxtEvalTitle.transform.parent.gameObject.SetActive(false);
        mInstTxtEvalComment.gameObject.SetActive(false);
        tEvalObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        NavigationService.NavigateAsync("SceneGameSelect").Subscribe();
    }

    [Button]
    public void TestSeqFail()
    {
        mEvaluationType = EvaluationType.Fail;
        StartCoroutine(SeqShowEvaluation());
    }
    [Button]
    public void TestSeqNormal()
    {
        mEvaluationType = EvaluationType.Normal;
        StartCoroutine(SeqShowEvaluation());
    }
    [Button]
    public void TestSeqGood()
    {
        mEvaluationType = EvaluationType.Good;
        StartCoroutine(SeqShowEvaluation());
    }
}

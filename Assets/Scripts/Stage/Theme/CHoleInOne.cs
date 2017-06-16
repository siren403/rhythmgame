using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Toolkit;
using System;
using DG.Tweening;


public class CHoleInOne : CStageBase
{
    private const int BEAT_LEVEL_1 = 39;
    private const int BEAT_LEVEL_2 = 76;
    private const string KEY_TRIGGER_BEAT = "TrigBeat";
    private const string KEY_TRIGGER_SHOT = "TrigShot";
    private const string KEY_TRIGGER_SHOTFAIL = "TrigShotFail";
    private const string KEY_TRIGGER_MANDRILLBALL = "TrigBall";
    
    public enum ActionType
    {
        None,
        SEMonkey, SEMandril, SEMonkeyShort,
        ThrowBall,
    }

    public AudioClip SEPrologue = null;
    public Animator AnimPrologue = null;

    public AudioClip SEMonkey = null;
    public AudioClip SEMandril = null;
    public AudioClip SEMonkeyShort = null;

    public AudioClip SEMonkey_0 = null;
    public AudioClip SEMonkey_1 = null;
    public AudioClip SEImpact = null;
    public AudioClip SEGoal = null;


    public Animator AnimMonkey = null;
    public Animator AnimMandrill = null;
    public Animator AnimGolfer = null;
    public Animator AnimBubble = null;

    private Queue<GameObject> mBallPool = new Queue<GameObject>();
    private Queue<GameObject> mActiveBallPool = new Queue<GameObject>();

    public GameObject PFBall = null;
    public Transform BallStartPoint = null;
    public Transform BallEndPoint = null;
    public Transform BallPerpectPoint = null;
    public Transform BallFailPoint = null;
    public Transform BallLatePoint = null;
    public Transform BallFastPoint = null;


    private Dictionary<string, Action<CSequencePlayer, CSequenceData>> mActionList 
        = new Dictionary<string, Action<CSequencePlayer, CSequenceData>>();

    private CScenePlayGame mScene = null;


     void Awake()
    {
        mActionList[CHoleInOneActionCode.SEMONKEY] = PlaySEMonkey;
        mActionList[CHoleInOneActionCode.THROWBALL] = ThrowBall;

        mActionList[CHoleInOneActionCode.MONKEYSHORT] = MonkeyShort;
        mActionList[CHoleInOneActionCode.SEMONKEYSHORT] = PlaySEMonkeyShort;

        mActionList[CHoleInOneActionCode.SEMANDRIL] = (tSeqPlayer, tSeqData) => mAudioSource.PlayOneShot(SEMandril);
        mActionList[CHoleInOneActionCode.MANDRILLBALL] = (tSeqPlayer, tSeqData) => AnimMandrill.SetTrigger(KEY_TRIGGER_MANDRILLBALL);

        mActionList[CHoleInOneActionCode.THROWSTRAIGHTBALL] = ThrowStraightBall;


        mActionList[CHoleInOneActionCode.GOLFERREADY] = (tSeqPlayer, tSeqData) =>
        {
            //AnimGolfer.SetTrigger(KEY_TRIGGER_SHOTREADY);
            AnimGolfer.CrossFade("ShotReady", 0);
        };
    }

    public override void SetScene(CScenePlayGame tScene)
    {
        mScene = tScene;
    }

    public override IEnumerator StartPrologue()
    {
        mAudioSource.PlayOneShot(SEPrologue);
        AnimPrologue.SetTrigger("TrigBegin");
        yield return new WaitForSeconds(SEPrologue.length - 2.2f);

        mScene.InstUIPlayGame.DoFade(1, 0);
        yield return new WaitForEndOfFrame();

        AnimPrologue.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.0f);

        yield return mScene.InstUIPlayGame.DoFade(0).WaitForCompletion();

    }

    public override void OnEveryBeat(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        switch ((int)tSeqPlayer.BeatProgress)
        {
            case BEAT_LEVEL_1:
                AnimMandrill.SetInteger("IntBeatLevel", 1);
                break;
            case BEAT_LEVEL_2:
                AnimMandrill.SetInteger("IntBeatLevel", 2);
                break;
        }

        foreach(var code in tData.ActionCode)
        {
            if (mActionList.ContainsKey(code))
            {
                mActionList[code].Invoke(tSeqPlayer, tData);
            }
        }
    }
    public override void OnBaseBeat(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        AnimMonkey.SetTrigger(KEY_TRIGGER_BEAT);
        AnimMandrill.SetTrigger(KEY_TRIGGER_BEAT);
        AnimGolfer.SetTrigger(KEY_TRIGGER_BEAT);
    }
    public override void OnInputResult(CSequencePlayer tSeqPlayer, InputResult tResult)
    {
        switch (tResult)
        {
            case InputResult.Fast:
                mAudioSource.PlayOneShot(SEImpact);
                AnimGolfer.CrossFade("Shot", 0);
                if (mActiveBallPool.Count > 0)
                {
                    GameObject tBall = mActiveBallPool.Dequeue();
                    tBall.transform.DOScale(0.3f, tSeqPlayer.BPS).SetEase(Ease.OutQuad);
                    tBall.transform.DOJump(BallFastPoint.position, 2, 1, tSeqPlayer.BPS)
                       .SetEase(Ease.OutQuad)
                       .OnComplete(() =>
                       {
                           ReturnPoolBall(tBall);
                       });
                }
                break;
            case InputResult.Perfect:
                mAudioSource.PlayOneShot(SEImpact);
                AnimGolfer.CrossFade("Shot", 0);

                AnimBubble.gameObject.SetActive(true);

                if (mActiveBallPool.Count > 0)
                {
                    GameObject tBall = mActiveBallPool.Dequeue();
                    tBall.transform.position = BallEndPoint.position;
                    tBall.transform.DOScale(0, tSeqPlayer.BPS * 2.0f).SetEase(Ease.OutQuad);
                    tBall.transform.DOJump(BallPerpectPoint.position, 4, 1, tSeqPlayer.BPS * 2.0f)
                       .SetEase(Ease.OutQuad)
                       .OnComplete(() =>
                       {
                           AnimBubble.CrossFade("Perfect", 0);
                           mAudioSource.PlayOneShot(SEGoal);
                           ReturnPoolBall(tBall);
                       });
                }
                break;
            case InputResult.Late:
                mAudioSource.PlayOneShot(SEImpact);
                AnimGolfer.CrossFade("Shot", 0);

                if (mActiveBallPool.Count > 0)
                {
                    GameObject tBall = mActiveBallPool.Dequeue();
                    tBall.transform.DOScale(0.3f, tSeqPlayer.BPS).SetEase(Ease.OutQuad);
                    tBall.transform.DOJump(BallLatePoint.position, 2, 1, tSeqPlayer.BPS)
                       .SetEase(Ease.OutQuad)
                       .OnComplete(() =>
                       {
                           ReturnPoolBall(tBall);
                       });
                }
                break;
            case InputResult.Fail:
                AnimGolfer.SetTrigger(KEY_TRIGGER_SHOTFAIL);
                if (mActiveBallPool.Count > 0)
                {
                    GameObject tBall = mActiveBallPool.Dequeue();
                    var tSeq = DOTween.Sequence();
                    tSeq.Append(tBall.transform.DOJump(BallFailPoint.position, 0.6f, 1, tSeqPlayer.BPS)
                        .SetEase(Ease.OutQuad));
                    tSeq.Append(tBall.transform.DOMove(new Vector2(0.5f,-1.0f), tSeqPlayer.BPS)
                        .SetEase(Ease.OutQuad)
                        .SetRelative());
                    tSeq.AppendCallback(()=> 
                    {
                        ReturnPoolBall(tBall);
                    });
                    tSeq.Play();
                }
                break;
        }
    }



    private void PlaySEMonkey(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        mAudioSource.PlayOneShot(SEMonkey_0);
        AnimMonkey.CrossFade("Short_0", 0);
    }
    private void ThrowBall(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        mAudioSource.PlayOneShot(SEMonkey_1);
        AnimMonkey.CrossFade("Short_1", 0);
        AnimGolfer.CrossFade("ShotReady",0);
        GameObject tBall = RentBall();
        tBall.transform.position = BallStartPoint.position;
        tBall.transform.DOJump(BallEndPoint.position, 2, 1, tSeqPlayer.BPS )
            .SetEase(Ease.Linear);
    }

    private void ThrowStraightBall(CSequencePlayer tSeqPlayer,CSequenceData tData)
    {
        GameObject tBall = RentBall();
        tBall.transform.position = BallStartPoint.position;
        tBall.transform.DOMove(BallEndPoint.position, tSeqPlayer.BPS * 0.1f)
           .SetEase(Ease.Linear)
           .OnComplete(()=> ReturnPoolBall(mActiveBallPool.Dequeue()));
    }
    private void PlaySEMonkeyShort(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        mAudioSource.PlayOneShot(SEMonkeyShort);
    }
    private void MonkeyShort(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        AnimMonkey.CrossFade("Short_0", 0);
    }

    private GameObject RentBall()
    {
        GameObject tBall = null;

        if (mBallPool.Count > 0)
        {
            tBall = mBallPool.Dequeue();
        }
        else
        {
            tBall = Instantiate(PFBall, BallStartPoint.position, Quaternion.identity);
            tBall.transform.SetParent(this.transform);
        }

        tBall.gameObject.SetActive(true);
        mActiveBallPool.Enqueue(tBall);
        return tBall;
    }
    private void ReturnPoolBall(GameObject tGameObject)
    {
        tGameObject.SetActive(false);
        tGameObject.transform.localScale = Vector3.one;
        mBallPool.Enqueue(tGameObject);
    }

   
}

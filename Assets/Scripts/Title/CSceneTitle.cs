using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScenePresenter;
using DG.Tweening;
using UniRx;

public class CSceneTitle : SceneBase
{
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


    public CUITitle UITitle = null;

    public Animator AnimTitle = null;


    protected override void BeforeInitialize()
    {
        AudioManager.Inst.Init();
        UITitle.DoFade(1, 0);
    }

    protected override void Initialize()
    {
        StartCoroutine(SeqTitle());
    }

    private IEnumerator SeqTitle()
    {
        yield return new WaitForSeconds(1.0f);
        yield return UITitle.DoFade(0).WaitForCompletion();

        AnimTitle.SetTrigger("TrigBegin");

        AudioManager.Inst.PlayBGM("title");
        yield return new WaitForSeconds(4.0f);

        yield return new WaitUntil(() => InputManager.GetKey(InputCode.SingleDown));

        AudioManager.Inst.StopBGM();
        AudioManager.Inst.PlaySE("beebeep");
        UITitle.DoFade(1);
        mAudioSource.DOFade(0, 0.3f);
        yield return UITitle.DoFade(1).WaitForCompletion();

        NavigationService.NavigateAsync("SceneGameSelect").Subscribe();
    }

}

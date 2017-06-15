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

    public Renderer RenderBG = null;


    protected override void BeforeInitialize()
    {
        UITitle.DoFade(1, 0);
    }

    protected override void Initialize()
    {
        
        StartCoroutine(SeqTitle());
    }

    private IEnumerator SeqTitle()
    {
        string tPropName = "_MainTex";
        DOTween.To(() => RenderBG.material.GetTextureOffset(tPropName),
            (v) => RenderBG.material.SetTextureOffset(tPropName, v),
            new Vector2(-10, -10),
            10)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

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

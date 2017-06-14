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

    [SerializeField]
    private AudioClip BGMTitle = null;

    public CUITitle UITitle = null;

    public SpriteRenderer SpriteStar = null;

    public List<GameObject> InstTitleTexts = new List<GameObject>();
    public GameObject InstCharacter = null;
    public GameObject InstStartText = null;

    protected override void BeforeInitialize()
    {
        UITitle.DoFade(1, 0);
        SpriteStar.enabled = false;
    }

    protected override void Initialize()
    {
        StartCoroutine(SeqTitle());
    }

    private IEnumerator SeqTitle()
    {
        yield return new WaitForSeconds(1.0f);
        yield return UITitle.DoFade(0).WaitForCompletion();

        mAudioSource.clip = BGMTitle;
        mAudioSource.Play();
        yield return new WaitForSeconds(4.0f);

        yield return new WaitUntil(() => InputManager.GetKey(InputCode.SingleDown));

        UITitle.DoFade(1);
        mAudioSource.DOFade(0, 0.3f);
        yield return UITitle.DoFade(1).WaitForCompletion();

        NavigationService.NavigateAsync("SceneGameSelect").Subscribe();
    }
}

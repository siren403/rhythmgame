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

        yield return new WaitForSeconds(0.7f);
        InstTitleTexts[0].SetActive(true);
        InstTitleTexts[1].SetActive(true);

        yield return new WaitForSeconds(0.7f);
        InstTitleTexts[2].SetActive(true);
        InstTitleTexts[3].SetActive(true);

        yield return new WaitForSeconds(1.0f);
        InstTitleTexts[4].SetActive(true);
        InstTitleTexts[5].SetActive(true);

        yield return new WaitForSeconds(0.3f);
        InstTitleTexts[6].SetActive(true);
        InstTitleTexts[7].SetActive(true);

        yield return new WaitForSeconds(0.4f);
        InstCharacter.transform.DOLocalMoveY(3, 0.3f).SetEase(Ease.OutExpo).SetRelative();
        InstStartText.transform.DOLocalMoveY(250, 0.3f).SetEase(Ease.OutExpo).SetRelative();

        yield return new WaitForSeconds(0.5f);
        SpriteStar.enabled = true;

        yield return new WaitUntil(() => InputManager.GetKey(InputCode.SingleDown));

        UITitle.DoFade(1);
        mAudioSource.DOFade(0, 0.3f);
        yield return UITitle.DoFade(1).WaitForCompletion();

        NavigationService.NavigateAsync("SceneGameSelect").Subscribe();
    }
}

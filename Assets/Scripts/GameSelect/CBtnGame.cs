using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CBtnGame : MonoBehaviour
{
    public Image InstImgBG = null;
    public Text InstTxtTitle = null;
    public Image InstImgIcon = null;

    private bool mIsOpened = false;

    private Sequence mCurrentSequence = null;

    private CUIGameSelect mUIParent = null;

    public string SceneName = string.Empty;

    public bool IsClosed = false;

    public void SetUIParent(CUIGameSelect tUi)
    {
        mUIParent = tUi;
    }

    public void OnPress()
    {
        if(IsClosed || (mCurrentSequence != null && mCurrentSequence.IsPlaying()))
        {
            return;
        }

        InstImgIcon.transform.DOScale(0.9f, 0.3f)
                .From();
        if (mIsOpened == false)
        {
            mCurrentSequence = DOTween.Sequence();
            mCurrentSequence.Append(InstImgBG.transform.DOScaleX(1, 0.27f).SetEase(Ease.OutExpo));
            mCurrentSequence.Append(InstTxtTitle.DOFade(1, 0.15f));
            mCurrentSequence.AppendCallback(() => InstImgBG.raycastTarget = true);
            mCurrentSequence.Play();
        }
        else
        {
            mCurrentSequence = DOTween.Sequence();
            mCurrentSequence.Append(InstTxtTitle.DOFade(0, 0.15f));
            mCurrentSequence.Append(InstImgBG.transform.DOScaleX(0, 0.27f).SetEase(Ease.OutExpo));
            mCurrentSequence.AppendCallback(() => InstImgBG.raycastTarget = false);
            mCurrentSequence.Play();
        }
        mIsOpened = !mIsOpened;
    }

    public void OnStartGame()
    {
        if (mIsOpened)
        {
            AudioManager.Inst.StopBGM();
            AudioManager.Inst.PlaySE("beebeep");
            mUIParent.DoStartGame(SceneName);
        }
    }
}

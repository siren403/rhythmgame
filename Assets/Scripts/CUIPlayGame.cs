using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CUIPlayGame : MonoBehaviour
{
    public enum EvaluationType
    {
        None,
        Fail, Normal, Good,
    }

    [SerializeField]
    private Image mInstImgFade = null;

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

    private EvaluationType mEvaluationType = EvaluationType.None;

    public Tween DoFade(float value,float duration = 0.3f)
    {
        return mInstImgFade.DOFade(value, duration);
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
        yield return new WaitForSeconds(1.0f);
        mInstTxtEvalComment.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.4f);
        switch (mEvaluationType)
        {
            case EvaluationType.Fail:
                mInstEvalFailResult.SetActive(true);
                break;
            case EvaluationType.Normal:
                mInstEvalNormalResult.SetActive(true);
                break;
            case EvaluationType.Good:
                mInstEvalGoodResult.SetActive(true);
                break;
        }

    }
}

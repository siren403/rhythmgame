using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScenePresenter;
using UniRx;
using BitStrap;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CScenePlayGame : SceneBase
{
    protected override IPresenter[] Children
    {
        get
        {
            return new IPresenter[] { InstSequencePlayer };
        }
    }

    public CSequencePlayer InstSequencePlayer = null;
    private CStageBase mStage = null;

    public float SeekBeat = 0;

    public CUIPlayGame InstUIPlayGame = null;

    public override IObservable<Unit> PrepareAsync()
    {
        InstUIPlayGame.DoFade(1, 0);

        var tAsync = SceneManager.LoadSceneAsync(Argument.ToString(), LoadSceneMode.Additive);
        return Observable.FromCoroutine<Unit>(_ => 
        NavigationService.HyperOptimizedFastAsyncOperationLoad(tAsync, _));
    }

    protected override void BeforeInitialize()
    {

        mStage = FindObjectOfType<CStageBase>();

        InstSequencePlayer.SetReceiver(mStage);
        InstSequencePlayer.SetStageData(mStage.StageData);
        InstSequencePlayer.OnComplete = () =>
        {
            InstUIPlayGame.DoFade(1)
                .SetDelay(0.5f)
                .OnComplete(() =>
                {
                    var tEval = InstSequencePlayer.Evaluation.EvaluateValue;
                    string tEvalComment = string.Empty;
                    CUIPlayGame.EvaluationType tEvalType = CUIPlayGame.EvaluationType.None;

                    if (tEval >= mStage.StageData.EvaluationFailRatio + mStage.StageData.EvaluationNormalRatio)
                    {
                        tEvalComment = mStage.StageData.EvaluationGoodText;
                        tEvalType = CUIPlayGame.EvaluationType.Good;
                    }
                    else if (tEval >= mStage.StageData.EvaluationFailRatio)
                    {
                        tEvalComment = mStage.StageData.EvaluationNormalText;
                        tEvalType = CUIPlayGame.EvaluationType.Normal;
                    }
                    else
                    {
                        tEvalComment = mStage.StageData.EvaluationFailText;
                        tEvalType = CUIPlayGame.EvaluationType.Fail;
                    }

                    InstUIPlayGame.ShowEvaluation(mStage.StageData.EvaluationTitle, tEvalComment, tEvalType);
                });
        };
    }
    protected override void Initialize()
    {
        InstUIPlayGame.DoFade(0)
            .SetDelay(0.5f)
            .OnComplete(() => 
            {
                InstSequencePlayer.Play();
            });
    }

    [Button]
    public void OnSeek()
    {
        InstSequencePlayer.Seek(SeekBeat);
    }
}

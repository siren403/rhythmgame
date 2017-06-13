using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScenePresenter;
using UniRx;
using BitStrap;
using DG.Tweening;

public class CScenePlayGame : SceneBase
{
    protected override IPresenter[] Children
    {
        get
        {
            return new IPresenter[] { InstSequencePlayer, InstHoleInOne };
        }
    }

    public CSequencePlayer InstSequencePlayer = null;
    public CHoleInOne InstHoleInOne = null;
    public CStageData CurrentStageData = null;
    public float SeekBeat = 0;

    public CUIPlayGame InstUIPlayGame = null;

    protected override void BeforeInitialize()
    {
        InstSequencePlayer.SetStageData(CurrentStageData);
        InstSequencePlayer.SetReceiver(InstHoleInOne);
        InstSequencePlayer.OnComplete = () =>
        {
            InstUIPlayGame.DoFade(1)
                .SetDelay(2.0f)
                .OnComplete(() =>
                {
                    var tEval = InstSequencePlayer.Evaluation.EvaluateValue;
                    string tEvalComment = string.Empty;
                    CUIPlayGame.EvaluationType tEvalType = CUIPlayGame.EvaluationType.None;

                    if (tEval >= CurrentStageData.EvaluationFailRatio + CurrentStageData.EvaluationNormalRatio)
                    {
                        tEvalComment = CurrentStageData.EvaluationGoodText;
                        tEvalType = CUIPlayGame.EvaluationType.Good;
                    }
                    else if (tEval >= CurrentStageData.EvaluationFailRatio)
                    {
                        tEvalComment = CurrentStageData.EvaluationNormalText;
                        tEvalType = CUIPlayGame.EvaluationType.Normal;
                    }
                    else
                    {
                        tEvalComment = CurrentStageData.EvaluationFailText;
                        tEvalType = CUIPlayGame.EvaluationType.Fail;
                    }

                    InstUIPlayGame.ShowEvaluation(CurrentStageData.EvaluationTitle, tEvalComment, tEvalType);
                });
        };
        InstUIPlayGame.DoFade(1, 0);
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

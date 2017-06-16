using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISequenceReceiver
{
    void OnEveryBeat(CSequencePlayer tSeqPlayer ,CSequenceData tData);
    void OnBaseBeat(CSequencePlayer tSeqPlayer, CSequenceData tData);
    void OnInputResult(CSequencePlayer tSeqPlayer, InputResult tResult);
    IEnumerator StartPrologue();
    void SetScene(CScenePlayGame tScene);

}


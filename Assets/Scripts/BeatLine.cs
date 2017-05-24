using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatLine : MonoBehaviour {

    public int BPM = 0;

    private float BPS = 0.0f;

    private List<float> BeatPos = new List<float>();

    // Use this for initialization
    void Start()
    {
        if(BPM <= 0)
        {
            return;
        }

        BPS = (float)BPM / 60.0f;

        Debug.Log(string.Format("BPS : {0}", BPS));


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

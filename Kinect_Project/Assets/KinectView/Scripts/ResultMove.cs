using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;
using UnityEngine.SceneManagement;

public class ResultMove : MonoBehaviour {

    private float timeElapsed;//メインシーンへ移行する用のタイマー

    // Use this for initialization
    void Start () {
        timeElapsed = 0;
	}
	
	// Update is called once per frame
	void Update () {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > 10)         //約10秒経過したらメインシーンへ
        {
            timeElapsed = 0;
            SceneManager.LoadScene("MainScene");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameControl : MonoBehaviour
{
    public static GameControl control;

    public int totalCarCount;
    public int[] totalScore;
    public float initialTimer;
    public int maxScore;
    public int[,] Score;       // 주차구역과 차종을 이용하여 점수 매핑

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (control == null)
        {
            control = this;
            control.totalCarCount = 20;
            control.initialTimer = 4f * 60;     // 4분
            control.totalScore = new int[totalCarCount + 2];
            maxScore = 700;
            // 점수 관련 초기 세팅
            Score = new int[(int)CarType.Max, (int)SiteName.Max]
            {   // 어린이 / 소화전 / 버스 / 교차로 / 횡단보도 / 인도 / 합법구역
                { 12, 4, 8, 4, 4, 4, 10},  // 합법구역은 10점
                { 13, 5, 9, 5, 5, 5, 10},
            };
        } else if (control != this)
        {
            Destroy(gameObject);
        }
    }
}

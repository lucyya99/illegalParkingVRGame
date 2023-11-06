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
    public int[,] Score;       // ���������� ������ �̿��Ͽ� ���� ����

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (control == null)
        {
            control = this;
            control.totalCarCount = 20;
            control.initialTimer = 4f * 60;     // 4��
            control.totalScore = new int[totalCarCount + 2];
            maxScore = 700;
            // ���� ���� �ʱ� ����
            Score = new int[(int)CarType.Max, (int)SiteName.Max]
            {   // ��� / ��ȭ�� / ���� / ������ / Ⱦ�ܺ��� / �ε� / �չ�����
                { 12, 4, 8, 4, 4, 4, 10},  // �չ������� 10��
                { 13, 5, 9, 5, 5, 5, 10},
            };
        } else if (control != this)
        {
            Destroy(gameObject);
        }
    }
}

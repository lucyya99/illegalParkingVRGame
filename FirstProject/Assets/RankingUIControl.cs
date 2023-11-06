using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RankingUIControl : MonoBehaviour
{
    [SerializeField]
    private CalculateTotalScore CalculateTotalScore;

    //랭킹 정렬
    private int[] bestScore = new int[3] { 0, 0, 0 };
    private GameObject[] bestScores;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void ScoreSet()
    {
        //현재에 저장
        PlayerPrefs.SetInt("CurrentPlayerScore", CalculateTotalScore.sum);

        int tmpScore = 0;

        for (int i = 0; i < 3; i++)
        {
            //저장된 최고점수 가져오기
            bestScore[i] = PlayerPrefs.GetInt(i + "BestScore");

            //현재 점수가 랭킹에 오를 수 있을 때
            while (bestScore[i] < CalculateTotalScore.sum)
            {
                //자리 바꿈
                tmpScore = bestScore[i];
                bestScore[i] = CalculateTotalScore.sum;

                //랭킹에 저장
                PlayerPrefs.SetInt(i + "BestScore", CalculateTotalScore.sum);
            }
        }

        //랭킹에 맞춰 점수 저장
        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetInt(i + "BestScore", bestScore[i]);
        }

        bestScores = new GameObject[3]
        {
            GameObject.Find("GoldObj").gameObject,
            GameObject.Find("SilverObj").gameObject,
            GameObject.Find("BronzeObj").gameObject
        };

        for (int i = 0; i < 3; i++)
        {
            if (bestScore[i] == 0)
            {
                bestScores[i].SetActive(false);
                continue;
            }
            bestScores[i].transform.GetChild(1).GetComponent<TMP_Text>().text = $"{bestScore[i]}";
        }
    }
}
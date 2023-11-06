using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RankingUIControl : MonoBehaviour
{
    [SerializeField]
    private CalculateTotalScore CalculateTotalScore;

    //��ŷ ����
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
        //���翡 ����
        PlayerPrefs.SetInt("CurrentPlayerScore", CalculateTotalScore.sum);

        int tmpScore = 0;

        for (int i = 0; i < 3; i++)
        {
            //����� �ְ����� ��������
            bestScore[i] = PlayerPrefs.GetInt(i + "BestScore");

            //���� ������ ��ŷ�� ���� �� ���� ��
            while (bestScore[i] < CalculateTotalScore.sum)
            {
                //�ڸ� �ٲ�
                tmpScore = bestScore[i];
                bestScore[i] = CalculateTotalScore.sum;

                //��ŷ�� ����
                PlayerPrefs.SetInt(i + "BestScore", CalculateTotalScore.sum);
            }
        }

        //��ŷ�� ���� ���� ����
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
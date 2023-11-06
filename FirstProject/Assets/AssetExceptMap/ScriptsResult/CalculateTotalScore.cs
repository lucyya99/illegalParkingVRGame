using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.WSA;


public class CalculateTotalScore : MonoBehaviour
{
    public int sum = 0;
    public int oScore = 0, oCount = 0;
    public int xScore = 0, xCount = 0;
    int timeScore;
    int itemScore;
    // ��, ��, ��
    // ���ѽð��� 5���̶�� �ϸ�, ���ѽð� ������ 5 * 60 * 10 = 3000�� �ִ�
    // ���� �����ص� 4�и��� ã�� �� ���� ���̶� ���� 60 * 10 = 600
    // 600 + (100) = 700

    public GameObject oScoreObj;
    public GameObject xScoreObj;
    public GameObject timeScoreObj;
    public GameObject itemScoreObj;
    public GameObject totalScoreObj;

    // Start is called before the first frame update
    void Start()
    {
        oScoreObj = GameObject.Find("OScore").gameObject;
        xScoreObj = GameObject.Find("XScore").gameObject;
        timeScoreObj = GameObject.Find("TimeScore").gameObject;
        itemScoreObj = GameObject.Find("ItemScore").gameObject;
        totalScoreObj = GameObject.Find("TotalScoreObj").gameObject;

        // ���� & ���� ����
        Calculator();

        // count UI ����
        setCountTxt(oScoreObj, oCount, GameControl.control.totalCarCount);
        setCountTxt(xScoreObj, xCount, GameControl.control.totalCarCount);
        setCountTxt(timeScoreObj, (int)(timeScore / 10), (int) GameControl.control.initialTimer);
        int itemCount = (int)(itemScore / 5);
        Debug.Log($"{itemCount}");
        itemScoreObj.transform.GetChild(0).GetComponent<TMP_Text>().text = itemCount.ToString();

        // score UI ����
        setScoreTxt(oScoreObj, oScore);
        setScoreTxt(xScoreObj, xScore);
        setScoreTxt(timeScoreObj, timeScore);
        setScoreTxt(itemScoreObj, itemScore);
        setScoreTxt(totalScoreObj, sum);

        setMedal();
    }

    // ����
    public void setCountTxt(GameObject _parent, int _count, int _totalCount)
    {
        GameObject _countObj = _parent.transform.GetChild(0).gameObject;
        TMP_Text _countTxt = _countObj.GetComponent<TMP_Text>();

        _countTxt.text = _count.ToString() + "/" + _totalCount;
    }

    public void setScoreTxt(GameObject _parent, int _score)
    {
        // ���� ������Ʈ�� ������
        GameObject _scoreObj = _parent.transform.GetChild(1).gameObject;
        TMP_Text _scoreTxt = _scoreObj.GetComponent<TMP_Text>();
        _scoreTxt.text = _score.ToString();
    }

    enum Medal
    {
        Gold,
        Silver,
        Bronze,
        Max
    }

    Medal myMedal;

    void setMedal()
    {
        // maxScore = 600 / 3 = 200
        // 0~200, 200~400, 400~600
        float standard = GameControl.control.maxScore / 3.0f;
        
        if (sum >= standard * 2)      myMedal = Medal.Gold;
        else if (sum >= standard * 1) myMedal = Medal.Silver;
        else                                myMedal = Medal.Bronze;


        TMP_Text resultTxt = GameObject.Find("Room/ResultCanvas/ResultTxt").gameObject.GetComponent<TMP_Text>();
        switch (myMedal)
        {
            case Medal.Gold:
                GameObject.Find("ResultCanvas/Popup02_Red/Gold").gameObject.SetActive(true);
                GameObject.Find("ResultCanvas/Popup02_Red/Silver").gameObject.SetActive(false);
                GameObject.Find("ResultCanvas/Popup02_Red/Bronze").gameObject.SetActive(false);
                resultTxt.text = "���ϵ帳�ϴ�! �ݸ޴��Դϴ�!!";
                break;
            case Medal.Silver:
                GameObject.Find("ResultCanvas/Popup02_Red/Gold").gameObject.SetActive(false);
                GameObject.Find("ResultCanvas/Popup02_Red/Silver").gameObject.SetActive(true);
                GameObject.Find("ResultCanvas/Popup02_Red/Bronze").gameObject.SetActive(true);
                resultTxt.text = "�ƽ����ϴ�, �ݸ޴޿� �����غ�����!";
                break;
            case Medal.Bronze:
                GameObject.Find("ResultCanvas/Popup02_Red/Gold").gameObject.SetActive(false);
                GameObject.Find("ResultCanvas/Popup02_Red/Silver").gameObject.SetActive(false);
                GameObject.Find("ResultCanvas/Popup02_Red/Bronze").gameObject.SetActive(true);
                resultTxt.text = "�ٽ� �����غ�����!";
                break;
        }
    }

    public void Calculator()
    {
        // index - 2�� ������, index - 1�� ���ѽð� ������ �� �߰� ����
        int maxIndex = GameControl.control.totalCarCount;
        for (int i = 0; i < maxIndex - 2; i++)
        {
            // ȹ���� �������� ���� ������ ���� ������ ���� ���Ѵ�
            // �ռ� ������ +����, ������ -������ totalScore �迭�� �־�ξ��� ������, 0�� �������� �Ǵ��Ѵ�
            // ���� ������ ���� ������ ǥ���ϵ��� UI�� ������� ������ ���� ������ش�
            int _score = GameControl.control.totalScore[i];
            if (_score > 0)
            {
                oScore += _score; ++oCount;
            }
            else if (_score < 0)
            {
                xScore += _score; ++xCount;
            }
            sum += _score;
        }

        // ���ѽð�
        timeScore = GameControl.control.totalScore[maxIndex - 1];

        // ������
        itemScore = GameControl.control.totalScore[maxIndex - 2];
        Debug.Log($"item score : {GameControl.control.totalScore[maxIndex - 2]}");

        // �����۰� ���ѽð����� ���� �� ����
        sum += timeScore;
        sum += itemScore;
    }
}
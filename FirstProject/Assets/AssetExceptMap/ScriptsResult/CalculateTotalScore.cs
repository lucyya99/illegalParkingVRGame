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
    // 금, 은, 동
    // 제한시간이 5분이라고 하면, 제한시간 점수는 5 * 60 * 10 = 3000이 최대
    // 정말 빨리해도 4분만에 찾을 수 있을 것이라 예상 60 * 10 = 600
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

        // 개수 & 점수 세팅
        Calculator();

        // count UI 세팅
        setCountTxt(oScoreObj, oCount, GameControl.control.totalCarCount);
        setCountTxt(xScoreObj, xCount, GameControl.control.totalCarCount);
        setCountTxt(timeScoreObj, (int)(timeScore / 10), (int) GameControl.control.initialTimer);
        int itemCount = (int)(itemScore / 5);
        Debug.Log($"{itemCount}");
        itemScoreObj.transform.GetChild(0).GetComponent<TMP_Text>().text = itemCount.ToString();

        // score UI 세팅
        setScoreTxt(oScoreObj, oScore);
        setScoreTxt(xScoreObj, xScore);
        setScoreTxt(timeScoreObj, timeScore);
        setScoreTxt(itemScoreObj, itemScore);
        setScoreTxt(totalScoreObj, sum);

        setMedal();
    }

    // 계산기
    public void setCountTxt(GameObject _parent, int _count, int _totalCount)
    {
        GameObject _countObj = _parent.transform.GetChild(0).gameObject;
        TMP_Text _countTxt = _countObj.GetComponent<TMP_Text>();

        _countTxt.text = _count.ToString() + "/" + _totalCount;
    }

    public void setScoreTxt(GameObject _parent, int _score)
    {
        // 점수 오브젝트를 가져옴
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
                resultTxt.text = "축하드립니다! 금메달입니다!!";
                break;
            case Medal.Silver:
                GameObject.Find("ResultCanvas/Popup02_Red/Gold").gameObject.SetActive(false);
                GameObject.Find("ResultCanvas/Popup02_Red/Silver").gameObject.SetActive(true);
                GameObject.Find("ResultCanvas/Popup02_Red/Bronze").gameObject.SetActive(true);
                resultTxt.text = "아쉽습니다, 금메달에 도전해보세요!";
                break;
            case Medal.Bronze:
                GameObject.Find("ResultCanvas/Popup02_Red/Gold").gameObject.SetActive(false);
                GameObject.Find("ResultCanvas/Popup02_Red/Silver").gameObject.SetActive(false);
                GameObject.Find("ResultCanvas/Popup02_Red/Bronze").gameObject.SetActive(true);
                resultTxt.text = "다시 도전해보세요!";
                break;
        }
    }

    public void Calculator()
    {
        // index - 2는 아이템, index - 1은 제한시간 남았을 때 추가 점수
        int maxIndex = GameControl.control.totalCarCount;
        for (int i = 0; i < maxIndex - 2; i++)
        {
            // 획득한 점수들을 정답 점수와 오답 점수에 각각 더한다
            // 앞서 정답은 +점수, 오답은 -점수로 totalScore 배열에 넣어두었기 때문에, 0을 기준으로 판단한다
            // 정답 개수와 오답 개수를 표시하도록 UI를 만들었기 때문에 같이 계산해준다
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

        // 제한시간
        timeScore = GameControl.control.totalScore[maxIndex - 1];

        // 아이템
        itemScore = GameControl.control.totalScore[maxIndex - 2];
        Debug.Log($"item score : {GameControl.control.totalScore[maxIndex - 2]}");

        // 아이템과 제한시간까지 더한 총 점수
        sum += timeScore;
        sum += itemScore;
    }
}
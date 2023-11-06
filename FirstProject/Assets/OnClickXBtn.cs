using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickXBtn : MonoBehaviour
{
    // About Game Score
    // 게임 UI에 표시될 멘트
    public readonly string[] ScoreDescribe = new string[7]
    {
    "어린이보호구역은\n1분 이상 주정차가 불가능합니다",
    "소화전 주변 5m 이내는\n1분 이상 주정차가 불가능합니다",
    "버스정류장 10m 이내는\n1분 이상 주정차가 불가능합니다",    
    "교차로 모퉁이 5m 이내는\n1분 이상 주정차가 불가능합니다",
    "횡단보도에서는\n1분 이상 주정차가 불가능합니다",
    "인도(보도)에서\n 1분 이상 주정차가 불가능합니다",
    "이곳에서는\n주정차가 가능합니다",
    };
    public readonly string overTwoHourTxt = "2시간 이상 주정차시\n과태료가 1만원 추가됩니다";

    bool playerAnswer;
    CarProperty selectedCarPR;
    GameObject scoreTxt;
    GameObject scoreDescribe;
    PlayerProperty playerPR;

    public void OnClickBtn()
    {
        // 어차피 메서드를 분리해서 스크립트 작성하므로, playerAnswer도 미리 초기화
        playerAnswer = false;
        playerPR = GameObject.Find("XR Origin (XR Rig)").GetComponent<PlayerProperty>();

        // 선택된 차량, 선택된 캔버스에 표시할 UI 요소들 세팅
        setSelectedObj();

        // 점수 계산 및 점수 관련 문자열 설정
        CalculateScore();

        // O, X 버튼과 차량은 없애고, UI 요소를 나타냄
        setObjActive();

        // UI 요소가 나타난 후, 점수 애니메이션이 1초 정도 나타나고 없어지며,
        // 그 후 팝업창이 2.5초 정도 나타나고 사라진다
    }

    private void setSelectedObj()
    {
        // 선택된 차량 넣기
        // Canvas에서 자동차 넘버를 가지고 있음
        // 1. OBtn이나, XBtn의 부모, OXBtn
        // 2. OXBtn의 부모인 Canvas의 CarCanvasManager를 불러와서 car index 확인
        GameObject _btnObj = this.gameObject;
        GameObject _canvasObj = _btnObj.transform.parent.gameObject.transform.parent.gameObject;

        // 선택된 차량을 저장한다
        // CarProperty 내부에 저장된 변수를 가져와 비교
        List<CarProperty> allCarPropertis = new List<CarProperty>(FindObjectsOfType<CarProperty>());
        foreach (CarProperty carProperty in allCarPropertis)
        {
            // 조건에 맞으면, 선택된 차량
            GameObject willBeMyCar = carProperty.myCanvasGO;
            if (willBeMyCar.Equals(_canvasObj))
            {
                selectedCarPR = carProperty;
                break;
            }
        }

        // 선택된 Canvas 밑에 있는 scoreTxt
        scoreTxt = _canvasObj.transform.Find("ScoreTxt").gameObject;

        // 선택된 Canvas 밑에 있는 scoreDescribe
        scoreDescribe = _canvasObj.transform.Find("DescribePopup").gameObject;
        scoreTxt.GetComponent<MoveAnim>().describePopupObj = scoreDescribe;
    }

    // 플레이어가 자동차 선택 -> [O / X]를 선택했을 때 해당 메서드 실행
    // 선택된 자동차가 무엇인지 / 사용자의 응답이 무엇인지 들어가야함
    // 유저가 몇번째로 선택했는지 추가해서 점수 계산하는 배열에 집어넣음. 해당 배열 사용해서 결과화면에서 정답/오답 개수와 최종 점수 보여줄 예정
    private void CalculateScore()
    {
        int score = 0;

        // 선택된 자동차의 장소에 따라 설명 UI에 표시
        string scoreDescribeTxt = ScoreDescribe[(int)selectedCarPR.carSiteName];

        // 정답인지, 오답인지 판별
        bool isRightAnswer = IsCorrectAnswer(selectedCarPR, playerAnswer);

        // 소리 삽입할 오브젝트를 문자열로 지정
        string answerSound = "";

        // 정답이면, 점수 +
        if (isRightAnswer == true)
        {
            answerSound = "RightSound";
            score = GameControl.control.Score[(int)selectedCarPR.carType, (int)selectedCarPR.carSiteName];

            // 불법일때, 2시간 이상인지 체크
            if (selectedCarPR.carStopTime >= 120 && selectedCarPR.isLegal == false)
            {
                ++score;
                // 2시간 이상이면, 2시간 이상에 대한 UI가 추가되어야함
                scoreDescribeTxt += "\n\n";
                scoreDescribeTxt += overTwoHourTxt;
            }
        }

        // 오답이면, 점수 -
        if (isRightAnswer == false)
        {
            answerSound = "WrongSound";
            score = -GameControl.control.Score[(int)selectedCarPR.carType, (int)selectedCarPR.carSiteName];

            // 불법일때, 2시간 이상인지 체크
            if (selectedCarPR.carStopTime >= 120 && selectedCarPR.isLegal == false)
            {
                --score;
                // 2시간 이상이면, 2시간 이상에 대한 UI가 추가되어야함
                scoreDescribeTxt += "\\n\\n";
                scoreDescribeTxt += overTwoHourTxt;
            }
        }

        // 오디오 재생
        selectedCarPR.myCanvasGO.transform.Find(answerSound).GetComponent<AudioSource>().Play();

        // Player 점수에 추가
        GameControl.control.totalScore[playerPR.tryCounter] = score;

        // 점수 UI 변경
        TMP_Text temp = scoreTxt.GetComponent<TMP_Text>();
        string sign = score > 0 ? "+" : "";
        temp.text = sign + score;

        TMP_Text temp2 = scoreDescribe.transform.Find("Score").gameObject.GetComponent<TMP_Text>();
        if (selectedCarPR.isLegal == false) temp2.text = "과태료 " + Mathf.Abs(score) + "만원";
        else temp2.text = "";

        // 설명 UI 내용도 변경
        setScoreTxt(selectedCarPR, scoreDescribeTxt);

        // Player 정답 시도 횟수 추가
        ++playerPR.tryCounter;
    }

    private bool IsCorrectAnswer(CarProperty selectedCarPR, bool userAnswer)
    {
        // 합법 -> 유저 선택 = O면 오답, 합법 -> 유저 선택 = X면 정답
        if (selectedCarPR.isLegal == true)
        {
            if (userAnswer == true) { return false; }
            if (userAnswer == false) { return true; }
        }
        // 불법 -> 유저 선택 = O면 정답, 불법 -> 유저 선택 = X면 오답
        if (selectedCarPR.isLegal == false)
        {
            if (userAnswer == true) { return true; }
            if (userAnswer == true) { return false; }
        }

        // car에 대한 설정이 안되어있으면 무조건 오답으로 설정
        return false;
    }

    private void setObjActive()
    {
        // 현재 선택된 오브젝트 이름
        GameObject clickObj = EventSystem.current.currentSelectedGameObject;

        // 선택된 오브젝트의 부모 오브젝트를 없앰
        // = OBtn의 부모 오브젝트 = OXBtn => O/X 버튼을 모두 없앨 수 있음
        GameObject clickParentObj = clickObj.transform.parent.gameObject;
        clickParentObj.SetActive(false);

        // 선택된 차량도 함께 제거
        selectedCarPR.gameObject.SetActive(false);

        // 점수 + 애니메이션 나타남
        scoreTxt.SetActive(true);
    }

    private void setScoreTxt(CarProperty _selectedCarPR, string _scoreTxt)
    {
        // 팝업 제목에 불법 주정차 구역과 주정차 가능 구역 중 해당되는 것 표시
        if (_selectedCarPR.isLegal == false)
        {
            scoreDescribe.transform.Find("Illegal").gameObject.SetActive(true);
            scoreDescribe.transform.Find("Legal").gameObject.SetActive(false);
        }
        if (_selectedCarPR.isLegal == true)
        {
            scoreDescribe.transform.Find("Illegal").gameObject.SetActive(false);
            scoreDescribe.transform.Find("Legal").gameObject.SetActive(true);
        }

        // 장소에 맞게 text를 바꿔줌
        GameObject contextObj = scoreDescribe.transform.Find("Describe").gameObject;
        TMP_Text contextTxt = contextObj.GetComponent<TMP_Text>();
        contextTxt.text = _scoreTxt;
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public enum SiteName
{
    ChildrenZone,                       // 어린이보호구역
    FireHydrant,                        // 소화전
    BusStop,                            // 버스정류장
    Intersection,                       // 교차로
    Crosswalk,                          // 횡단보도
    Sidewalk,                           // 인도(보도)
    NotIncluded,                        // 주정차 합법 구역
    Max,
};

public enum CarType
{
    General,
    Special,
    Max,
};

public class GameManager : MonoBehaviour
{
    // About Game Timer
    private float gameTimer = GameControl.control.initialTimer;
    private string timeFormat;
    private GameObject timerObj;
    private TMP_Text timerObjTxt;
    private GameObject menuObj;

    GameObject player;
    PlayerProperty playerPR;

    // About Coin
    GameObject[] coinRoads;
    GameObject[] coinPrefabs;

    // Car 배치관련 세팅
    int maxIllegalCars = 15; // 최대 불법 주차 차량 수
    int maxNormalCars = 5; // 최대 정상 주차 차량 수

    int minCarStopTime = 1; // 최소 주차 시간(분)
    int maxCarStopTime = 80; // 최대 주차 시간(분)
    List<CarProperty> carList = new List<CarProperty>();    // 동전 배치시 사용. 다른 용도 x

    // Start is called before the first frame update
    void Start()
    {
        // 점수 관련 초기 세팅
        InitScore();

        // 타이머 관련 초기 세팅
        InitTimer();

        // 플레이어 관련 초기 세팅
        InitPlayer();

        // 자동차 초기 배치
        InitCars();

        // 동전 초기 배치
        CreateCoinRoad();
    }

    // Update is called once per frame
    void Update()
    {
        // 타이머 기능
        SetTimer();

        // 제한시간 전 플레이어가 선택을 완료한 경우, 남은 시간 * 10점을 더하고 게임을 종료한다
        if (gameTimer > 0 && playerPR.tryCounter >= GameControl.control.totalCarCount)
        {
            // 점수 가산
            int maxIndex = GameControl.control.totalScore.Length - 1;
            GameControl.control.totalScore[maxIndex] = ((int)gameTimer) * 10;

            Debug.Log($"plus score : {GameControl.control.totalScore[maxIndex]}");

            // 게임 종료
            GameObject.Find("EndingSound/BadEnding").GetComponent<AudioSource>().Play();
            EndGame();
        }
    }

    private void CreateCoinRoad()
    {
        coinRoads = GameObject.FindGameObjectsWithTag("ItemRoad");
        coinPrefabs = GameObject.FindGameObjectsWithTag("Item");

        // ???? ?? ????? ??????? ???
        foreach (GameObject coinRoad in coinRoads)
        {
            RoadProperty road = coinRoad.GetComponent<RoadProperty>();
            Collider roadCollider = coinRoad.GetComponent<Collider>();
            Bounds roadBounds = roadCollider.bounds;

            float distance = 15f;
            if (road.isHorizontal == true)
            {
                // z?? ???, x?? ????
                // ???? : bound.center.x, bound.position.y, bound.min.z + ????
                float posZ = roadBounds.min.z + distance;
                while (posZ < roadBounds.max.z)
                {
                    // ???? ?????? ?????? ?? ?? ???
                    Vector3 willBeCoinPos = new Vector3(roadBounds.center.x, coinRoad.transform.position.y, posZ);
                    float coinCarDistance = 7.0f;

                    // ???? ????
                    int coinKind = Random.Range(0, 4);
                    GameObject willbeCoin = Instantiate(coinPrefabs[coinKind]);
                    willbeCoin.transform.position = willBeCoinPos;

                    foreach (CarProperty car in carList)
                    {
                        if (Vector3.Distance(car.transform.position, willBeCoinPos) < coinCarDistance) Destroy(willbeCoin);
                    }
                    posZ += distance;
                }
            }
            else
            {
                // x?? ???, z?? ????
                // ???? bound.min.x + ????, bound.position.y, bound.center.z
                float posX = roadBounds.min.x;
                while (posX <= roadBounds.max.x)
                {
                    // ???? ?????? ?????? ?? ?? ???
                    posX += distance;
                    Vector3 willBeCoinPos = new Vector3(posX, coinRoad.transform.position.y, roadBounds.center.z);
                    float coinCarDistance = 7.0f;

                    // ???? ????
                    int coinKind = Random.Range(0, 4);
                    GameObject willbeCoin = Instantiate(coinPrefabs[coinKind]);
                    willbeCoin.transform.position = willBeCoinPos;

                    foreach (CarProperty car in carList)
                    {
                        if (Vector3.Distance(car.transform.position, willBeCoinPos) < coinCarDistance) Destroy(willbeCoin);
                    }
                }
            }
        }
    }

    private void InitScore()
    {
        // 1) 최대 정답/오답 가능 수 : 총 자량 대수, 실시간 반영시 마이너스가 될 수 있으므로 배열로 관리해서 나중에 한꺼번에 보여줌
        // 2) 아이템 획득한 경우, 제한시간 남긴 경우 추가
        Array.Fill(GameControl.control.totalScore, 0);  // 자동차 선택이 안된 경우와 제한시간 내에 선택을 끝내지 못한 경우를 대비해 전부 0으로 초기값 세팅
    }

    private void InitPlayer()
    {
        player = GameObject.Find("XR Origin (XR Rig)");
        playerPR = player.GetComponent<PlayerProperty>();
    }

    private void InitCars()
    {
        // 자동차 배치 관련 시작 ///////////
        // 주정차 영역이 합법 구역인지 아닌지 목록 분류
        // 불법/정상 RangeManager를 가져오기

        int max = GameControl.control.totalCarCount;
        RangeManager[] allRangeManagers = FindObjectsOfType<RangeManager>();
        List<RangeManager> legalRangeManagers = new List<RangeManager>();
        List<RangeManager> illegalRangeManagers = new List<RangeManager>();
        CarProperty car = new CarProperty();

        foreach (RangeManager range in allRangeManagers)
        {
            if (!range.isLegal)
            {
                illegalRangeManagers.Add(range);
            }
            else
            {
                legalRangeManagers.Add(range);
            }
        }

        Debug.Log("illegalRangeManagers.Count: " + illegalRangeManagers.Count);
        Debug.Log("legalRangeManagers.Count: " + legalRangeManagers.Count);

        List<CarProperty> allCarPropertis = new List<CarProperty>(FindObjectsOfType<CarProperty>());
        Debug.Log("allCarPropertis.Count: " + allCarPropertis.Count);
        CarProperty selectedCar;
        int carIdx, randomIndex;

        int illegalCnt = illegalRangeManagers.Count;
        Debug.Log(illegalCnt + " " + maxIllegalCars);
        //불법주정차구역 (illegalRangeManagers) 리스트에서 15개 범위를 임의로 선택하여 자동차 배치 (SpawnRandomCar) 함수에 전달
        for (int i = 0; i < illegalCnt; i++)
        {
            Debug.Log($"{i}");
            carIdx = Random.Range(0, allCarPropertis.Count);
            selectedCar = allCarPropertis[carIdx];
            randomIndex = Random.Range(0, illegalRangeManagers.Count);
            RangeManager selectedRange = illegalRangeManagers[randomIndex];

            // 자동차 배치 함수에 선택된 RangeManager를 전달
            car = selectedCar.SpawnRandomCar(randomIndex, selectedRange, minCarStopTime, maxCarStopTime);
            carList.Add(car);

            // 선택된 RangeManager를 리스트에서 제거
            illegalRangeManagers.RemoveAt(randomIndex);
        }

        int legalCnt = legalRangeManagers.Count;
        //정상정차구역 (legalRangeManagers) 리스트에서 5개 범위를 임의로 선택하여 자동차 배치 (SpawnRandomCar) 함수에 전달
        for (int i = 0; i < maxIllegalCars && i < legalCnt; i++)
        {
            carIdx = Random.Range(0, allCarPropertis.Count);
            selectedCar = allCarPropertis[carIdx];
            randomIndex = Random.Range(0, legalRangeManagers.Count);
            RangeManager selectedRange = legalRangeManagers[randomIndex];

            // 자동차 배치 함수에 선택된 RangeManager를 전달
            car = selectedCar.SpawnRandomCar(randomIndex, selectedRange, minCarStopTime, maxCarStopTime);
            carList[i] = car;

            // 선택된 RangeManager를 리스트에서 제거
            legalRangeManagers.RemoveAt(randomIndex);
        }
        Debug.Log("carList.Count: " + carList.Count);
    }

    public void EndGame()
    {
        MenuManagerFromGame temp = menuObj.GetComponent<MenuManagerFromGame>();
        temp.goResult();
    }

    // 타이머 관련 메서드
    private void InitTimer()
    {
        timeFormat = TimeCount(false); // Text에 초기값을 넣어 주기 위해

        timerObj = GameObject.Find("MainUICanvas/GameUI/Time");
        timerObjTxt = timerObj.GetComponent<TMP_Text>();
        timerObjTxt.text = timeFormat;

        menuObj = GameObject.Find("MenuManager");
    }

    private void SetTimer()
    {
        // 씬 전환 시 -> 게임 시작
        timeFormat = TimeCount();

        // gameTimer가 줄어들때, 완전히 0에 맞출수 없기 때문에
        if (gameTimer <= 0)
        {
            SetTimerZero();

            // 카운트 다운 종료시 이벤트
            GameObject.Find("EndingSound/BadEnding").GetComponent<AudioSource>().Play();
            EndGame();
        }

        // UI Setting
        timerObjTxt.text = timeFormat;
    }

    private string TimeCount(bool IsUpdate = true)
    {
        if (IsUpdate)
            gameTimer -= Time.deltaTime;

        TimeSpan timespan = TimeSpan.FromSeconds(gameTimer);
        string timer = string.Format("{0:00}:{1:00}",
            timespan.Minutes, timespan.Seconds);        // time format -> 03:30, 3분 30초

        return timer;
    }

    private void SetTimerZero()
    {
        timeFormat = @"00:00";
        gameTimer = 0;
    }
}
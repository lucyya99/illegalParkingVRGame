
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
    ChildrenZone,                       // ��̺�ȣ����
    FireHydrant,                        // ��ȭ��
    BusStop,                            // ����������
    Intersection,                       // ������
    Crosswalk,                          // Ⱦ�ܺ���
    Sidewalk,                           // �ε�(����)
    NotIncluded,                        // ������ �չ� ����
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

    // Car ��ġ���� ����
    int maxIllegalCars = 15; // �ִ� �ҹ� ���� ���� ��
    int maxNormalCars = 5; // �ִ� ���� ���� ���� ��

    int minCarStopTime = 1; // �ּ� ���� �ð�(��)
    int maxCarStopTime = 80; // �ִ� ���� �ð�(��)
    List<CarProperty> carList = new List<CarProperty>();    // ���� ��ġ�� ���. �ٸ� �뵵 x

    // Start is called before the first frame update
    void Start()
    {
        // ���� ���� �ʱ� ����
        InitScore();

        // Ÿ�̸� ���� �ʱ� ����
        InitTimer();

        // �÷��̾� ���� �ʱ� ����
        InitPlayer();

        // �ڵ��� �ʱ� ��ġ
        InitCars();

        // ���� �ʱ� ��ġ
        CreateCoinRoad();
    }

    // Update is called once per frame
    void Update()
    {
        // Ÿ�̸� ���
        SetTimer();

        // ���ѽð� �� �÷��̾ ������ �Ϸ��� ���, ���� �ð� * 10���� ���ϰ� ������ �����Ѵ�
        if (gameTimer > 0 && playerPR.tryCounter >= GameControl.control.totalCarCount)
        {
            // ���� ����
            int maxIndex = GameControl.control.totalScore.Length - 1;
            GameControl.control.totalScore[maxIndex] = ((int)gameTimer) * 10;

            Debug.Log($"plus score : {GameControl.control.totalScore[maxIndex]}");

            // ���� ����
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
        // 1) �ִ� ����/���� ���� �� : �� �ڷ� ���, �ǽð� �ݿ��� ���̳ʽ��� �� �� �����Ƿ� �迭�� �����ؼ� ���߿� �Ѳ����� ������
        // 2) ������ ȹ���� ���, ���ѽð� ���� ��� �߰�
        Array.Fill(GameControl.control.totalScore, 0);  // �ڵ��� ������ �ȵ� ���� ���ѽð� ���� ������ ������ ���� ��츦 ����� ���� 0���� �ʱⰪ ����
    }

    private void InitPlayer()
    {
        player = GameObject.Find("XR Origin (XR Rig)");
        playerPR = player.GetComponent<PlayerProperty>();
    }

    private void InitCars()
    {
        // �ڵ��� ��ġ ���� ���� ///////////
        // ������ ������ �չ� �������� �ƴ��� ��� �з�
        // �ҹ�/���� RangeManager�� ��������

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
        //�ҹ����������� (illegalRangeManagers) ����Ʈ���� 15�� ������ ���Ƿ� �����Ͽ� �ڵ��� ��ġ (SpawnRandomCar) �Լ��� ����
        for (int i = 0; i < illegalCnt; i++)
        {
            Debug.Log($"{i}");
            carIdx = Random.Range(0, allCarPropertis.Count);
            selectedCar = allCarPropertis[carIdx];
            randomIndex = Random.Range(0, illegalRangeManagers.Count);
            RangeManager selectedRange = illegalRangeManagers[randomIndex];

            // �ڵ��� ��ġ �Լ��� ���õ� RangeManager�� ����
            car = selectedCar.SpawnRandomCar(randomIndex, selectedRange, minCarStopTime, maxCarStopTime);
            carList.Add(car);

            // ���õ� RangeManager�� ����Ʈ���� ����
            illegalRangeManagers.RemoveAt(randomIndex);
        }

        int legalCnt = legalRangeManagers.Count;
        //������������ (legalRangeManagers) ����Ʈ���� 5�� ������ ���Ƿ� �����Ͽ� �ڵ��� ��ġ (SpawnRandomCar) �Լ��� ����
        for (int i = 0; i < maxIllegalCars && i < legalCnt; i++)
        {
            carIdx = Random.Range(0, allCarPropertis.Count);
            selectedCar = allCarPropertis[carIdx];
            randomIndex = Random.Range(0, legalRangeManagers.Count);
            RangeManager selectedRange = legalRangeManagers[randomIndex];

            // �ڵ��� ��ġ �Լ��� ���õ� RangeManager�� ����
            car = selectedCar.SpawnRandomCar(randomIndex, selectedRange, minCarStopTime, maxCarStopTime);
            carList[i] = car;

            // ���õ� RangeManager�� ����Ʈ���� ����
            legalRangeManagers.RemoveAt(randomIndex);
        }
        Debug.Log("carList.Count: " + carList.Count);
    }

    public void EndGame()
    {
        MenuManagerFromGame temp = menuObj.GetComponent<MenuManagerFromGame>();
        temp.goResult();
    }

    // Ÿ�̸� ���� �޼���
    private void InitTimer()
    {
        timeFormat = TimeCount(false); // Text�� �ʱⰪ�� �־� �ֱ� ����

        timerObj = GameObject.Find("MainUICanvas/GameUI/Time");
        timerObjTxt = timerObj.GetComponent<TMP_Text>();
        timerObjTxt.text = timeFormat;

        menuObj = GameObject.Find("MenuManager");
    }

    private void SetTimer()
    {
        // �� ��ȯ �� -> ���� ����
        timeFormat = TimeCount();

        // gameTimer�� �پ�鶧, ������ 0�� ����� ���� ������
        if (gameTimer <= 0)
        {
            SetTimerZero();

            // ī��Ʈ �ٿ� ����� �̺�Ʈ
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
            timespan.Minutes, timespan.Seconds);        // time format -> 03:30, 3�� 30��

        return timer;
    }

    private void SetTimerZero()
    {
        timeFormat = @"00:00";
        gameTimer = 0;
    }
}
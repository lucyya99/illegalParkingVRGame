using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using static GameManager;
using Random = UnityEngine.Random;

public class CarProperty : MonoBehaviour
{
    public int carIndex;
    public CarType carType;
    public float carDirection;   // �ڵ����� ����/���η� ��ġ�� ������ ����

    //�ҹ��������� 15��, �������������� 5�� ��ġ�� ����
    // 1. �ҹ��������� (�ҹ����������� ��ġ) : false
    // 2. ������������ (�������������� ��ġ) : true
    public bool isLegal;
    public int carStopTime = 0;
    public bool isIllegalParking = false;

    public SiteName carSiteName;
    public RangeManager myZone;
    public GameObject myCanvasGO;

    // Start is called before the first frame update
    void Start()
    {


        // �ʱ�ȭ�� �� Ÿ�̸Ӹ� ����
        StartCoroutine(UpdateCarStopTime());
    }

    // Update is called once per frame
    void Update()
    {
        // ���⿡ �ٸ� ������Ʈ ������ �߰��� �� �ֽ��ϴ�.
    }

    // 1�и��� carStopTime�� ������Ű�� �ڷ�ƾ
    private IEnumerator UpdateCarStopTime()
    {
        while (true)
        {
            // 1��(60��)���� carStopTime ����
            yield return new WaitForSeconds(60f);
            carStopTime++;
            if (!isLegal && carStopTime > 60)
            {
                SetIllegalParking(true);
            }
        }
    }

    // �ڵ����� spawnArea ������ ������ġ�� ��ġ
    public CarProperty SpawnRandomCar(int carListIndex, RangeManager zone, int minCarStopTime, int maxCarStopTime)
    {
        // ���õ� ���� �ݶ��̴��� spawnArea�� ����
        Collider spawnArea = zone.gameObject.GetComponent<Collider>();
        // ��ġ�� ������ ��踦 ������
        Bounds bounds = spawnArea.bounds;

        // ������ ��ġ�� ��ġ�� ���� ������ ����
        Vector3 randomPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            spawnArea.transform.position.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        GameObject newCar = Instantiate(gameObject, randomPosition, Quaternion.identity);
        // newCar�� ȸ����Ŵ
        newCar.transform.Rotate(0f, zone.carDirection, 0f);
        Vector3 checkPosition = newCar.transform.position;


        Debug.Log(checkPosition + " is in " + zone.siteName);

        // ������ ���� �ð� ����
        carStopTime = Random.Range(minCarStopTime, maxCarStopTime);

        // �ҹ� ���� ���� �Ǵ�
        if (!isLegal)
        {
            // newCar�� ���� ��ġ�� ���� Ȯ���Ͽ� �ҹ� ����ð��� 1�� �̻����� Ȯ���Ͽ� ���� ����
            isIllegalParking = carStopTime > 60;
        }

        // CarProperty ������Ʈ�� �����ͼ� �Ӽ� ����
        CarProperty carProperty = newCar.GetComponent<CarProperty>();
        carProperty.myZone = zone;
        carProperty.carSiteName = zone.siteName;
        carProperty.isLegal = zone.isLegal;
        carProperty.carDirection = zone.carDirection;
        carProperty.isIllegalParking = isIllegalParking;
        carProperty.carStopTime = carStopTime;
        carProperty.carIndex = carListIndex;

        GameObject carCanvas = GameObject.Find("CarCanvas").gameObject;
        if (carCanvas != null)
        {
            GameObject canvas = carCanvas.transform.Find("Canvas").gameObject;
            if (canvas != null)
            {
                // canvas ������ �� ó���� ���� �߰�
                // Canvas�� �̹� �����ϴ� ���, �̸� �����Ͽ� CarCanvas�� �ڽ����� ����ϴ�.
                GameObject copiedCanvas = Instantiate(canvas, carCanvas.transform);

                // ���õ� ���� �ݶ��̴��� spawnArea�� ����
                Collider newCarCol = newCar.GetComponent<Collider>();
                // ��ġ�� ������ ��踦 ������
                Bounds boundsCarCol = newCarCol.bounds;

                Vector3 copiedCanvasPosition = copiedCanvas.transform.position;
                copiedCanvasPosition.x = newCar.transform.position.x;
                copiedCanvasPosition.z = newCar.transform.position.z;
                copiedCanvasPosition.y = newCar.transform.position.y + boundsCarCol.max.y * 1.2f;
                copiedCanvas.transform.position = copiedCanvasPosition;

                GameObject player = GameObject.Find("XR Origin (XR Rig)");
                // �÷��̾ �ٶ󺸵��� ȸ����Ű��
                Vector3 playerDirection = player.transform.position - copiedCanvas.transform.position;
                Quaternion playerRotation = Quaternion.LookRotation(playerDirection);
                copiedCanvas.transform.rotation = playerRotation;
                copiedCanvas.transform.Rotate(0f, 180, 0f);

                // 0번에 위치한 오브젝트 : 정차시간 넣기
                TMP_Text stopTimeTxt = copiedCanvas.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
                stopTimeTxt.text = "+" + carStopTime + "분";

                // object child count
                int childCount = copiedCanvas.transform.childCount;

                // active 설정
                for (int i = 0; i < childCount; ++i)
                {
                    if (i == 0)
                        copiedCanvas.transform.GetChild(i).gameObject.SetActive(true);
                    else
                        copiedCanvas.transform.GetChild(i).gameObject.SetActive(false);
                }

                carProperty.myCanvasGO = copiedCanvas;
            }
        }

        return carProperty;
    }

    // Ŭ�� �� ������ �޼���
    public void SelectCar()
    {
        // 오디오 재생
        this.GetComponent<AudioSource>().Play();

        // �ڵ����� Ŭ���ϸ� �� �޼��尡 ȣ��˴ϴ�.
        // Ŭ�� �� ���ϴ� ������ �߰��ϼ���.
        Debug.Log("Car Clicked: " + carIndex + ":" + carSiteName + ":" + isLegal + ":" + isIllegalParking + ":" + carStopTime);

        // Canvas Active Setting
        myCanvasGO.transform.GetChild(0).gameObject.SetActive(false);
        myCanvasGO.transform.GetChild(1).gameObject.SetActive(true);
        myCanvasGO.transform.GetChild(2).gameObject.SetActive(true);

        Destroy(this.GetComponent<XRSimpleInteractable>());
    }

    public void SetIllegalParking(bool illegal)
    {
        isIllegalParking = illegal;
    }
}
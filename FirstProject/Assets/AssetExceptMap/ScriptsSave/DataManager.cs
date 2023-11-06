using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    static GameObject container;

    //�̱������� ����
    static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (!instance)
            {
                //���ο� ������Ʈ ����
                container = new GameObject();

                //������Ʈ �̸� ����
                container.name = "DataManager";

                //���ο� ������Ʈ�� ������ �Ŵ��� �ְ�, �ν��Ͻ��� �Ҵ�
                instance = container.AddComponent(typeof(DataManager)) as DataManager;

                //�ش� ������Ʈ�� ������� �ʵ��� ����
                DontDestroyOnLoad(container);
            }
            return instance;
        }
    }

    //���� ������ �����̸� ����
    string GameDataFileName = "GameData.json";

    //����� Ŭ���� ����
    public Data data = new Data();

    //�ҷ�����
    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        //����� ������ �ִٸ� ����� ���� �о���� Json�� Ŭ���� �������� ��ȯ�ؼ� �Ҵ�
        if (File.Exists(filePath))
        {
            string FromJsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<Data>(FromJsonData);
            Debug.Log("�ҷ����� �Ϸ�");
        }

        // �ҷ����� ���� ������ ����..?
    }

    //�����ϱ�
    public void SaveGameData()
    {
        //Ŭ������ Json �������� ��ȯ
        string ToJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        //�̹� ����� ������ �ִٸ� �����, ���ٸ� ���� ���� ����
        File.WriteAllText(filePath, ToJsonData);

        //�ùٸ��� ����ƴ��� Ȯ��
        Debug.Log("���� �Ϸ�");
        Debug.Log($"{data.playTime}");
        Debug.Log($"{data.score}");
        Debug.Log($"{data.medalName}");
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagerFromGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void saveGameData()
    {
        DataManager.Instance.SaveGameData(); //�����ϱ�
    }

    public void onClickHome()
    {
        GameObject blueCar = GameObject.Find("Vehicle_Car_Large_Red").gameObject;
        blueCar.GetComponent<AudioSource>().Play();
        Debug.Log("����ȭ��");
        SceneManager.LoadScene("MainScene");
    }

    public void goResult()
    {
        Debug.Log("���ȭ��");
        SceneManager.LoadScene("ResultScene");
    }

    public void goGame()
    {
        GameObject redCar = GameObject.Find("Vehicle_Car_Large_Red").gameObject;
        redCar.GetComponent<AudioSource>().Play();
        Debug.Log("����ȭ��");
        SceneManager.LoadScene("GameScene");
    }
}

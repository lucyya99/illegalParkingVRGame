using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;  // 씬 전환을 위해 추가
using UnityEngine.XR.Interaction.Toolkit;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onClickNewGame()
    {
        GameObject redCar = GameObject.Find("Vehicle_Car_Large_Red").gameObject;
        redCar.GetComponent<AudioSource>().Play();
        Debug.Log("새 게임");
        SceneManager.LoadScene("GameScene");
    }

    public void onClickQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

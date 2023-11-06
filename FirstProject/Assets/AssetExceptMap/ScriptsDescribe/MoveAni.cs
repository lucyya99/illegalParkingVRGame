using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class MoveAnim : MonoBehaviour
{
    private CanvasGroup cg;
    float time = 0f;
    float aniTime = 1.5f;
    public GameObject describePopupObj;
    RectTransform uiPos;
    bool isPlaying = false;

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf == true)
        {
            if (isPlaying == true)
                return;

            isPlaying = true;
            StartCoroutine(scoreMoveAni());
        }
    }

    IEnumerator scoreMoveAni()
    {
        while (time < aniTime)
        {
            /*// Fade Out Animation
            cg = GetComponent<CanvasGroup>();
            cg.alpha = Mathf.Lerp(1f, 0f, time / aniTime);

            // transform Animation
            uiPos = GetComponent<RectTransform>();
            uiPos.Translate(uiPos.up * Time.deltaTime * 10f);*/

            /*// Bigger Animation
            transform.localScale = Vector3.one * (1 + time);*/

            time += Time.deltaTime;

            yield return null;
        }
        
        try
        {
            gameObject.SetActive(false);
            describePopupObj.SetActive(true);
            // GameObject.Find("Map/Particle System").GetComponent<CarParticle>().letPlay = false;
        }
        catch
        {
            Debug.Log("일단 무시");
        }
    }
}
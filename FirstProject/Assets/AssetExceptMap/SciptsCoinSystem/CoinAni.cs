using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class CoinAni : MonoBehaviour
{
    AudioSource coinSound;
    bool isPlay = false;
    Collider playerCollider;
    // Start is called before the first frame update
    void Start()
    {
        coinSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    float rotSpeed = 120f;

    void Update()
    {
        transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (isPlay) return;
            isPlay = true;
            playerCollider = other;
            StartCoroutine(PullCoin());
        }
    }

    IEnumerator PullCoin()
    {
        float t_GetSpeed = 8f;
        float t_Distance = 1f;

        coinSound.Play();
        while (Vector3.Distance(this.transform.position, this.playerCollider.transform.position) >= t_Distance)
        {
            transform.position = Vector3.MoveTowards(transform.position, this.playerCollider.transform.position, Time.deltaTime * t_GetSpeed);
            yield return null;
        }
        this.gameObject.SetActive(false);
        isPlay = false;
        GameControl.control.totalScore[GameControl.control.totalCarCount - 2] += 5; // 점수 합산 (코인 1개당 5점)
        Debug.Log(GameControl.control.totalScore[GameControl.control.totalCarCount - 2]);
        yield break;
    }
}

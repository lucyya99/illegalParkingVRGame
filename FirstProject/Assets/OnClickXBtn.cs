using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickXBtn : MonoBehaviour
{
    // About Game Score
    // ���� UI�� ǥ�õ� ��Ʈ
    public readonly string[] ScoreDescribe = new string[7]
    {
    "��̺�ȣ������\n1�� �̻� �������� �Ұ����մϴ�",
    "��ȭ�� �ֺ� 5m �̳���\n1�� �̻� �������� �Ұ����մϴ�",
    "���������� 10m �̳���\n1�� �̻� �������� �Ұ����մϴ�",    
    "������ ������ 5m �̳���\n1�� �̻� �������� �Ұ����մϴ�",
    "Ⱦ�ܺ���������\n1�� �̻� �������� �Ұ����մϴ�",
    "�ε�(����)����\n 1�� �̻� �������� �Ұ����մϴ�",
    "�̰�������\n�������� �����մϴ�",
    };
    public readonly string overTwoHourTxt = "2�ð� �̻� ��������\n���·ᰡ 1���� �߰��˴ϴ�";

    bool playerAnswer;
    CarProperty selectedCarPR;
    GameObject scoreTxt;
    GameObject scoreDescribe;
    PlayerProperty playerPR;

    public void OnClickBtn()
    {
        // ������ �޼��带 �и��ؼ� ��ũ��Ʈ �ۼ��ϹǷ�, playerAnswer�� �̸� �ʱ�ȭ
        playerAnswer = false;
        playerPR = GameObject.Find("XR Origin (XR Rig)").GetComponent<PlayerProperty>();

        // ���õ� ����, ���õ� ĵ������ ǥ���� UI ��ҵ� ����
        setSelectedObj();

        // ���� ��� �� ���� ���� ���ڿ� ����
        CalculateScore();

        // O, X ��ư�� ������ ���ְ�, UI ��Ҹ� ��Ÿ��
        setObjActive();

        // UI ��Ұ� ��Ÿ�� ��, ���� �ִϸ��̼��� 1�� ���� ��Ÿ���� ��������,
        // �� �� �˾�â�� 2.5�� ���� ��Ÿ���� �������
    }

    private void setSelectedObj()
    {
        // ���õ� ���� �ֱ�
        // Canvas���� �ڵ��� �ѹ��� ������ ����
        // 1. OBtn�̳�, XBtn�� �θ�, OXBtn
        // 2. OXBtn�� �θ��� Canvas�� CarCanvasManager�� �ҷ��ͼ� car index Ȯ��
        GameObject _btnObj = this.gameObject;
        GameObject _canvasObj = _btnObj.transform.parent.gameObject.transform.parent.gameObject;

        // ���õ� ������ �����Ѵ�
        // CarProperty ���ο� ����� ������ ������ ��
        List<CarProperty> allCarPropertis = new List<CarProperty>(FindObjectsOfType<CarProperty>());
        foreach (CarProperty carProperty in allCarPropertis)
        {
            // ���ǿ� ������, ���õ� ����
            GameObject willBeMyCar = carProperty.myCanvasGO;
            if (willBeMyCar.Equals(_canvasObj))
            {
                selectedCarPR = carProperty;
                break;
            }
        }

        // ���õ� Canvas �ؿ� �ִ� scoreTxt
        scoreTxt = _canvasObj.transform.Find("ScoreTxt").gameObject;

        // ���õ� Canvas �ؿ� �ִ� scoreDescribe
        scoreDescribe = _canvasObj.transform.Find("DescribePopup").gameObject;
        scoreTxt.GetComponent<MoveAnim>().describePopupObj = scoreDescribe;
    }

    // �÷��̾ �ڵ��� ���� -> [O / X]�� �������� �� �ش� �޼��� ����
    // ���õ� �ڵ����� �������� / ������� ������ �������� ������
    // ������ ���°�� �����ߴ��� �߰��ؼ� ���� ����ϴ� �迭�� �������. �ش� �迭 ����ؼ� ���ȭ�鿡�� ����/���� ������ ���� ���� ������ ����
    private void CalculateScore()
    {
        int score = 0;

        // ���õ� �ڵ����� ��ҿ� ���� ���� UI�� ǥ��
        string scoreDescribeTxt = ScoreDescribe[(int)selectedCarPR.carSiteName];

        // ��������, �������� �Ǻ�
        bool isRightAnswer = IsCorrectAnswer(selectedCarPR, playerAnswer);

        // �Ҹ� ������ ������Ʈ�� ���ڿ��� ����
        string answerSound = "";

        // �����̸�, ���� +
        if (isRightAnswer == true)
        {
            answerSound = "RightSound";
            score = GameControl.control.Score[(int)selectedCarPR.carType, (int)selectedCarPR.carSiteName];

            // �ҹ��϶�, 2�ð� �̻����� üũ
            if (selectedCarPR.carStopTime >= 120 && selectedCarPR.isLegal == false)
            {
                ++score;
                // 2�ð� �̻��̸�, 2�ð� �̻� ���� UI�� �߰��Ǿ����
                scoreDescribeTxt += "\n\n";
                scoreDescribeTxt += overTwoHourTxt;
            }
        }

        // �����̸�, ���� -
        if (isRightAnswer == false)
        {
            answerSound = "WrongSound";
            score = -GameControl.control.Score[(int)selectedCarPR.carType, (int)selectedCarPR.carSiteName];

            // �ҹ��϶�, 2�ð� �̻����� üũ
            if (selectedCarPR.carStopTime >= 120 && selectedCarPR.isLegal == false)
            {
                --score;
                // 2�ð� �̻��̸�, 2�ð� �̻� ���� UI�� �߰��Ǿ����
                scoreDescribeTxt += "\\n\\n";
                scoreDescribeTxt += overTwoHourTxt;
            }
        }

        // ����� ���
        selectedCarPR.myCanvasGO.transform.Find(answerSound).GetComponent<AudioSource>().Play();

        // Player ������ �߰�
        GameControl.control.totalScore[playerPR.tryCounter] = score;

        // ���� UI ����
        TMP_Text temp = scoreTxt.GetComponent<TMP_Text>();
        string sign = score > 0 ? "+" : "";
        temp.text = sign + score;

        TMP_Text temp2 = scoreDescribe.transform.Find("Score").gameObject.GetComponent<TMP_Text>();
        if (selectedCarPR.isLegal == false) temp2.text = "���·� " + Mathf.Abs(score) + "����";
        else temp2.text = "";

        // ���� UI ���뵵 ����
        setScoreTxt(selectedCarPR, scoreDescribeTxt);

        // Player ���� �õ� Ƚ�� �߰�
        ++playerPR.tryCounter;
    }

    private bool IsCorrectAnswer(CarProperty selectedCarPR, bool userAnswer)
    {
        // �չ� -> ���� ���� = O�� ����, �չ� -> ���� ���� = X�� ����
        if (selectedCarPR.isLegal == true)
        {
            if (userAnswer == true) { return false; }
            if (userAnswer == false) { return true; }
        }
        // �ҹ� -> ���� ���� = O�� ����, �ҹ� -> ���� ���� = X�� ����
        if (selectedCarPR.isLegal == false)
        {
            if (userAnswer == true) { return true; }
            if (userAnswer == true) { return false; }
        }

        // car�� ���� ������ �ȵǾ������� ������ �������� ����
        return false;
    }

    private void setObjActive()
    {
        // ���� ���õ� ������Ʈ �̸�
        GameObject clickObj = EventSystem.current.currentSelectedGameObject;

        // ���õ� ������Ʈ�� �θ� ������Ʈ�� ����
        // = OBtn�� �θ� ������Ʈ = OXBtn => O/X ��ư�� ��� ���� �� ����
        GameObject clickParentObj = clickObj.transform.parent.gameObject;
        clickParentObj.SetActive(false);

        // ���õ� ������ �Բ� ����
        selectedCarPR.gameObject.SetActive(false);

        // ���� + �ִϸ��̼� ��Ÿ��
        scoreTxt.SetActive(true);
    }

    private void setScoreTxt(CarProperty _selectedCarPR, string _scoreTxt)
    {
        // �˾� ���� �ҹ� ������ ������ ������ ���� ���� �� �ش�Ǵ� �� ǥ��
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

        // ��ҿ� �°� text�� �ٲ���
        GameObject contextObj = scoreDescribe.transform.Find("Describe").gameObject;
        TMP_Text contextTxt = contextObj.GetComponent<TMP_Text>();
        contextTxt.text = _scoreTxt;
    }
}

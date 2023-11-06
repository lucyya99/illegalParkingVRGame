using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using Unity.VisualScripting;

[Serializable] //����ȭ


public class Data
{
    public string playTime;
    public int score;
    public string medalName;

    public Data()
    {
        playTime = DateTime.Now.ToString("yymmdd HH:mm");
        score = 0;
        for (int i = 0; i < GameControl.control.totalScore.Length; i++)
        {
            score += GameControl.control.totalScore[i];
        }
        float standard = GameControl.control.maxScore / 3;
        if (score >= standard * 2) medalName = "�ݸ޴�";
        else if (score >= standard * 1) medalName = "���޴�";
        else medalName = "���޴�" ;
    }
}
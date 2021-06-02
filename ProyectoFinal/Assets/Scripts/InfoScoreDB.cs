using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoScoreDB : MonoBehaviour
{
    public TextMeshProUGUI highScoreStoryText;
    public TextMeshProUGUI highScoreArcadeText;

    private int highStoryScore;
    private int highArcadeScore;
    
    private DBmanager dbManagement;
    
    private void Awake()
    {
        dbManagement = this.GetComponent<DBmanager>();
        dbManagement.arcadeMode = GameManager.GM_instance.ArcadeMode;
    }
    
    void Start()
    {
        highStoryScore = dbManagement.GetHighScoreDB("StoryRecords");
        highArcadeScore = dbManagement.GetHighScoreDB("ArcadeRecords");

        highScoreStoryText.text = $"{highStoryScore} PT";
        highScoreArcadeText.text = $"{highArcadeScore} PT";
    }
}

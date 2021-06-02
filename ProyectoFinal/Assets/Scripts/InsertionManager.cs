using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InsertionManager : MonoBehaviour
{
    public TextMeshProUGUI gameMode;
    [Space]
    
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI maxScoreTxt;

    public TextMeshProUGUI infoTxt;
    
    private int _actualScore;
    private int _maxScore;

    private DBmanager _dBmanager;
    
    private Char[] alphabet;
    private int[] alphabetPos;

    public TextMeshProUGUI[] letterPos;

    private string name;

    private int firstMaxScore;
    
    private void Start()
    {
        _dBmanager = GetComponent<DBmanager>();
        infoTxt.text = "";

        gameMode.text = GameManager.GM_instance.ArcadeMode ? "Arcade Mode" : "Story Mode";
        _dBmanager.arcadeMode = GameManager.GM_instance.ArcadeMode;

        _actualScore = 0;
        scoreTxt.text = _actualScore.ToString();

        _maxScore = _dBmanager.GetMaxScoreDB(_actualScore);
        firstMaxScore = _maxScore;
        maxScoreTxt.text = _maxScore.ToString();
        
        alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        alphabetPos = new int[letterPos.Length];

        for (int i = 0; i < alphabetPos.Length; i++)
        {
            alphabetPos[i] = 0;
        }
    }

    private void Update()
    {
        if (_actualScore > _maxScore)
        {
            _maxScore = _dBmanager.GetMaxScoreDB(_actualScore);
            maxScoreTxt.text = _maxScore.ToString();
        }
        if (_actualScore < _maxScore)
        {
            _maxScore = _dBmanager.GetMaxScoreDB(_actualScore);
            maxScoreTxt.text = _maxScore.ToString();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
        }

    }

    public void ChangeLetter(int pos)
    {
        if (alphabetPos[pos-1] >= alphabet.Length - 1)
        {
            alphabetPos[pos-1] = 0;
        }
        else
        {
            alphabetPos[pos-1]++;
        }
        letterPos[pos-1].text = alphabet[alphabetPos[pos-1]].ToString();
    }
    
    public void AddPT()
    {
        _actualScore += 100;
        scoreTxt.text = _actualScore.ToString();
    }

    public void SubstractPT()
    {
        _actualScore -= 100;
        scoreTxt.text = _actualScore.ToString();
    }

    public void ConfirmSelection()
    {
        if (_actualScore > firstMaxScore)
        {
            foreach (var letter in letterPos)
            {
                name += letter.text;
            }

            _dBmanager.arcadeMode = GameManager.GM_instance.ArcadeMode;
            if (_dBmanager != null)
            {
                _dBmanager.ReplaceScoresDB(name, _actualScore);
            }
            else
            {
                Debug.LogWarning("No db manager connected");
            }
            infoTxt.text = "RECORDING...";
            StartCoroutine(FinishRecordingPlayer());
        }
        else
        {
            infoTxt.text = "No superas la puntuación mínima";
        }
    }
    
    IEnumerator FinishRecordingPlayer()
    {
        yield return new WaitForSeconds(0.2f);
        _dBmanager.UpdateIDs();
        yield return new WaitForSeconds(0.1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Home");

    }
}

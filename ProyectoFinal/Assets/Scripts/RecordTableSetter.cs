using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RecordTableSetter : MonoBehaviour
{
    public TextMeshProUGUI infoTxt;
    public GameObject RecordsTableObj;

    private DBmanager _dBmanager;
    
    private void Start()
    {
        _dBmanager = GetComponent<DBmanager>();
        if (_dBmanager != null)
        {
            _dBmanager.arcadeMode = GameManager.GM_instance.ArcadeMode;
            _dBmanager.recordFieldPrefab = RecordsTableObj;
            _dBmanager.ReadTopDb();
        }

        infoTxt.text = GameManager.GM_instance.ArcadeMode ? "Arcade Mode" : "Story Mode";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
        }
    }
}

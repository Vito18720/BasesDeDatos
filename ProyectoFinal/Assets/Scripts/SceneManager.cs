using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public void InsertArcadeScene()
    {
        GameManager.GM_instance.ArcadeMode = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Insertions");
    }
    
    public void InsertStoryScene()
    {
        GameManager.GM_instance.ArcadeMode = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Insertions");
    }

    public void ReadRecordArcadeTableScene()
    {
        GameManager.GM_instance.ArcadeMode = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Records");
    }
    
    public void ReadRecordStoryTableScene()
    {
        GameManager.GM_instance.ArcadeMode = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Records");
    }

    public void ChangeControlsScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Controls");
    }
}

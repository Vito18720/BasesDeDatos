using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

public class InputManagement : MonoBehaviour
{
    public Dictionary<string, KeyCode> keys;
        
    public TextMeshProUGUI Jump, Teleport, Reload, Interact;

    private GameObject currentKey;

    private string filePath;

    private void Awake()
    {
        filePath = $"{Application.dataPath}/InputConfiguration/Inputs.config";
    }

    void Start()
    {
        Debug.Log(filePath);
        if (File.Exists(filePath))
        {
            LoadGame();
            Debug.Log("Load");
        }
        else
        {
            Debug.Log("NotSaveFile");
            keys = new Dictionary<string, KeyCode>()
            {
                {"JumpBind", KeyCode.Space},
                {"TeleportBind", KeyCode.B},
                {"ReloadBind", KeyCode.S},
                {"InteractBind", KeyCode.E}
            };
        }

        Jump.text = keys["JumpBind"].ToString();
        Teleport.text = keys["TeleportBind"].ToString();
        Reload.text = keys["ReloadBind"].ToString();
        Interact.text = keys["InteractBind"].ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
        }
    }

    private void OnGUI()
    {
        if (currentKey != null)
        {
            Debug.Log("GUI");
            foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(kcode))
                {
                    keys[currentKey.name] = kcode;
                    currentKey.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = kcode.ToString();
                    StartCoroutine(SetNullCurrentKey());
                    break;
                }
            }
            
            SaveInputs();
        }
    }

    IEnumerator SetNullCurrentKey()
    {
        yield return new WaitForSeconds(0.1f);
        currentKey = null;
    }

    public void Reasign(GameObject buttonPressed)
    {
        if (currentKey == null)
        {
            StartCoroutine(ChangeKeyWithPauseForReasignation(buttonPressed));
        }
    }

    IEnumerator ChangeKeyWithPauseForReasignation(GameObject buttonPressed)
    {
        yield return new WaitForSeconds(0.2f);
        currentKey = buttonPressed;
    }

    
    public void SaveInputs()
    {
        JObject jSave = new JObject();

        jSave = Serialize();
        
        byte[] encriptedInputs = Encryption(jSave.ToString());
        
        File.WriteAllBytes(filePath, encriptedInputs);
    }

    public void LoadGame()
    {
        byte[] decryption = File.ReadAllBytes(filePath);
        string jString = Desencryption(decryption);
        
        JObject jSaveInputs = JObject.Parse(jString);
        
        Deserialize(jSaveInputs.ToString());
    }

    public JObject Serialize()
    {
        string jsonInputsDictionary = JsonConvert.SerializeObject(keys, Formatting.Indented);
        JObject jInput = JObject.Parse(jsonInputsDictionary);
        return jInput;
    }

    public void Deserialize(string jsonString)
    {
        keys = JsonConvert.DeserializeObject<Dictionary<string, KeyCode>>(jsonString);

        foreach (var k in keys)
        {
            Debug.Log($"{k.Key}: {k.Value}");
        }
    }

    private byte[] _key = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x014, 0x015, 0x016};
    private byte[] _initializationVector = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x014, 0x015, 0x016};
    
    
    byte[] Encryption(string encription)
    {
        AesManaged managed = new AesManaged();

        ICryptoTransform encryptor = managed.CreateEncryptor(_key, _initializationVector);

        MemoryStream memoryStream = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        StreamWriter streamWriter = new StreamWriter(cryptoStream);
        
        streamWriter.WriteLine(encription);
        
        streamWriter.Close();
        memoryStream.Close();
        cryptoStream.Close();

        return memoryStream.ToArray();
    }

    string Desencryption(byte[] encryptedField)
    {
        AesManaged managed = new AesManaged();

        ICryptoTransform encryptor = managed.CreateDecryptor(_key, _initializationVector);
        
        MemoryStream memoryStream = new MemoryStream(encryptedField);
        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Read);
        StreamReader streamReader = new StreamReader(cryptoStream);

        string desencryption = streamReader.ReadToEnd();
        
        streamReader.Close();
        memoryStream.Close();
        cryptoStream.Close();

        return desencryption;
    }
}

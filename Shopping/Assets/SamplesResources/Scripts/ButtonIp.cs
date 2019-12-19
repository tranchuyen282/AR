using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonIp : MonoBehaviour
{

    #region Member
    public InputField ip;
    public Button submitButton;
    public Text text;
    private bool check = false;
    private string textIP="";
    private string oldIP = "";
    
    #endregion Member

    // Start is called before the first frame update
    void Start()
    {
        oldIP = PlayerPrefs.GetString("ip", "");
        if(oldIP.Length > 0)
        {
            text.text = "IP: " + oldIP;
            check = true;
        }
        else
        {
            text.text = "Enter ip";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Method button
    public void setIp()
    {
        if(ip.text.Length > 0)
        {
            textIP = ip.text.Trim();
            if (!textIP.Equals(oldIP)) {
                PlayerPrefs.SetString("ip", textIP);
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.buildIndex + 1);
            }
            else
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.buildIndex + 1);
            }
            
        }
        else
        {
            if(check == true)
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.buildIndex + 1);
            }
            else
            {
                MessageBox.DisplayMessageBox("Note", "Enter ip", true, CloseDialog);
            }
            
            
        }
    }
   

    public void CloseDialog()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        
    }
    #endregion Method button

    #region PRIVATE_METHODS
    public void LoadNextSceneAfter()
    {
       // yield return new WaitForSeconds(1);

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex + 1);
    }
    #endregion //PRIVATE_METHODS
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AndroidUtility;
public class TestCode : MonoBehaviour
{
    public Text LogText;
    public void Click_AndroidToast(string msg)
    {
        AndroidTools.showAsToast(msg);
    }
    public void Click_AndroidToast_LENGTH_LONG(string msg)
    {
        AndroidTools.showAsToast(msg, ToastDuration.LENGTH_LONG);
    }
    public void Click_AndroidLog(string msg)
    {
        AndroidTools.AndroidLog(msg, "my tag I", LogLevel.i);
        AndroidTools.AndroidLog(msg, "my tag v", LogLevel.v);
        AndroidTools.AndroidLog(msg, "my tag d", LogLevel.d);
        AndroidTools.AndroidLog(msg, "my tag w", LogLevel.w);
        AndroidTools.AndroidLog(msg, "my tag E", LogLevel.e);
    }
    public InputField installInput;
    public void Click_Install_Apk()
    {
        AndroidTools.InstallAPP(installInput.text);
    }
    public void Click_GetWIFIState()
    {
        string state = AndroidTools.GetWIFIState();
        LogText.text = state;
        AndroidTools.AndroidLog(state, "GetWIFIState", LogLevel.w);
    }

    public void Click_01_GetAllPkg()
    {
        List<string> list = AndroidTools.GetAllPkg();
        AndroidTools.AndroidLog(list.Count.ToString(), "GetAllPkg", LogLevel.e);
    }
    public void Click_02_OpenlApp()
    {
        AndroidTools.OpenAppByPkg(unstallInput.text);
    }
    public InputField unstallInput;
    public void Click_03_UnstallApp()
    {
        AndroidTools.UnstallApp(unstallInput.text);
    }
    public void Click_04_IsAppRunning()
    {
        bool isRun = AndroidTools.IsAppRunning(unstallInput.text);
        AndroidTools.AndroidLog("Is app Running:" + isRun);
    }
    public void Click_04_IsServiceRunning()
    {
        bool isRun = AndroidTools.IsServiceRunning(unstallInput.text);
        AndroidTools.AndroidLog("Is service Running:" + isRun);
    }
    public void Click_04_SelectPic()
    {
        //先设置jar和AndroidMainfiest
        @"先设置jar和AndroidMainfiest，在SelectPicture\Android有文件".showAsToast();
        return;
        SelectPic p = new SelectPic();
        p.selectPic();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

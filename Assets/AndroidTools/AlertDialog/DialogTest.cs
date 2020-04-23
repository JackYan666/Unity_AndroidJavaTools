using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AndroidUtility;
/// <summary>
/// Discription:DialogTest Powered byMemoryC 
/// Functions: Test AlertDialog
/// CopyRight:MemoryC
/// Time:2017.05.04
/// </>
/// </summary>
/// 
public class DialogTest : MonoBehaviour
{

    public UnityEngine.UI.Button button;

    private AndroidJavaClass UnityPlayer;
    private AndroidJavaObject activity;
    // Use this for initialization
    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(delegate {

                DialogOnClickListener confirmListener = new DialogOnClickListener();
                confirmListener.onClickDelegate = onConfirm;

                DialogOnClickListener cancelListener = new DialogOnClickListener();
                cancelListener.onClickDelegate = onCancel;

                AlertDialog alertDialog = new AlertDialog(AndroidTools.UnityActivity);
                alertDialog.setTitle("标题震惊");
                alertDialog.setMessage("确定要干那事儿?");
                alertDialog.setPositiveButton("当然", confirmListener);
                alertDialog.setNegativeButton("算了", cancelListener);//如果不需要取消后的事件处理，把cancelListener换成new DialogOnClickListener ()就行，上面也不用声明cancelListener
                alertDialog.create();
                alertDialog.show();
            });
        }
    }

    void onConfirm(AndroidJavaObject dialog, int which)
    {
        "哈哈，你摊上事儿了".showAsToast();
    }

    void onCancel(AndroidJavaObject dialog, int which)
    {
        "你啥也没干，回去吧".showAsToast();
    }
}
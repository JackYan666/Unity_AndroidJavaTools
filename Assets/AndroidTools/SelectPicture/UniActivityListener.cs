using UnityEngine;
using System.Collections;

public class UniActivityListener : AndroidJavaProxy
{

    public delegate void ActivityResultDelegate(int requestCode, int resultCode, AndroidJavaObject data);
    public ActivityResultDelegate onActivityResultListener;

    public UniActivityListener() : base("com.unity3d.player.UnityActivityListener")
    {

    }

    public void onActivityResult(int requestCode, int resultCode, AndroidJavaObject data)
    {
        if (onActivityResultListener != null)
        {
            onActivityResultListener(requestCode, resultCode, data);
        }
    }
}
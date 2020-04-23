using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AndroidUtility
{

    /// <summary>
    /// 该功能测试获取到了路径，包两个错误onRestart、onStart
    /// 修改Man activity
    /// 需要引入jar包 实现com.unity3d.player.UnityActivityListener
    /// http://www.manew.com/thread-96609-1-1.html
    /// </summary>
    public class SelectPic
    {
        AndroidJavaClass Intent;
        AndroidJavaClass Uri;

        public void selectPic()
        {
            Intent = new AndroidJavaClass("android.content.Intent");
            Uri = new AndroidJavaClass("android.net.Uri");

            //给当前的activity添加回调监听
            UniActivityListener listener = new UniActivityListener();
            listener.onActivityResultListener = onActivityResult;
            AndroidTools.UnityActivity.Call("setListener", listener);
            //下面开始调用选择图片
            AndroidJavaClass Media = new AndroidJavaClass("android.provider.MediaStore$Images$Media");
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", Intent.GetStatic<AndroidJavaObject>("ACTION_PICK"), Media.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI"));
            intent.Call<AndroidJavaObject>("setDataAndType", Media.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI"), new AndroidJavaObject("java.lang.String", "image/*"));
            AndroidTools.UnityActivity.Call("startActivityForResult", intent, 1);
            //执行到这里之后，就等选择完之后自动执行onActivityResult了
        }

        private void onActivityResult(int requestCode, int resultCode, AndroidJavaObject data)
        {
            if (resultCode != -1)
            {
                Debug.Log("resultCode != RESULT_OK");
                return;
            }
            //requestCode就是你调用currentActivity.Call ("startActivityForResult",intent,1);时的最后面那个常数
            if (requestCode == 1)
            {
                Debug.Log("resultCode == RESULT_OK");
                AndroidJavaObject uri = data.Call<AndroidJavaObject>("getData");
                AndroidJavaClass VERSION = new AndroidJavaClass("android.os.Build$VERSION");
                int version = VERSION.GetStatic<int>("SDK_INT");
                AndroidJavaObject path;
                if (version >= 19)
                {
                    path = handleImageOnKikKat(uri);
                }
                else
                {
                    path = handleImageBeforeKitKat(uri);
                }
                byte[] pathByte = path.Call<byte[]>("getBytes");

                string pathStr = System.Text.Encoding.Default.GetString(pathByte);
                AndroidTools.AndroidLog(pathStr, "pathStr", LogLevel.e);
                //////////////////////////////****让路******////////////////////////////////
                ////                                                                    ////
                ////           pathStr就是你们想要的选择的图片路径了                    ////
                ////                                                                    ////
                //////////////////////////////****让路******////////////////////////////////
            }
        }

        private AndroidJavaObject handleImageOnKikKat(AndroidJavaObject uri)
        {
            AndroidJavaClass DocumentsContract = new AndroidJavaClass("android.provider.DocumentsContract");
            AndroidJavaObject imgPath = null;
            if (DocumentsContract.CallStatic<bool>("isDocumentUri", AndroidTools.UnityActivity, uri))
            {
                AndroidJavaObject docID = DocumentsContract.CallStatic<AndroidJavaObject>("getDocumentId", uri);
                if (uri.Call<AndroidJavaObject>("getAuthority").Call<bool>("equals", new AndroidJavaObject("java.lang.String", "com.android.providers.media.documents")))
                {
                    AndroidJavaObject id = docID.Call<AndroidJavaObject[]>("split", new AndroidJavaObject("java.lang.String", ":"))[1];
                    AndroidJavaObject selection = new AndroidJavaObject("java.lang.String", "_id=").Call<AndroidJavaObject>("concat", id);

                    AndroidJavaClass Media = new AndroidJavaClass("android.provider.MediaStore$Images$Media");
                    imgPath = getImagePath(Media.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI"), selection);
                }
                else if ((new AndroidJavaObject("java.lang.String", "com.android.providers.downloads.documents")).Call<bool>("equals", uri.Call<AndroidJavaObject>("getAuthority")))
                {
                    AndroidJavaClass ContentUris = new AndroidJavaClass("android.content.ContentUris");
                    AndroidJavaClass Long = new AndroidJavaClass("java.lang.Long");
                    AndroidJavaObject contentUri = ContentUris.CallStatic<AndroidJavaObject>("withAppendedId", Uri.CallStatic<AndroidJavaObject>("parse", new AndroidJavaObject("java.lang.String", "content://downloads/public_downloads")), Long.CallStatic<AndroidJavaObject>("valueOf", docID));
                    imgPath = getImagePath(contentUri, null);
                }
            }
            else if ((new AndroidJavaObject("java.lang.String", "content")).Call<bool>("equals", uri.Call<AndroidJavaObject>("getScheme")))
            {
                imgPath = getImagePath(uri, null);
            }
            return imgPath;
        }

        private AndroidJavaObject handleImageBeforeKitKat(AndroidJavaObject uri)
        {
            AndroidJavaObject imagePath = getImagePath(uri, null);
            return imagePath;
        }

        private AndroidJavaObject getImagePath(AndroidJavaObject uri, AndroidJavaObject seletion)
        {
            AndroidJavaObject path = null;
            AndroidJavaObject cursor = AndroidTools.UnityActivity.Call<AndroidJavaObject>("getContentResolver").Call<AndroidJavaObject>("query", uri, null, seletion, null, null);
            if (cursor != null)
            {
                if (cursor.Call<bool>("moveToFirst"))
                {
                    //AndroidJavaClass Media = new AndroidJavaClass("android.provider.MediaStore$Images$Media");
                    path = cursor.Call<AndroidJavaObject>("getString", cursor.Call<int>("getColumnIndex", new AndroidJavaObject("java.lang.String", "_data")));//TODO:
                }
                cursor.Call("close");
            }
            return path;
        }
    }
}
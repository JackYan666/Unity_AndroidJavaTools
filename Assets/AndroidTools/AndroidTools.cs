using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AndroidUtility
{
    #region Enum
    /// <summary>
    /// Toast持续时长
    /// </summary>
    public enum ToastDuration
    {
        LENGTH_SHORT,
        LENGTH_LONG
    }

    /// <summary>
    /// Log 打印级别
    /// </summary>
    public enum LogLevel
    {
        v,
        d,
        i,
        w,
        e
    }

    /// <summary>
    /// wifi 状态
    /// </summary>
    public enum WIFIState
    {
        WIFI_STATE_UNKNOWN = -1,
        WIFI_STATE_DISABLING = 0,
        WIFI_STATE_DISABLED = 1,
        WIFI_STATE_ENABLING = 2,
        WIFI_STATE_ENABLED = 3
    }
    #endregion

    public static class AndroidTools
    {

        #region Unity方法、基础方法

        private static AndroidJavaObject _UnityActivity = null;
        /// <summary>
        /// 获取当前App的Activity
        /// </summary>
        /// <returns></returns>
        public static AndroidJavaObject UnityActivity
        {
            get
            {
                if (_UnityActivity == null)
                {
                    _UnityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                }
                return _UnityActivity;
            }
        }

        private static AndroidJavaObject _UnityAppContext = null;

        /// <summary>
        /// 获取当前App的Context
        /// </summary>
        public static AndroidJavaObject UnityAppContext
        {
            get
            {
                if (_UnityAppContext == null)
                {
                    _UnityAppContext = UnityActivity.Call<AndroidJavaObject>("getApplicationContext");
                }
                return _UnityAppContext;
            }
        }
        private static AndroidJavaObject _PackageManager = null;
        public static AndroidJavaObject PackageManager
        {
            get
            {
                if (_PackageManager == null)
                {
                    _PackageManager = UnityActivity.Call<AndroidJavaObject>("getPackageManager");
                }
                return _PackageManager;
            }
        }


        /// <summary>
        /// 获取当前app包名
        /// </summary>
        /// <returns></returns>
        public static string getPackageName()
        {
            return UnityActivity.Call<string>("getPackageName");
            //讲解：call<返回值类型>("方法名");
        }


        /// <summary>
        /// UI线程中运行
        /// </summary>
        /// <param name="r"></param>
        public static void RunOnUIThread(AndroidJavaRunnable r)
        {
            UnityActivity.Call("runOnUiThread", r);
        }
        /// <summary>
        /// 获取所有安装应用包名和应用名
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllPkg()
        {
            List<string> apks = new List<string>();
            AndroidJavaObject packageInfos = PackageManager.Call<AndroidJavaObject>("getInstalledPackages", 0);
            AndroidJavaObject[] packages = packageInfos.Call<AndroidJavaObject[]>("toArray");
            for (int i = 0; i < packages.Length; i++)
            {
                AndroidJavaObject applicationInfo = packages[i].Get<AndroidJavaObject>("applicationInfo");
                if ((applicationInfo.Get<int>("flags") & applicationInfo.GetStatic<int>("FLAG_SYSTEM")) == 0)// 判断是不是系统应用
                {
                    string packageName = applicationInfo.Get<string>("packageName");
                    AndroidJavaObject applicationLabel = PackageManager.Call<AndroidJavaObject>("getApplicationLabel", applicationInfo);
                    string packageLable = applicationLabel.Call<string>("toString");
                    string p = packageLable + "|" + packageName;
                    AndroidLog(i + ":" + p);
                    apks.Add(p);
                }
            }
            return apks;
        }
        /*public static boolean isRunning(Context context, String packageName) {
                ActivityManager am = (ActivityManager) context.getSystemService(Context.ACTIVITY_SERVICE);
                List<RunningAppProcessInfo> infos = am.getRunningAppProcesses();
                for (RunningAppProcessInfo rapi : infos) {
                    if (rapi.processName.equals(packageName))
                        return true;
                }
                return false;
            }         */
        /// <summary>
        /// 判断指定包名的进程是否运行
        /// </summary>
        /// <returns></returns>
        public static bool IsAppRunning(string pkg)
        {
            AndroidJavaClass Context = new AndroidJavaClass("android.content.Context");
            AndroidJavaObject systemServices = UnityAppContext.Call<AndroidJavaObject>("getSystemService", Context.GetStatic<AndroidJavaObject>("ACTIVITY_SERVICE"));

            AndroidJavaObject runningAppInfos = systemServices.Call<AndroidJavaObject>("getRunningAppProcesses");
            AndroidJavaObject[] runningApps = runningAppInfos.Call<AndroidJavaObject[]>("toArray");
            Debug.Log("running apps Length:"+ runningApps.Length);
            for (int i = 0; i < runningApps.Length; i++)
            {
                AndroidLog(runningApps[i].Get<AndroidJavaObject>("processName").ToCString(), "RunningApp", LogLevel.i);
                AndroidJavaObject appName = runningApps[i].Get<AndroidJavaObject>("processName");
                AndroidLog(appName.ToCString());
                AndroidLog((appName.ToCString() == pkg).ToString());
                if (appName.Call<bool>("equals", pkg.ToJavaString()))
                {
                    return true;
                }
            }
            return false;
        }

        /*
         	public static boolean isServiceRunning(Context mContext, String className) {
		boolean isRunning = false;
		ActivityManager activityManager = (ActivityManager) mContext.getSystemService(Context.ACTIVITY_SERVICE);
		List<ActivityManager.RunningServiceInfo> serviceList = activityManager.getRunningServices(100);
		if (!(serviceList.size() > 0)) {
			return false;
		}
		for (int i = 0; i < serviceList.size(); i++) {
			String name = serviceList.get(i).service.getClassName();
			if (name.equals(className) == true) {
				isRunning = true;
				break;
			}
		}
		return isRunning;
	}             */
        /// <summary>
        /// 判断指定服务是否运行
        /// </summary>
        /// <returns></returns>
        public static bool IsServiceRunning(string serviceName)
        {
            AndroidJavaClass Context = new AndroidJavaClass("android.content.Context");
            AndroidJavaObject systemServices = UnityAppContext.Call<AndroidJavaObject>("getSystemService", Context.GetStatic<AndroidJavaObject>("ACTIVITY_SERVICE"));

            AndroidJavaObject runningServiceInfos = systemServices.Call<AndroidJavaObject>("getRunningServices",100);
            AndroidJavaObject[] runningServices = runningServiceInfos.Call<AndroidJavaObject[]>("toArray");
            Debug.Log("Running service Length:" + runningServices.Length);
            for (int i = 0; i < runningServices.Length; i++)
            {
                AndroidJavaObject info = runningServices[i].Get<AndroidJavaObject>("service");
                AndroidLog(info.ToCString());
                AndroidJavaObject className = info.Call<AndroidJavaObject>("getClassName");
                AndroidLog(className.ToCString());
                if (className.Call<bool>("equals", serviceName.ToJavaString()))
                {
                    return true;
                }
            }
            return false;
        }
        #region Toast Log java与C#string互转
        /// <summary>
        /// this string 表示扩展string的原方法
        /// 两种调用方式：类名静态方法直接调用；"my string".showAsToast();
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="duration"></param>
        public static void showAsToast(this string msg, ToastDuration duration = ToastDuration.LENGTH_SHORT)
        {
#if UNITY_ANDROID
            //new AndroidJavaClass("全类名")  ---new一个Android原生类
            AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
            RunOnUIThread(() =>
            {
                Toast.CallStatic<AndroidJavaObject>("makeText", UnityAppContext, msg.ToJavaString(), (int)duration).Call("show");
            });
#endif
        }


        public static void AndroidLogI(string msg, string tag = "Unity Tag I")
        {
            AndroidJavaClass JavaLog = new AndroidJavaClass("android.util.Log");
            JavaLog.CallStatic<int>("i", tag, msg);
        }
        /// <summary>
        /// 原生安卓Log
        /// </summary>
        /// <param name="msg">打印信息</param>
        /// <param name="tag">Tag</param>
        /// <param name="logEnum">打印级别</param>
        public static void AndroidLog(string msg, string tag = "Unity Tag", LogLevel logEnum = LogLevel.i)
        {
            AndroidJavaClass Log = new AndroidJavaClass("android.util.Log");
            string logLevel = Enum.GetName(typeof(LogLevel), logEnum);
            Log.CallStatic<int>(logLevel, tag, msg);
        }

        /// <summary>
        /// C# string 转换为Java String
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static AndroidJavaObject ToJavaString(this string msg)
        {
            return new AndroidJavaObject("java.lang.String", msg);
        }

        /// <summary>
        /// Java String 转换为C# string
        /// </summary>
        /// <param name="javaString"></param>
        /// <returns></returns>
        public static string ToCString(this AndroidJavaObject javaString)
        {
            byte[] resultByte = javaString.Call<byte[]>("getBytes");
            return System.Text.Encoding.Default.GetString(resultByte);
        }
        #endregion
        #endregion


        #region 功能方法
        //获取指定包名的Activity
        public static AndroidJavaObject GetActivity(string package_name, string activity_name)
        {
            return new AndroidJavaClass(package_name).GetStatic<AndroidJavaObject>(activity_name);
        }

        //获取原生类型： android.provider.Settings中的属性，以android_id为例（只要系统有写就可以）
        public static string GetAndroidID()
        {
            string android_id = "NONE";
            try
            {
                AndroidJavaObject contentResolver = UnityActivity.Call<AndroidJavaObject>("getContentResolver");
                AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
                //CallStatic 的使用：静态方法获取一个Android原生类型 
                android_id = secure.CallStatic<string>("getString", contentResolver, "android_id");
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }

            return android_id;
        }
        //设置 不自动锁屏
        public static void DisableScreenLock()
        {

            UnityActivity.Call<AndroidJavaObject>("getWindow").Call("addFlags", 128);
            //讲解：call("方法名",参数1);
        }

        // 获取内置SD卡路径
        public static string GetStoragePath()
        {
            if (Application.platform == RuntimePlatform.Android)
                return new AndroidJavaClass("android.os.Environment").CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<string>("getPath");
            else
                return "d:/movie";
        }

        /// <summary>
        /// 通过包名打开其他应用
        /// </summary>
        /// <param name="pkgName"></param>
        public static void OpenAppByPkg(string pkgName)
        {
            using (AndroidJavaObject joIntent = PackageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", pkgName))
            {
                if (null != joIntent)
                {
                    UnityAppContext.Call("startActivity", joIntent);
                }
            }
        }


        /// <summary>
        /// 发送带参数广播
        /// </summary>
        /// <param name="actionName">action</param>
        /// <param name="packageName">包名</param>
        /// <param name="broadcastName">广播名</param>
        public static void SendBroadcastWithArgs(string actionName, string packageName, string broadcastName)
        {
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", actionName);
            //设置广播参数
            //intent.Call<AndroidJavaObject>("putExtra", "enable".ToJavaString(), true);
            intent.Call<AndroidJavaObject>("putExtra", "args key1".ToJavaString(), "args value1".ToJavaString());
            intent.Call<AndroidJavaObject>("putExtra", "args key2".ToJavaString(), "args value2".ToJavaString());

            AndroidJavaObject componentNameJO = new AndroidJavaObject("android.content.ComponentName", packageName, broadcastName);
            intent.Call<AndroidJavaObject>("setComponent", componentNameJO);

            UnityAppContext.Call("sendBroadcast", intent);
        }

        /// <summary>
        /// 发送无参数广播
        /// </summary>
        /// <param name="actionName">action</param>
        /// <param name="packageName">包名</param>
        /// <param name="broadcastName">广播名</param>
        public static void SendBroadcast(string actionName, string packageName, string broadcastName)
        {
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", actionName);
            AndroidJavaObject componentNameJO = new AndroidJavaObject("android.content.ComponentName", packageName, broadcastName);
            intent.Call<AndroidJavaObject>("setComponent", componentNameJO);

            UnityAppContext.Call("sendBroadcast", intent);
        }


        //http://www.manew.com/thread-97298-1-1.html
        //path为.apk文件的完整路径
        public static void InstallAPP(string path)
        {
            AndroidJavaClass Intent = new AndroidJavaClass("android.content.Intent");
            AndroidJavaClass Uri = new AndroidJavaClass("android.net.Uri");

            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", Intent.GetStatic<AndroidJavaObject>("ACTION_VIEW"));
            AndroidJavaObject uri = Uri.CallStatic<AndroidJavaObject>("fromFile", new AndroidJavaObject("java.io.File", path.ToJavaString()));
            //apk MIME类型为："application/vnd.android.package-archive"，参考https://blog.csdn.net/boom_jia/article/details/52814914
            intent.Call<AndroidJavaObject>("setDataAndType", uri, "application/vnd.android.package-archive".ToJavaString());
            UnityActivity.Call("startActivity", intent);
        }


        /*添加权限 <permission android:name="android.permission.DELETE_PACKAGES" />
        public static void unstallApp(Context context,String packageName){
            Intent uninstall_intent = new Intent(Intent.ACTION_DELETE);
            uninstall_intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            uninstall_intent.setData(Uri.parse("package:"+packageName));
            context.startActivity(uninstall_intent);}    */
        /// <summary>
        /// 卸载app
        /// </summary>
        /// <param name="packageName"></param>
        public static void UnstallApp(string packageName)
        {
            AndroidJavaClass Intent = new AndroidJavaClass("android.content.Intent");
            AndroidJavaClass Uri = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject intentDelete = new AndroidJavaObject("android.content.Intent", Intent.GetStatic<AndroidJavaObject>("ACTION_DELETE"));
            //intentDelete.Call<AndroidJavaObject>("setFlags", Intent.GetStatic<AndroidJavaObject>("FLAG_ACTIVITY_NEW_TASK"));
            AndroidJavaObject uri = Uri.CallStatic<AndroidJavaObject>("parse", ("package:" + packageName).ToJavaString());
            intentDelete.Call<AndroidJavaObject>("setData", uri);
            UnityActivity.Call("startActivity", intentDelete);
        }


        /// <summary>
        /// 获取WiFi状态
        /// </summary>
        /// <returns></returns>
        public static string GetWIFIState()
        {
            WIFIState state = WIFIState.WIFI_STATE_UNKNOWN;
            AndroidJavaObject wifiManager = UnityActivity.Call<AndroidJavaObject>("getSystemService", "wifi".ToJavaString());
            if (wifiManager != null)
            {
                int wifiState = wifiManager.Call<int>("getWifiState");
                state = (WIFIState)wifiState;
            }
            return state.ToString();
        }





        #endregion
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RobinAndroidMain : MonoBehaviour
{

    public RobinVideoManager video;
    public UDPReceive udp;
    public DebugInfo_benja debugInfo;
    public string name ="[android]";
    public string movieFolderPath = "";
    public bool isDebug = false;
    public bool enableInEditor;

    // Use this for initialization
    void Start()
    {
        bool isEnabled = false;

        #if UNITY_ANDROID
        isEnabled = true;
        #endif

        #if UNITY_STANDALONE
        isEnabled = false;
        enableInEditor = false;
        #endif

        #if UNITY_EDITOR
        isEnabled = enableInEditor;
        #endif
        if (udp == null) udp = FindObjectOfType<UDPReceive>();

        if (!enableInEditor && !isEnabled)
        {
            udp.enabled = false;
            this.enabled = false;
            return;
        }

        Debug.Log(name+"starting android main");
        RobinMainScript main = FindObjectOfType<RobinMainScript>();
        udp.init("", main.port);
        udp.onMessage += onMessage;

        if (!enableInEditor)
        {
            //disable main script and udpsend
            if (main.udp != null) main.udp.enabled = false;
            main.enabled = false;
        }

        if (debugInfo == null) debugInfo = FindObjectOfType<DebugInfo_benja>();
        debugInfo.log("system", name);
        if (video == null) video = FindObjectOfType<RobinVideoManager>();

        AndroidJavaClass jc = new AndroidJavaClass("android.os.Environment");
        string path = jc.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", jc.GetStatic<string>("DIRECTORY_DOWNLOADS")).Call<string>("getAbsolutePath");


        debugInfo.log("data path", "testing " + path);
        if (System.IO.Directory.Exists(path))
        {
            if (System.IO.Directory.Exists(movieFolderPath))
            {
                debugInfo.log("data path", "found " + movieFolderPath);
            }
            if (movieFolderPath != "") movieFolderPath = path + "/" + movieFolderPath;
            else movieFolderPath = path;
            debugInfo.log("data path", "testing " + movieFolderPath);
            if (System.IO.Directory.Exists(movieFolderPath))
            {
                movieFolderPath += "/";
                debugInfo.log("data path","found " + movieFolderPath);
            }
        }
        if (movieFolderPath != "") video.setAssetsPath(movieFolderPath);
        //Reset();
        onDebug(true);
    }

    public string currentMovie;
    public string currentToken;

    public void onMessage(string message)
    {
        debugInfo.log("last message received", message);

        string messageBegin = "token:";
        if (message.Contains(messageBegin))
        {
            onToken(message.Substring(messageBegin.Length, 1));
        }

        messageBegin = "debug:";
        if (message.Contains(messageBegin))
        {
            onDebug(message.Substring(messageBegin.Length, 1) == "1");
        }

        messageBegin = "reset";
        if (message.Contains(messageBegin))
        {
            Reset();
        }

        messageBegin = "forceplay";
        if (message.Contains(messageBegin))
        {
            forceplay = true;
        }

    }

    private bool forceplay = false;

    public void onDebug(bool setDebug)
    {
        isDebug = debugInfo.setDebugState(setDebug);
    }

    public bool onToken(string token)
    {
        Debug.Log(name+"got token " + token);
        currentToken = token;
        debugInfo.log("token", token);
        
        currentMovie = "movie" + token + ".mp4";
        debugInfo.log("movie", "loading "+currentMovie);
        if (video.play(currentMovie,false,true))
        {
            Debug.Log(name+"starting movie " + currentMovie);
            debugInfo.log("movie","loaded "+video.filepath);
            return true;
        }
        debugInfo.log("movie", "error loading " +video.filepath);
        Debug.Log(name+"could not start movie " + currentMovie);
        currentToken += " (error)";
        currentMovie += "(file not found)";
        return false;
    }

    public void Reset()
    {
        Debug.Log(name+"RESETTING android");
        currentMovie = "";
        currentToken = "";
        video.stop(true);

    }


    // Update is called once per frame
    void Update()
    {
        if (forceplay) video.videoPlayer.Play();
        forceplay = false;
    }
}

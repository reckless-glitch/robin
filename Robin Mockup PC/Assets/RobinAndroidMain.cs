using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobinAndroidMain : MonoBehaviour
{

    public RobinVideoManager video;
    public UDPReceive udp;
    public DebugInfo_benja debugInfo;
    public string name ="[android]";
    public bool isDebug = false;
    public bool noDisablingInEditor;

    // Use this for initialization
    void Start()
    {
        bool isEnabled = false;

        #if UNITY_ANDROID
        isEnabled = true;
        #endif

        #if UNITY_STANDALONE
        isEnabled = false;
        noDisablingInEditor = false;
        #endif

        #if UNITY_EDITOR
        isEnabled = true;
        #endif
        if (udp == null) udp = FindObjectOfType<UDPReceive>();

        if (!noDisablingInEditor && !isEnabled)
        {
            udp.enabled = false;
            this.enabled = false;
            return;
        }

        Debug.Log(name+"starting android main");
        RobinMainScript main = FindObjectOfType<RobinMainScript>();
        udp.init("", main.port);
        udp.onMessage += onMessage;

        if (!noDisablingInEditor)
        {
            //disable main script and udpsend
            if (main.udp != null) main.udp.enabled = false;
            main.enabled = false;
        }

        if (video == null) video = FindObjectOfType<RobinVideoManager>();
        if (debugInfo == null) debugInfo = FindObjectOfType<DebugInfo_benja>();
        debugInfo.log("system", name);
        debugInfo.log("moviePath", Application.streamingAssetsPath);

        Reset();
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

    }
    
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
        if (video.play(currentMovie,false,true))
        {
            Debug.Log(name+"starting movie " + currentMovie);
            debugInfo.log("movie",currentMovie);
            return true;
        }
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

    }
}

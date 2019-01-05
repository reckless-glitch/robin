using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobinAndroidMain : MonoBehaviour
{

    public RobinVideoManager video;
    public UDPReceive udp;
    public string name ="[android]";


    // Use this for initialization
    void Start()
    {
        bool isEnabled = false;

        #if UNITY_ANDROID
        isEnabled = true;
        #endif

        #if UNITY_STANDALONE
        isEnabled = false;
        #endif

        #if UNITY_EDITOR
        isEnabled = true;
        #endif

        if (!isEnabled)
        {
            this.enabled = false;
            return;
        }
        Debug.Log(name+"starting android main");
        if (video == null) video = FindObjectOfType<RobinVideoManager>();
        if (udp == null) udp = FindObjectOfType<UDPReceive>();
        udp.onMessage += onMessage;
        Reset();
        onDebug(true);
    }

    public string currentMovie;
    public string currentToken;

    public void onMessage(string message)
    {
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
        udp.createGUI = setDebug;
    }

    public bool onToken(string token)
    {
        Debug.Log(name+"got token " + token);
        currentToken = token;
        currentMovie = "movie" + token + ".mp4";
        if (video.play(currentMovie,false,true))
        {
            Debug.Log(name+"starting movie " + currentMovie);
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

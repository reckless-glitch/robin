using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class RobinMainScript : MonoBehaviour {

    public RobinVideoManager video;
    public ArduinoComRobin_benja arduino;
    public RobinAndroidMain android;
    public UDPSend udp;
    public string name = "[PC]";
    public DebugInfo_benja debugInfo;
    public string localIP;
    public string androIP;
    public int port = 8051;

    // Use this for initialization
    void Start () {

        localIP = LocalIPAddress();
        androIP = localIP.Substring(0, localIP.LastIndexOf('.')) + ".1";
        bool isEnabled = true;
        if (!isEnabled)
        {
            this.enabled = false;
            return;
        }
        Debug.Log(name+"starting windows main");
        if (video == null) video = FindObjectOfType<RobinVideoManager>();
        if (arduino == null) arduino = FindObjectOfType<ArduinoComRobin_benja>();
        if (android == null) android = FindObjectOfType<RobinAndroidMain>();
        if (debugInfo == null) debugInfo = FindObjectOfType<DebugInfo_benja>();
        debugInfo.log("system", name);
        if (udp == null) udp = FindObjectOfType<UDPSend>();
        udp.init(androIP,port); //set IP and Port here
        arduino.onToken = onToken;
        Reset();
        setDebug(true);
        debugInfo.log("moviePath",Application.streamingAssetsPath);
        debugInfo.log("key q", "toggle debug (this) on/off ");
        debugInfo.log("key r", "reset all devices");
        debugInfo.log("keys a,b,c,d,e,f", "simulate token sent");
        debugInfo.log("keys g", "simulate not existing token sent");
        debugInfo.log("keys alt+F4", "simulate not existing token sent"); 
    }

    public string currentMovie;
    public string currentToken;
    public float scanningTimeSec = 5;
    public bool onToken(string token)
    {
        Reset();
        Debug.Log(name+"ontoken " + token);
        debugInfo.log("token", token);
        currentToken = token;
        currentMovie = "movie" + token + ".mp4";
        udp.sendString("token:"+token);
        if(android.enabled)
        {
            android.onToken(token);
            return true;
        }
        if (video.play(currentMovie,false,true))
        {
            Debug.Log(name+"starting movie " + currentMovie);
            debugInfo.log("current Movie", currentMovie);
            arduino.scan(scanningTimeSec);
            return true;
        }
        Debug.Log(name+"could not start movie " + currentMovie);
        debugInfo.log("current Movie", "not found "+ currentMovie);
        currentToken += " (error)";
        currentMovie += "(file not found)";
        return false;
    }

    public static string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "0.0.0.0";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }

    public void Reset()
    {
        Debug.Log(name+"RESETTING");
        arduino.Reset();
        currentMovie = "";
        currentToken = "";
        video.stop();
        udp.sendString("reset");
    }

    public bool isDebug = false;

    public void toggleDebug()
    {
        setDebug(!isDebug); 

    }

    public void setDebug(bool setDebug)
    {
        isDebug = debugInfo.setDebugState(setDebug);
        udp.sendString("debug:" + (isDebug ? "1" : "0"));
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("r")) Reset();
        if (Input.GetKeyDown("q")) toggleDebug();

        if (Input.GetKeyDown("a")) onToken("A");
        if (Input.GetKeyDown("b")) onToken("B");
        if (Input.GetKeyDown("c")) onToken("C");
        if (Input.GetKeyDown("d")) onToken("D");
        if (Input.GetKeyDown("e")) onToken("E");
        if (Input.GetKeyDown("f")) onToken("F");
        if (Input.GetKeyDown("g")) onToken("ä");//should provide an error which is handled
        if (Input.GetKeyDown("p")) { udp.sendString("forceplay"); video.videoPlayer.Play(); }
    }
}

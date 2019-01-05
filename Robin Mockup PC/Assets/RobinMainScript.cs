using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobinMainScript : MonoBehaviour {

    public RobinVideoManager video;
    public ArduinoComRobin_benja arduino;
    public UDPSend udp;
    public string name = "[PC]";


    // Use this for initialization
    void Start () {
        bool isEnabled = true;
        #if UNITY_ANDROID
        isEnabled = false;
        #endif
       
        #if UNITY_EDITOR
        isEnabled = true;
        #endif
        
        #if UNITY_STANDALONE
        isEnabled = true;
        #endif

        if (!isEnabled)
        {
            this.enabled = false;
            return;
        }
        Debug.Log(name+"starting windows main");
        if (video == null) video = FindObjectOfType<RobinVideoManager>();
        if (arduino == null) arduino = FindObjectOfType<ArduinoComRobin_benja>();
        if (udp == null) udp = FindObjectOfType<UDPSend>();
        udp.init(); //set IP and Port here
        arduino.onToken = onToken;
        Reset();
        setDebug(true);
    }

    public string currentMovie;
    public string currentToken;
    public float scanningTimeSec = 5;
    public bool onToken(string token)
    {
        Debug.Log(name+"ontoken " + token);
        currentToken = token;
        currentMovie = "movie" + token + ".mp4";
        udp.sendString("token:"+token);
        if (video.play(currentMovie))
        {
            Debug.Log(name+"starting movie " + currentMovie);
            arduino.scan(scanningTimeSec);
            return true;
        }
        Debug.Log(name+"could not start movie " + currentMovie);
        currentToken += " (error)";
        currentMovie += "(file not found)";
        return false;
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
        isDebug = setDebug;
        udp.createGUI = isDebug;
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
    }
}

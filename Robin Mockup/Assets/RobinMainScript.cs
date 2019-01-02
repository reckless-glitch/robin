using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobinMainScript : MonoBehaviour {

    public RobinVideoManager video;
    public ArduinoComRobin_benja arduino;

	// Use this for initialization
	void Start () {
        if (video == null) video = FindObjectOfType<RobinVideoManager>();
        if (arduino == null) arduino = FindObjectOfType<ArduinoComRobin_benja>();
        arduino.onToken = onToken;
        Reset();
    }

    public string currentMovie;
    public string currentToken;
    public float scanningTimeSec = 5;
    public bool onToken(string token)
    {
        currentToken = token;
        currentMovie = "movie" + token + ".mp4";
        if (video.play(currentMovie))
        {
            Debug.Log("starting movie " + currentMovie);
            arduino.scan(scanningTimeSec);
            return true;
        }
        Debug.Log("could not start movie " + currentMovie);
        currentToken += " (error)";
        currentMovie += "(file not found)";
        return false;
    }

    public void Reset()
    {
        Debug.Log("RESETTING");
        arduino.Reset();
        currentMovie = "";
        currentToken = "";
        video.stop();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("r"))
        {
           Reset();
        }
    }
}

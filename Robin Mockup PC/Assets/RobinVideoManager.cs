using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class RobinVideoManager : MonoBehaviour {

    public VideoPlayer videoPlayer;
    public string[] movieFiles;
    public string assetsPath;
    public string filepath = "";

	// Use this for initialization
	void Start () {
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.source = VideoSource.Url;
        assetsPath = Application.streamingAssetsPath + "\\";
	}

    private bool setPlay = false;
    private bool setLoop;

    public bool play(string filename = "",bool loop = false, bool viaUpdate = false)
    {

        if (filename != "") filepath = assetsPath + filename; 
        //trick to not reset the filepath if it has been set before and is rerun now
        if (!System.IO.File.Exists(filepath)) return false;

        if (viaUpdate)
        {
            //this is probably not the main loop so set this up to be rerun via update()
            setLoop = loop;
            setPlay = true;
            return true;
        }
        else if (setPlay)
        {
            //this has run before but needed to be rerun via update()
            //this makes sure we are in the main loop now
            loop = setLoop;
            setPlay = false;
        }

        videoPlayer.isLooping = loop;
        videoPlayer.url = filepath;
        videoPlayer.Play();
        return true;
    }

    private bool setstop = false;

    public void stop(bool viaUpdate = false)
    {
        if(viaUpdate)
        {
            setstop = true;
            return;
        }
        setstop = false;
        if (videoPlayer != null)
        {
            try
            {
                videoPlayer.Stop();
            }
            catch (System.Exception e)
            {
                setstop = !setstop;
            }
        }
    }

	// Update is called once per frame
	void Update () {
        if (setstop) stop();
        if (setPlay) play();

    }
}

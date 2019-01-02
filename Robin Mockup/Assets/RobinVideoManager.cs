using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class RobinVideoManager : MonoBehaviour {

    public VideoPlayer videoPlayer;
    public string[] movieFiles;
    public string filepath = "";

	// Use this for initialization
	void Start () {
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.source = VideoSource.Url;
        filepath = Application.streamingAssetsPath;
	}
	
    public bool play(string filename,bool loop = false)
    {
        filepath = Application.streamingAssetsPath + "\\" + filename;
        if (!UnityEngine.Windows.File.Exists(filepath))
            return false;
        videoPlayer.isLooping = loop;
        videoPlayer.url = filepath;
        videoPlayer.Play();
        return true;
    }

    public void stop()
    {
        if(videoPlayer!= null)
        videoPlayer.Stop();
    }

	// Update is called once per frame
	void Update () {
		
	}
}

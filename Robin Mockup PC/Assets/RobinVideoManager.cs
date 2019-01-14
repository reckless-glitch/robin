using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class RobinVideoManager : MonoBehaviour {

    public VideoPlayer videoPlayer;
    public DebugInfo_benja debugInfo;
    public string[] movieFiles;
    public string assetsPath ="";
    public string filepath = "";
    public bool doThePrepareThing = true;
    public VideoClip videoclip;

	// Use this for initialization
	void Start () {
        if(videoPlayer == null) videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
        if (assetsPath == "") assetsPath = Application.streamingAssetsPath + "/";
        if (debugInfo == null) debugInfo = FindObjectOfType<DebugInfo_benja>();
        debugInfo.log("[movie] filepath", filepath);
        debugInfo.log("[movie] filetest", "");
        debugInfo.log("[movie] status", "setup done");
        debugInfo.log("[movie]", "");

        
    }

    public bool setAssetsPath(string path)
    {
        if (System.IO.Directory.Exists(path))
        {
            assetsPath = path;
            return true;
        }
        return false;
    }

    private bool setPlay = false;
    private bool setLoop;

    public bool play(VideoClip clip, bool loop = false, bool viaUpdate = false)
    {
        if (clip == null) return false;
        videoPlayer.source = VideoSource.VideoClip;
        videoclip = clip;
        debugInfo.log("[movie] filetest", "exists - is internal clip");
        return play(loop, viaUpdate);
    }

    public bool play(string filename, bool loop = false, bool viaUpdate = false)
    {
        videoPlayer.source = VideoSource.Url;
        if (filename != "") filepath = assetsPath + filename;
        //trick to not reset the filepath if it has been set before and is rerun now
        debugInfo.log("[movie] filetest", "testing");
        if (!System.IO.File.Exists(filepath)) return false;
        debugInfo.log("[movie] filetest", "exists");
        return play(loop, viaUpdate);
    }

    public bool play(bool loop = false, bool viaUpdate = false)
    {
        if (viaUpdate)
        {
            //this is probably not the main loop so set this up to be rerun via update()
            setLoop = loop;
            setPlay = true;
            Debug.Log("play via update enabled");
            return true;
        }
        else if (setPlay)
        {
            //this has run before but needed to be rerun via update()
            //this makes sure we are in the main loop now
            loop = setLoop;
            setPlay = false;
            Debug.Log("play via update second run");
        }
        Debug.Log("play running");
        if (videoPlayer.source == VideoSource.Url)
        {
            debugInfo.log("[movie] filepath", filepath);
            videoPlayer.url = filepath;
        }
        else
        {
            debugInfo.log("[movie] filepath", "internal clip: ");
            videoPlayer.clip = videoclip;
        }

        /*
            if(doThePrepareThing)
            {
                videoPlayer.Prepare();
                //Wait until video is prepared
                debugInfo.log("[movie] status", "preparing");

                bool giveUp = false;
                float time = 5;
                float timestep = 0.2f;
                while (!videoPlayer.isPrepared && !giveUp)
                {
                    time -= timestep;
                    if (time < 0) giveUp = true;
                    yield return new WaitForSeconds(timestep);
                }
                if(giveUp) debugInfo.log("[movie] status", "not prepared");
            
    }
        */
        videoPlayer.isLooping = loop;
        try
        {
            debugInfo.log("[movie] status", "called play");
            videoPlayer.Play();
            debugInfo.log("[movie] status", "called play - ok");
            return true;
        }
        catch (System.Exception e)
        {
            debugInfo.log("[movie] status", "called play - failed");
        }
        return false;

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
                debugInfo.log("[movie] status", "called stop");
            }
            catch (System.Exception e)
            {
                setstop = !setstop;
            }
        }
    }

	// Update is called once per frame
	void Update () {
        debugInfo.log("[movie]", (videoPlayer.isPlaying ? "[play] " : "[stop] ") + videoPlayer.time.ToString("N2") + " s " + (videoPlayer.isLooping ? "[loop] " : "") + videoPlayer.url);

        if (setstop) stop();
        if (setPlay) play();

    }
}

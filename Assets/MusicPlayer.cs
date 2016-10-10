using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

    public static MusicPlayer musicPlayer = null;

    AudioSource music;

	// Use this for initialization
	void Start () {
        if(musicPlayer != null)
        {
            Destroy(gameObject);
        }
        else
        {
            musicPlayer = this;
            GameObject.DontDestroyOnLoad(gameObject);
            music = GetComponent<AudioSource>();
        }

        
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (music.isPlaying)
            {
                music.Stop();
            }
            else
            {
                music.Play();
            }
        }
        
	}
}

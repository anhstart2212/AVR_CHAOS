using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200001E RID: 30
[RequireComponent(typeof(AudioSource))]
public class MenuSoundController : MonoBehaviour
{
    // Token: 0x060000B7 RID: 183 RVA: 0x000072D0 File Offset: 0x000054D0
    private void Awake()
    {
        this.aud = base.GetComponent<AudioSource>();
        this.aud.volume = 0.1f;
        this.InitializeClips();
    }

    // Token: 0x060000B8 RID: 184 RVA: 0x000072F4 File Offset: 0x000054F4
    private void InitializeClips()
    {
        this.clipDict.Clear();
        this.clipDict.Add("back", Resources.Load<AudioClip>("Sounds/Menu/UI_button_click_back"));
        this.clipDict.Add("click", Resources.Load<AudioClip>("Sounds/Menu/UI_button_click"));
    }

    // Token: 0x060000B9 RID: 185 RVA: 0x00007340 File Offset: 0x00005540
    public void PlaySound(string type)
    {
        if (this.clipDict.ContainsKey(type))
        {
            if (this.aud.isPlaying)
            {
                this.aud.Stop();
            }
            this.aud.clip = this.clipDict[type];
            this.aud.Play();
        }
    }

    // Token: 0x040000DA RID: 218
    private AudioSource aud;

    // Token: 0x040000DB RID: 219
    private Dictionary<string, AudioClip> clipDict = new Dictionary<string, AudioClip>();
}

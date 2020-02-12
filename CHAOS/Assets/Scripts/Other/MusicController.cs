using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200009D RID: 157
public class MusicController : MonoBehaviour
{
    // Token: 0x06000572 RID: 1394 RVA: 0x0002DCD8 File Offset: 0x0002BED8
    private void Start()
    {
        this.music = base.GetComponent<AudioSource>();
        this.randomTrack = UnityEngine.Random.Range(0, this.tracks.Length);
        this.played = new bool[this.tracks.Length];
        base.StartCoroutine(this.MusicBox());
    }

    // Token: 0x06000573 RID: 1395 RVA: 0x0002DD28 File Offset: 0x0002BF28
    private IEnumerator MusicBox()
    {
        for (; ; )
        {
            if (!this.musicActiveFlag)
            {
                if (this.music.isPlaying)
                {
                    this.music.Pause();
                }
                yield return new WaitForEndOfFrame();
            }
            else
            {
                if (!this.music.isPlaying)
                {
                    this.music.UnPause();
                }
                if (this.songCount >= this.tracks.Length - 1)
                {
                    for (int i = 0; i < this.played.Length; i++)
                    {
                        this.played[i] = false;
                    }
                    this.songCount = 0;
                }
                if (!this.music.isPlaying)
                {
                    yield return new WaitForSeconds(this.trackDelay);
                    this.randomTrack = UnityEngine.Random.Range(0, this.tracks.Length);
                    this.SongPicker(this.randomTrack);
                }
                yield return new WaitForEndOfFrame();
            }
        }
        yield break;
    }

    // Token: 0x06000574 RID: 1396 RVA: 0x0002DD43 File Offset: 0x0002BF43
    public void ChangeSong()
    {
        if (!this.musicActiveFlag)
        {
            this.musicActiveFlag = !this.musicActiveFlag;
        }
        this.music.Stop();
    }

    // Token: 0x06000575 RID: 1397 RVA: 0x0002DD6C File Offset: 0x0002BF6C
    private void SongPicker(int i)
    {
        if (!this.played[i])
        {
            this.music.clip = this.tracks[i];
            this.music.volume = this.trackVolumes[i];
            this.music.Play();
            this.played[i] = true;
            this.songCount++;
        }
    }

    // Token: 0x040005EA RID: 1514
    public float trackDelay;

    // Token: 0x040005EB RID: 1515
    public AudioClip[] tracks;

    // Token: 0x040005EC RID: 1516
    public float[] trackVolumes;

    // Token: 0x040005ED RID: 1517
    private int randomTrack;

    // Token: 0x040005EE RID: 1518
    private int songCount;

    // Token: 0x040005EF RID: 1519
    private bool[] played;

    // Token: 0x040005F0 RID: 1520
    private AudioSource music;

    // Token: 0x040005F1 RID: 1521
    public bool musicActiveFlag = true;
}

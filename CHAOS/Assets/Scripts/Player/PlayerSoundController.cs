using Gamekit3D;
using System;
using UnityEngine;

// Token: 0x0200003F RID: 63
public class PlayerSoundController : Bolt.EntityBehaviour<IChaos_PlayerState>
{
    // Token: 0x06000261 RID: 609 RVA: 0x00016538 File Offset: 0x00014738
    private void Start()
    {
        this.player = base.GetComponent<Player>();
        this.cableFire = this.hookSounds[0];
        this.cableReel = this.hookSounds[1];
        this.cableLoop = this.hookSounds[2];
        this.cableLock = this.hookSounds[3];
        this.gasBurst = this.gasSounds[0];
        this.gasLoop = this.gasSounds[1];
        this.swordEject = this.swordSounds[0];
        this.swordDraw = this.swordSounds[1];
        this.windLoop = this.ambientSounds[0];
        this.swordSlash = this.swordSounds[2];
        this.swordMisses[0] = this.swordSounds[3];
        this.swordMisses[1] = this.swordSounds[4];
        this.swordMisses[2] = this.swordSounds[5];
        this.swordEscape = this.swordSounds[6];
        this.srcLeftFoot = this.sources[0];
        this.srcRightFoot = this.sources[1];
        this.srcGas = this.sources[2];
        this.srcSword = this.sources[3];
        this.srcLeftHook = this.sources[4];
        this.srcRightHook = this.sources[5];
        this.srcCableLoop = this.sources[6];
        this.srcWindLoop = this.sources[7];
        this.srcSlide = this.sources[8];
        this.srcLeftFoot.volume = this.volume.footStep;
        this.srcRightFoot.volume = this.volume.footStep;
        this.srcLeftHook.volume = this.volume.hookFire;
        this.srcRightHook.volume = this.volume.hookFire;
        this.srcCableLoop.volume = this.volume.cableReel;
        this.srcSword.volume = this.volume.sword;
        this.srcWindLoop.volume = this.volume.speedWind;
        this.srcSlide.volume = this.volume.slide;
        this.srcCableLoop.loop = true;
        this.srcCableLoop.clip = this.cableLoop;
        this.srcWindLoop.loop = true;
        this.srcWindLoop.clip = this.windLoop;
    }

    // Token: 0x06000262 RID: 610 RVA: 0x00016784 File Offset: 0x00014984
    private void Update()
    {
        this.cancelCableLoop = (this.player.ReeledState != 0 || state.IsGrounded);
        this.gasSmallEmitting = this.player.particles.gasCore.isPlaying;
        this.SpeedReelAndGasSettings();
        if (this.player.IsEitherHooked && this.srcGas.clip != this.gasBurst)
        {
            if (state.IsJumpReelKey)
            {
                if (!this.cancelCableLoop)
                {
                    if (!this.srcCableLoop.isPlaying)
                    {
                        this.srcCableLoop.Play();
                    }
                }
                else
                {
                    this.srcCableLoop.Stop();
                }
            }
            else
            {
                this.srcCableLoop.Stop();
                this.srcGas.Stop();
            }
            if (this.player.IsDoubleHooked)
            {
                this.srcGas.volume = this.volume.gasLarge;
            }
            else
            {
                this.srcGas.volume = this.volume.gasSmall;
            }
        }
        else
        {
            this.srcCableLoop.Stop();
            if (!(this.srcGas.clip == this.gasBurst) || !this.srcGas.isPlaying)
            {
                if ((this.gasSmallEmitting || (this.player.BurstTankLevel > 0f && Input.GetAxisRaw(InputAxisNames.verticalBurst) != 0f)) && !state.IsGrounded)
                {
                    this.srcGas.volume = this.volume.gasSmall;
                    this.srcGas.clip = this.gasLoop;
                    this.srcGas.loop = true;
                    if (!this.srcGas.isPlaying)
                    {
                        this.srcGas.Play();
                    }
                }
                else
                {
                    this.srcGas.Stop();
                }
            }
        }
        if (this.leftFire)
        {
            this.GearSoundPlayer(0, true);
            this.leftFire = false;
        }
        else if (this.leftRetract)
        {
            this.GearSoundPlayer(1, true);
            this.leftRetract = false;
        }
        if (this.rightFire)
        {
            this.GearSoundPlayer(0, false);
            this.rightFire = false;
        }
        else if (this.rightRetract)
        {
            this.GearSoundPlayer(1, false);
            this.rightRetract = false;
        }
        if (this.player.BurstForceIsRunning)
        {
            if (!this.burstSoundCheck)
            {
                this.GearSoundPlayer(3, false);
                this.burstSoundCheck = true;
            }
        }
        else
        {
            this.burstSoundCheck = false;
        }
        if (!this.SpeedWindSettings())
        {
            if (this.srcWindLoop.isPlaying)
            {
                this.srcWindLoop.Stop();
            }
        }
        else if (!this.srcWindLoop.isPlaying)
        {
            this.srcWindLoop.Play();
        }
    }

    // Token: 0x06000263 RID: 611 RVA: 0x00016A84 File Offset: 0x00014C84
    public void FootStep(string foot)
    {
        AudioSource audioSource;
        if (foot == "l")
        {
            audioSource = this.srcLeftFoot;
        }
        else
        {
            audioSource = this.srcRightFoot;
        }
        audioSource.clip = this.footSteps[UnityEngine.Random.Range(0, this.footSteps.Length - 1)];
        audioSource.Play();
    }

    // Token: 0x06000264 RID: 612 RVA: 0x00016AD8 File Offset: 0x00014CD8
    public void SwordSound(string type)
    {
        this.srcSword.pitch = 1f;
        if (type == "eject")
        {
            this.srcSword.pitch = 1.1f;
            this.srcSword.clip = this.swordEject;
        }
        else if (type == "draw")
        {
            this.srcSword.pitch = 1.25f;
            this.srcSword.clip = this.swordDraw;
        }
        else if (type == "hit")
        {
            this.srcSword.pitch = UnityEngine.Random.Range(0.925f, 1.125f);
            this.srcSword.clip = this.swordSlash;
        }
        else if (type == "escape")
        {
            this.srcSword.pitch = UnityEngine.Random.Range(1.05f, 1.15f);
            this.srcSword.clip = this.swordEscape;
        }
        else
        {
            if (type == "stop")
            {
                if (this.srcSword.isPlaying)
                {
                    this.srcSword.Stop();
                }
                return;
            }
            this.srcSword.pitch = UnityEngine.Random.Range(0.95f, 1.1f);
            this.srcSword.clip = this.swordMisses[UnityEngine.Random.Range(0, this.swordMisses.Length)];
        }
        this.srcSword.Play();
    }

    // Token: 0x06000265 RID: 613 RVA: 0x00016C54 File Offset: 0x00014E54
    private void GearSoundPlayer(int type, bool isLeft)
    {
        if (isLeft)
        {
            this.srcLeftHook.pitch = UnityEngine.Random.Range(0.95f, 1.2f);
        }
        else
        {
            this.srcRightHook.pitch = UnityEngine.Random.Range(0.95f, 1.2f);
        }
        if (type == 0)
        {
            this.GearAudioSourceSetter(isLeft, this.cableFire, false);
        }
        else if (type == 1)
        {
            this.GearAudioSourceSetter(isLeft, this.cableReel, false);
        }
        else if (type == 2)
        {
            this.GearAudioSourceSetter(isLeft, this.cableLock, false);
        }
        else if (type == 3)
        {
            this.GearAudioSourceSetter(isLeft, this.gasBurst, true);
        }
        else if (type == 4)
        {
            this.GearAudioSourceSetter(isLeft, this.gasBurst, true);
        }
    }

    // Token: 0x06000266 RID: 614 RVA: 0x00016D1C File Offset: 0x00014F1C
    private void GearAudioSourceSetter(bool left, AudioClip clip, bool isGas)
    {
        AudioSource audioSource;
        if (isGas)
        {
            audioSource = this.srcGas;
            audioSource.loop = false;
            audioSource.volume = this.volume.gasBurst;
            audioSource.pitch = UnityEngine.Random.Range(0.8f, 1f);
        }
        else
        {
            audioSource = this.GearSoundOverride(left, clip);
        }
        audioSource.clip = clip;
        audioSource.Play();
    }

    // Token: 0x06000267 RID: 615 RVA: 0x00016D80 File Offset: 0x00014F80
    private AudioSource GearSoundOverride(bool left, AudioClip clip)
    {
        if (left)
        {
            if (this.srcRightHook.isPlaying && this.srcRightHook.clip == clip)
            {
                this.srcRightHook.Stop();
            }
            return this.srcLeftHook;
        }
        if (this.srcLeftHook.isPlaying && this.srcLeftHook.clip == clip)
        {
            this.srcLeftHook.Stop();
        }
        return this.srcRightHook;
    }

    // Token: 0x06000268 RID: 616 RVA: 0x00016E04 File Offset: 0x00015004
    private bool HookSoundVolumeDropOff()
    {
        float num = this.player.limit.camMaxFollowDist * 0.5f;
        float magnitude = (base.transform.position - this.player.transforms.playerCamera.position).magnitude;
        float num2 = (!this.player.IsDoubleHooked) ? this.volume.gasSmall : this.volume.gasLarge;
        if (magnitude <= num)
        {
            this.HookSoundVolumeDropOffAssigner(this.volume.hookFire, this.volume.cableReel, num2);
            return false;
        }
        float num3 = 1f - (magnitude - num) / this.player.limit.camMaxFollowDist;
        this.HookSoundVolumeDropOffAssigner(this.volume.hookFire * num3, this.volume.cableReel * num3, num2 * num3);
        return true;
    }

    // Token: 0x06000269 RID: 617 RVA: 0x00016EEB File Offset: 0x000150EB
    private void HookSoundVolumeDropOffAssigner(float hookVol, float cableVol, float gasVol)
    {
        this.srcLeftHook.volume = hookVol;
        this.srcRightHook.volume = hookVol;
        this.srcCableLoop.volume = cableVol;
        this.srcGas.volume = gasVol;
    }

    // Token: 0x0600026A RID: 618 RVA: 0x00016F20 File Offset: 0x00015120
    private bool SpeedReelAndGasSettings()
    {
        float velocityMagnitude = this.player.VelocityMagnitude;
        if (velocityMagnitude <= 33f)
        {
            this.srcCableLoop.pitch = 0.9f;
            this.srcGas.pitch = 0.8f;
            return false;
        }
        float num = (velocityMagnitude - 33f) / (this.player.limit.maxHorizontal - 33f);
        this.srcCableLoop.pitch = 0.9f + num * 0.399999976f;
        this.srcGas.pitch = 0.8f + num * 0.199999988f;
        return true;
    }

    // Token: 0x0600026B RID: 619 RVA: 0x00016FB8 File Offset: 0x000151B8
    private bool SpeedWindSettings()
    {
        float num = this.player.limit.maxHorizontal / 3f;
        float velocityMagnitude = this.player.VelocityMagnitude;
        if (velocityMagnitude <= num)
        {
            this.srcWindLoop.pitch = 1.75f;
            return false;
        }
        float num2 = (velocityMagnitude - num) / (this.player.limit.maxHorizontal - num);
        this.srcWindLoop.volume = num2 * this.volume.speedWind;
        this.srcWindLoop.pitch = 1.75f + num2 * 0.5f;
        return true;
    }

    // Token: 0x04000284 RID: 644
    public AudioClip[] footSteps;

    // Token: 0x04000285 RID: 645
    public AudioClip[] hookSounds;

    // Token: 0x04000286 RID: 646
    public AudioClip[] gasSounds;

    // Token: 0x04000287 RID: 647
    public AudioClip[] swordSounds;

    // Token: 0x04000288 RID: 648
    public AudioClip[] ambientSounds;

    // Token: 0x04000289 RID: 649
    public AudioClip[] slideSounds;

    // Token: 0x0400028A RID: 650
    private AudioClip cableFire;

    // Token: 0x0400028B RID: 651
    private AudioClip cableReel;

    // Token: 0x0400028C RID: 652
    private AudioClip cableLoop;

    // Token: 0x0400028D RID: 653
    private AudioClip cableLock;

    // Token: 0x0400028E RID: 654
    private AudioClip gasBurst;

    // Token: 0x0400028F RID: 655
    private AudioClip gasLoop;

    // Token: 0x04000290 RID: 656
    private AudioClip swordEject;

    // Token: 0x04000291 RID: 657
    private AudioClip swordDraw;

    // Token: 0x04000292 RID: 658
    private AudioClip swordSlash;

    // Token: 0x04000293 RID: 659
    private AudioClip swordEscape;

    // Token: 0x04000294 RID: 660
    private AudioClip[] swordMisses = new AudioClip[3];

    // Token: 0x04000295 RID: 661
    private AudioClip windLoop;

    // Token: 0x04000296 RID: 662
    public AudioSource[] sources;

    // Token: 0x04000297 RID: 663
    private AudioSource srcLeftFoot;

    // Token: 0x04000298 RID: 664
    private AudioSource srcRightFoot;

    // Token: 0x04000299 RID: 665
    private AudioSource srcLeftHook;

    // Token: 0x0400029A RID: 666
    private AudioSource srcRightHook;

    // Token: 0x0400029B RID: 667
    private AudioSource srcCableLoop;

    // Token: 0x0400029C RID: 668
    private AudioSource srcWindLoop;

    // Token: 0x0400029D RID: 669
    private AudioSource srcGas;

    // Token: 0x0400029E RID: 670
    private AudioSource srcSword;

    // Token: 0x0400029F RID: 671
    private AudioSource srcSlide;

    // Token: 0x040002A0 RID: 672
    public PlayerVolumes volume;

    // Token: 0x040002A1 RID: 673
    private Player player;

    // Token: 0x040002A2 RID: 674
    [HideInInspector]
    public bool leftFire;

    // Token: 0x040002A3 RID: 675
    [HideInInspector]
    public bool rightFire;

    // Token: 0x040002A4 RID: 676
    [HideInInspector]
    public bool leftRetract;

    // Token: 0x040002A5 RID: 677
    [HideInInspector]
    public bool rightRetract;

    // Token: 0x040002A6 RID: 678
    private bool cancelCableLoop;

    // Token: 0x040002A7 RID: 679
    private bool gasSmallEmitting;

    // Token: 0x040002A8 RID: 680
    private bool burstSoundCheck;
}

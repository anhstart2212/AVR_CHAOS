using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200004A RID: 74
public class PlayerTargetManager : MonoBehaviour
{
    // Token: 0x060002BB RID: 699 RVA: 0x0001A742 File Offset: 0x00018942
    private void Start()
    {
        //this.main = base.GetComponent<TitanMain>();
    }

    // Token: 0x060002BC RID: 700 RVA: 0x0001A750 File Offset: 0x00018950
    private void Update()
    {
        this.UpdateNearbyTargets();
    }

    // Token: 0x060002BD RID: 701 RVA: 0x0001A758 File Offset: 0x00018958
    private void UpdateNearbyTargets()
    {
        //if (PlayerTarget.IsValid(this.main.ActiveTarget))
        //{
        //    if (this.main.PathIsBlocked)
        //    {
        //        if (!this.blockClockRunning)
        //        {
        //            this.StartBlockClock();
        //        }
        //    }
        //    else
        //    {
        //        this.StopBlockClock();
        //    }
        //    if (this.main.PlayerIsHidden)
        //    {
        //        if (!this.sightClockRunning)
        //        {
        //            this.StartSightClock();
        //        }
        //    }
        //    else
        //    {
        //        this.StopSightClock();
        //    }
        //    if (this.timeSinceHidden >= 1f || this.timeSinceBlocked >= 1f)
        //    {
        //        this.SetAggro(this.main.ActiveTarget, 0f);
        //        this.timeSinceHidden = 0f;
        //        this.timeSinceBlocked = 0f;
        //        this.resetCooloff = 1f;
        //    }
        //}
        //else
        //{
        //    this.StopBlockClock();
        //    this.StopSightClock();
        //}
        //foreach (PlayerTarget playerTarget in this.main.PlayerTrackingScript.GetNearList())
        //{
        //    if (PlayerTarget.IsValid(playerTarget))
        //    {
        //        if (Mathf.Approximately(this.resetCooloff, 0f))
        //        {
        //            playerTarget.direction = Common.GetVectorTo(this.main.BoneHead, playerTarget.GetTransform());
        //            playerTarget.sqrDistance = Vector3.SqrMagnitude(playerTarget.direction);
        //            playerTarget.distance = Mathf.Sqrt(playerTarget.sqrDistance);
        //            float distanceFactor = this.GetDistanceFactor(playerTarget.distance);
        //            float positionFactor = this.GetPositionFactor(Common.RemoveYComponent(playerTarget.direction));
        //            float hookedOnFactor = this.GetHookedOnFactor(playerTarget);
        //            float aggroPerFrame = this.GetAggroPerFrame(distanceFactor, positionFactor, hookedOnFactor);
        //            bool flag = PlayerTarget.IsValid(this.main.ActiveTarget);
        //            bool flag2 = this.main.ActiveTarget == playerTarget;
        //            bool flag3 = (this.main.PlayerIsHidden && aggroPerFrame < 0f) || !this.main.PlayerIsHidden;
        //            bool flag4 = !flag || (flag && flag2 && flag3);
        //            if (distanceFactor >= 1f && flag3)
        //            {
        //                this.SetAggro(playerTarget, 100f);
        //            }
        //            else if (flag4)
        //            {
        //                this.AddAggro(playerTarget, aggroPerFrame);
        //            }
        //        }
        //        else
        //        {
        //            playerTarget.aggro = Mathf.Clamp(playerTarget.aggro - 20f * Time.fixedDeltaTime, 0f, 100f);
        //            this.resetCooloff = Mathf.Clamp01(this.resetCooloff - Time.fixedDeltaTime * 0.1f);
        //        }
        //    }
        //}
        //foreach (PlayerTarget pt in this.main.PlayerTrackingScript.GetFarList())
        //{
        //    if (PlayerTarget.IsValid(pt))
        //    {
        //        this.SetAggro(pt, 0f);
        //    }
        //}
    }

    // Token: 0x060002BE RID: 702 RVA: 0x0001AA98 File Offset: 0x00018C98
    private float GetAggroPerFrame(float distMod, float posMod, float hookMod)
    {
        float num = (distMod <= 0.8f) ? (this.maxDistPerSecond + this.distBoostPerSecond) : this.maxDistPerSecond;
        float num2 = (posMod <= 0.8f) ? (this.maxPosPerSecond + this.posBoostPerSecond) : this.maxPosPerSecond;
        return (num * distMod + num2 * posMod + this.hookPerSecond * hookMod) * Time.fixedDeltaTime;
    }

    // Token: 0x060002BF RID: 703 RVA: 0x0001AB04 File Offset: 0x00018D04
    //private float GetDistanceFactor(float distance)
    //{
    //    if (distance < this.main.TitanClass.AggroFlipDistance)
    //    {
    //        return 1f - Common.GetRatio(distance, this.main.TitanClass.AggroMinDistance, this.main.TitanClass.AggroFlipDistance);
    //    }
    //    return -Common.GetRatio(distance, this.main.TitanClass.AggroFlipDistance, this.main.TitanClass.AggroMaxDistance);
    //}

    // Token: 0x060002C0 RID: 704 RVA: 0x0001AB7C File Offset: 0x00018D7C
    private float GetPositionFactor(Vector3 direction)
    {
        float num = Vector3.Dot(base.transform.forward.normalized, direction.normalized);
        if (num < -0.5f)
        {
            return -(1f - Common.GetRatio(num, -1f, -0.5f));
        }
        return Common.GetRatio(num, -0.5f, 0.5f);
    }

    // Token: 0x060002C1 RID: 705 RVA: 0x0001ABDC File Offset: 0x00018DDC
    private float GetHookedOnFactor(PlayerTarget pt)
    {
        float num = (!(pt.GetPlayerScript().LeftHookedTitan == base.gameObject)) ? 0f : 0.5f;
        float num2 = (!(pt.GetPlayerScript().RightHookedTitan == base.gameObject)) ? 0f : 0.5f;
        return num + num2;
    }

    // Token: 0x060002C2 RID: 706 RVA: 0x0001AC42 File Offset: 0x00018E42
    private float GetBlockingFactor()
    {
        return -this.timeSinceBlocked;
    }

    // Token: 0x060002C3 RID: 707 RVA: 0x0001AC4B File Offset: 0x00018E4B
    private float GetLineOfSightFactor()
    {
        return -this.timeSinceHidden;
    }

    // Token: 0x060002C4 RID: 708 RVA: 0x0001AC54 File Offset: 0x00018E54
    private void StartBlockClock()
    {
        if (this.blockClockRunning)
        {
            base.StopCoroutine(this.blockClock);
        }
        this.blockClock = this.BlockClock();
        base.StartCoroutine(this.blockClock);
    }

    // Token: 0x060002C5 RID: 709 RVA: 0x0001AC86 File Offset: 0x00018E86
    private void StopBlockClock()
    {
        if (this.blockClockRunning)
        {
            base.StopCoroutine(this.blockClock);
        }
        this.blockClockRunning = false;
        this.timeSinceBlocked = 0f;
    }

    // Token: 0x060002C6 RID: 710 RVA: 0x0001ACB4 File Offset: 0x00018EB4
    private IEnumerator BlockClock()
    {
        float increment = Time.fixedDeltaTime * 0.2f;
        this.blockClockRunning = true;
        this.timeSinceBlocked = 0f;
        while (this.timeSinceBlocked < 1f)
        {
            this.timeSinceBlocked += increment;
            yield return null;
        }
        this.timeSinceBlocked = Mathf.Clamp01(this.timeSinceBlocked);
        this.blockClockRunning = false;
        yield break;
    }

    // Token: 0x060002C7 RID: 711 RVA: 0x0001ACCF File Offset: 0x00018ECF
    private void StartSightClock()
    {
        if (this.sightClockRunning)
        {
            base.StopCoroutine(this.sightClock);
        }
        this.sightClock = this.SightClock();
        base.StartCoroutine(this.sightClock);
    }

    // Token: 0x060002C8 RID: 712 RVA: 0x0001AD01 File Offset: 0x00018F01
    private void StopSightClock()
    {
        if (this.sightClockRunning)
        {
            base.StopCoroutine(this.sightClock);
        }
        this.sightClockRunning = false;
        this.timeSinceHidden = 0f;
    }

    // Token: 0x060002C9 RID: 713 RVA: 0x0001AD2C File Offset: 0x00018F2C
    private IEnumerator SightClock()
    {
        float increment = Time.fixedDeltaTime * 0.25f;
        this.sightClockRunning = true;
        this.timeSinceHidden = 0f;
        while (this.timeSinceHidden < 1f)
        {
            this.timeSinceHidden += increment;
            yield return new WaitForEndOfFrame();
        }
        this.timeSinceHidden = Mathf.Clamp01(this.timeSinceHidden);
        this.sightClockRunning = false;
        yield break;
    }

    // Token: 0x060002CA RID: 714 RVA: 0x0001AD47 File Offset: 0x00018F47
    private void SetAggro(PlayerTarget pt, float value)
    {
        pt.aggro = Mathf.Clamp(value, 0f, 100f);
    }

    // Token: 0x060002CB RID: 715 RVA: 0x0001AD5F File Offset: 0x00018F5F
    private void AddAggro(PlayerTarget pt, float value)
    {
        pt.aggro = Mathf.Clamp(pt.aggro + value, 0f, 100f);
    }

    //private TitanMain main;

    // Token: 0x04000320 RID: 800
    public static float AGGRO_LIMIT = 50f;

    // Token: 0x04000321 RID: 801
    private float maxDistPerSecond = 20f;

    // Token: 0x04000322 RID: 802
    private float maxPosPerSecond = 20f;

    // Token: 0x04000323 RID: 803
    private float hookPerSecond = 35f;

    // Token: 0x04000324 RID: 804
    private float posBoostPerSecond = 40f;

    // Token: 0x04000325 RID: 805
    private float distBoostPerSecond = 30f;

    // Token: 0x04000326 RID: 806
    private float resetCooloff;

    // Token: 0x04000327 RID: 807
    private float timeSinceBlocked;

    // Token: 0x04000328 RID: 808
    private bool blockClockRunning;

    // Token: 0x04000329 RID: 809
    private IEnumerator blockClock;

    // Token: 0x0400032A RID: 810
    private float timeSinceHidden;

    // Token: 0x0400032B RID: 811
    private bool sightClockRunning;

    // Token: 0x0400032C RID: 812
    private IEnumerator sightClock;
}


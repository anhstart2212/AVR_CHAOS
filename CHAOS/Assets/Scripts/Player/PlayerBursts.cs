using Gamekit3D;
using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000037 RID: 55
public class PlayerBursts : Bolt.EntityBehaviour<IChaos_PlayerState>
{
    // Token: 0x06000205 RID: 517 RVA: 0x000105E8 File Offset: 0x0000E7E8
    private void Start()
    {
        this.rb = base.GetComponent<Rigidbody>();
        this.player = base.GetComponent<Player>();
        this.cam = this.player.transforms.playerCamera;
        this.rotator = this.player.transforms.rotator;
        this.player.BurstForceIsRunning = false;
        this.player.BurstTurnIsRunning = false;
        base.StartCoroutine(this.BurstTankManager());
    }

    // Token: 0x06000206 RID: 518 RVA: 0x00010660 File Offset: 0x0000E860
    private void Update()
    {
        if (!state.IsGrounded && this.player.BurstTankLevel > 0f)
        {
            if (Time.time > this.player.BurstStartTime + this.player.CurrentBurstRate)
            {
                this.DetectBurstKeys();
            }
        }
        else
        {
            this.player.BurstForceIsRunning = false;
            this.player.BurstTurnIsRunning = false;
            this.SetBurstTankDepleteAmount(0f);
        }
    }

    // Token: 0x06000207 RID: 519 RVA: 0x000106E4 File Offset: 0x0000E8E4
    private int DetectBurstKeys()
    {
        int num = 0;
        bool flag = false;
        bool flag2 = false;
        bool flag3 = false;
        bool flag4 = false;
        bool flag5 = false;
        bool flag6 = false;
        bool flag7 = false;
        bool flag8 = false;
        //float axisRaw = Input.GetAxisRaw(this.player.axisName.moveLeftRight);
        //float axisRaw2 = Input.GetAxisRaw(this.player.axisName.moveFrontBack);
        //float axisRaw3 = Input.GetAxisRaw(this.player.axisName.verticalBurst);

        float axisRaw = state.MovementXKey;
        float axisRaw2 = state.MovementYKey;
        float axisRaw3 = Input.GetAxisRaw(this.player.axisName.verticalBurst);
        if (axisRaw > 0f)
        {
            if (!this.adAxisDown)
            {
                flag4 = true;
                this.adAxisDown = true;
            }
        }
        else if (axisRaw < 0f)
        {
            if (!this.adAxisDown)
            {
                flag3 = true;
                this.adAxisDown = true;
            }
        }
        else
        {
            this.adAxisDown = false;
        }
        if (axisRaw2 > 0f)
        {
            if (!this.wsAxisDown)
            {
                flag = true;
                this.wsAxisDown = true;
            }
        }
        else if (axisRaw2 < 0f)
        {
            if (!this.wsAxisDown)
            {
                flag2 = true;
                this.wsAxisDown = true;
            }
        }
        else
        {
            this.wsAxisDown = false;
        }
        if (axisRaw3 > 0f)
        {
            flag7 = true;
            if (!this.qeAxisDown)
            {
                flag5 = true;
                this.qeAxisDown = true;
            }
        }
        else if (axisRaw3 < 0f)
        {
            flag8 = true;
            if (!this.qeAxisDown)
            {
                flag6 = true;
                this.qeAxisDown = true;
            }
        }
        else
        {
            this.qeAxisDown = false;
        }
        if (flag)
        {
            num++;
        }
        if (flag2)
        {
            num += 2;
        }
        if (flag3)
        {
            num += 4;
        }
        if (flag4)
        {
            num += 8;
        }
        if (this.useSingleTapForVertical)
        {
            if (flag7)
            {
                num += 16;
            }
            if (flag8)
            {
                num += 32;
            }
        }
        else
        {
            if (flag5)
            {
                num += 16;
            }
            if (flag6)
            {
                num += 32;
            }
        }
        int result = 0;
        if (this.player.WalledState != PlayerAnimation.WALL_STOP && Time.time <= this.burstTriggerTimer + this.burstTriggerWait)
        {
            if (this.useSingleTapForVertical && (flag7 || flag8))
            {
                if (!this.player.BurstForceIsRunning)
                {
                    this.ActivateBurstRoutines((!flag7) ? 32 : 16);
                    result = ((!flag7) ? 32 : 16);
                }
            }
            else if (num == this.burstCount && this.burstCheck)
            {
                if (!this.BurstCounterValid(num))
                {
                    Debug.LogWarning("Counter variable is out of bounds! Counter is set to: " + num);
                }
                else if (!this.player.BurstForceIsRunning)
                {
                    this.ActivateBurstRoutines(num);
                    result = num;
                }
                this.BurstCycle(0, true);
                num = 0;
            }
        }
        if (this.BurstCounterValid(num))
        {
            this.BurstCycle(num, false);
        }
        return result;
    }

    // Token: 0x06000208 RID: 520 RVA: 0x000109AE File Offset: 0x0000EBAE
    private bool BurstCounterValid(int counter)
    {
        return counter == 1 || counter == 2 || counter == 4 || counter == 8 || counter == 16 || counter == 32;
    }

    // Token: 0x06000209 RID: 521 RVA: 0x000109DF File Offset: 0x0000EBDF
    private void BurstCycle(int counter, bool reset)
    {
        if (!reset)
        {
            this.burstCount = counter;
            this.burstCheck = true;
            this.burstTriggerTimer = Time.time;
        }
        else
        {
            this.burstCount = 0;
            this.burstCheck = false;
            this.burstTriggerTimer = 0f;
        }
    }

    // Token: 0x0600020A RID: 522 RVA: 0x00010A20 File Offset: 0x0000EC20
    private IEnumerator HookBurstDelay(int counter)
    {
        this.player.MovementScript.Disable();
        while (this.player.IsEitherHooked)
        {
            yield return null;
        }
        this.player.MovementScript.Enable();
        this.ActivateBurstRoutines(counter);
        yield break;
    }

    // Token: 0x0600020B RID: 523 RVA: 0x00010A42 File Offset: 0x0000EC42
    public void Enable()
    {
    }

    // Token: 0x0600020C RID: 524 RVA: 0x00010A44 File Offset: 0x0000EC44
    public void Disable()
    {
        this.player.BurstForceIsRunning = false;
        this.player.BurstTurnIsRunning = false;
    }

    // Token: 0x0600020D RID: 525 RVA: 0x00010A60 File Offset: 0x0000EC60
    public void ActivateBurstRoutines(int burstDir)
    {
        Vector3 vector = Vector3.zero;
        Vector3 vector2 = new Vector3(base.transform.forward.x, 0f, base.transform.forward.z);
        Vector3 normalized = vector2.normalized;
        bool flag = true;
        this.player.BurstStartTime = Time.time;
        switch (burstDir)
        {
            case 1:
                vector = this.cam.forward;
                break;
            case 2:
                vector = -this.cam.forward;
                break;
            default:
                if (burstDir != 16)
                {
                    if (burstDir != 32)
                    {
                        Debug.LogWarning("burstDir variable is out of bounds! burstDir is set to: " + burstDir);
                        return;
                    }
                    vector = -Vector3.up + 0.1f * normalized;
                    flag = false;
                }
                else
                {
                    vector = Vector3.up + 0.1f * normalized;
                    flag = false;
                }
                break;
            case 4:
                vector = -this.cam.right;
                break;
            case 8:
                vector = this.cam.right;
                break;
        }
        this.player.CurrentBurstRate = this.player.limit.burstRateSeconds;
        int num;
        if (flag)
        {
            this.player.InstantGasConsumption(1);
            num = this.GetHorizontalBurstDirection(vector);
        }
        else
        {
            this.player.InstantGasConsumption(2);
            num = -1;
        }
        float value;
        switch (num + 1)
        {
            case 0:
                value = -1f;
                break;
            case 1:
                value = 0f;
                break;
            case 2:
                value = 1f;
                break;
            case 3:
                value = 3f;
                break;
            case 4:
                value = 5f;
                break;
            case 5:
                value = ((UnityEngine.Random.value >= 0.5f) ? 5.9f : 5.1f);
                break;
            case 6:
                value = 6f;
                break;
            case 7:
                value = 4f;
                break;
            case 8:
                value = 2f;
                break;
            default:
                value = 0f;
                break;
        }
        this.player.Animator.SetFloat("BurstType", value);
        if (flag)
        {
            base.StartCoroutine(this.BurstTurnHorz(num));
            base.StartCoroutine(this.BurstForceHorz(num));
        }
        else
        {
            base.StartCoroutine(this.BurstTurnVert(vector));
            base.StartCoroutine(this.BurstForceVert(Vector3.Normalize(vector)));
        }
    }

    // Token: 0x0600020E RID: 526 RVA: 0x00010D14 File Offset: 0x0000EF14
    private int GetHorizontalBurstDirection(Vector3 camDirection)
    {
        Vector3 vector = this.player.VelocityDirectionXZ;
        if (this.player.VelocityMagnitudeXZ < 1f)
        {
            vector = Common.RemoveYComponent(this.cam.forward).normalized;
        }
        camDirection = Common.RemoveYComponent(camDirection);
        float num = Vector3.Angle(vector, camDirection);
        bool flag = Vector3.Cross(vector, camDirection).y < 0f;
        this.AssignValidBurstDirectionElements(vector);
        int result;
        if (flag)
        {
            if (num < 90f)
            {
                float ratio = Common.GetRatio(num, 0f, 90f);
                if (ratio < 0.2f)
                {
                    result = 0;
                }
                else if (ratio < 0.7f)
                {
                    result = 7;
                }
                else
                {
                    result = 6;
                }
            }
            else
            {
                num -= 90f;
                float ratio2 = Common.GetRatio(num, 0f, 90f);
                if (ratio2 < 0.2f)
                {
                    result = 6;
                }
                else if (ratio2 < 0.7f)
                {
                    result = 5;
                }
                else
                {
                    result = 4;
                }
            }
        }
        else if (num < 90f)
        {
            float ratio3 = Common.GetRatio(num, 0f, 90f);
            if (ratio3 < 0.2f)
            {
                result = 0;
            }
            else if (ratio3 < 0.7f)
            {
                result = 1;
            }
            else
            {
                result = 2;
            }
        }
        else
        {
            num -= 90f;
            float ratio4 = Common.GetRatio(num, 0f, 90f);
            if (ratio4 < 0.2f)
            {
                result = 2;
            }
            else if (ratio4 < 0.7f)
            {
                result = 3;
            }
            else
            {
                result = 4;
            }
        }
        return result;
    }

    // Token: 0x0600020F RID: 527 RVA: 0x00010EBC File Offset: 0x0000F0BC
    private void AssignValidBurstDirectionElements(Vector3 velocity)
    {
        Vector3 vector = Vector3.Cross(velocity, Vector3.up);
        Vector3 vector2 = Vector3.Normalize(velocity + vector);
        Vector3 vector3 = Vector3.Normalize(velocity - vector);
        this.validBurstDirections[0] = velocity;
        this.validBurstDirections[1] = vector3;
        this.validBurstDirections[2] = -vector;
        this.validBurstDirections[3] = -vector2;
        this.validBurstDirections[4] = -velocity;
        this.validBurstDirections[5] = -vector3;
        this.validBurstDirections[6] = vector;
        this.validBurstDirections[7] = vector2;
    }

    // Token: 0x06000210 RID: 528 RVA: 0x00010F94 File Offset: 0x0000F194
    private IEnumerator BurstTurnHorz(int turnType)
    {
        Vector3 turnDir = this.validBurstDirections[turnType];
        if (turnType == 5 || turnType == 3 || turnType == 4)
        {
            turnDir = -turnDir;
        }
        float lerp = 0f;
        float turnTime = 0.5f;
        this.player.BurstTurnIsRunning = true;
        this.player.BurstStartRotation = base.transform.rotation;
        this.rotator.LookAt(this.rotator.position + turnDir);
        this.player.BurstEndRotation = this.rotator.rotation;
        float angleTimeFactor = Vector3.Angle(base.transform.forward, turnDir) / 180f;
        turnTime *= angleTimeFactor;
        while (lerp < 1f)
        {
            lerp += Time.deltaTime / turnTime;
            base.transform.rotation = Quaternion.Lerp(this.player.BurstStartRotation, this.player.BurstEndRotation, lerp);
            yield return null;
        }
        if (!this.player.IsEitherHooked)
        {
            while (!PlayerAnimation.InGasBurst(this.player.Animator))
            {
                yield return null;
            }
            float t = (turnType != 4 && turnType != 5 && turnType != 3) ? 0.825f : 0.9f;
            while (this.player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < t && PlayerAnimation.InGasBurst(this.player.Animator))
            {
                yield return null;
            }
        }
        this.player.BurstTurnIsRunning = false;
        yield break;
    }

    // Token: 0x06000211 RID: 529 RVA: 0x00010FB8 File Offset: 0x0000F1B8
    private IEnumerator BurstTurnVert(Vector3 turnDir)
    {
        float lerp = 0f;
        float turnTime = 0.875f;
        this.player.BurstTurnIsRunning = true;
        this.player.BurstStartRotation = base.transform.rotation;
        this.rotator.LookAt(this.rotator.position + turnDir);
        this.player.BurstEndRotation = this.rotator.rotation;
        float angleTimeFactor = Vector3.Angle(base.transform.forward, turnDir) / 180f;
        turnTime *= angleTimeFactor;
        while (this.player.BurstTurnIsRunning)
        {
            lerp += Time.deltaTime / turnTime;
            base.transform.rotation = Quaternion.Lerp(this.player.BurstStartRotation, this.player.BurstEndRotation, lerp);
            if (lerp >= 1f)
            {
                this.player.BurstTurnIsRunning = false;
            }
            yield return null;
        }
        yield break;
    }

    // Token: 0x06000212 RID: 530 RVA: 0x00010FDC File Offset: 0x0000F1DC
    private IEnumerator BurstTankManager()
    {
        this.burstTankManagerRunning = true;
        this.player.BurstTankLevel = 100f;
        float burstRefuelPerSecond = 75f;
        bool timerStarted = false;
        float startTime = 0f;
        while (this.burstTankManagerRunning)
        {
            switch (this.DetermineBurstTankState(timerStarted, startTime + 1.5f))
            {
                case 1:
                    this.DepleteBurstTank(this.burstDepletePerSecond * Time.deltaTime);
                    timerStarted = false;
                    break;
                case 2:
                    timerStarted = false;
                    break;
                case 3:
                    startTime = Time.time;
                    timerStarted = true;
                    break;
                case 4:
                    break;
                case 5:
                    this.DepleteBurstTank(-burstRefuelPerSecond * Time.deltaTime);
                    break;
                default:
                    Debug.LogWarning("Error: Burst tank manager achieved an unknown state!");
                    break;
            }
            this.player.BurstTankLevel = Mathf.Clamp(this.player.BurstTankLevel, 0f, 100f);
            yield return null;
        }
        yield break;
    }

    // Token: 0x06000213 RID: 531 RVA: 0x00010FF7 File Offset: 0x0000F1F7
    private void DepleteBurstTank(float value)
    {
        this.player.BurstTankLevel -= value;
    }

    // Token: 0x06000214 RID: 532 RVA: 0x0001100C File Offset: 0x0000F20C
    private void SetBurstTankDepleteAmount(float value)
    {
        this.burstDepletePerSecond = value;
    }

    // Token: 0x06000215 RID: 533 RVA: 0x00011018 File Offset: 0x0000F218
    private int DetermineBurstTankState(bool timerStarted, float waitTime)
    {
        int result = 0;
        if (this.burstDepletePerSecond > 0f)
        {
            result = 1;
        }
        else if (this.player.BurstTankLevel >= 100f)
        {
            result = 2;
        }
        else if (!timerStarted)
        {
            result = 3;
        }
        else if (Time.time < waitTime)
        {
            result = 4;
        }
        else if (Time.time >= waitTime)
        {
            result = 5;
        }
        return result;
    }

    // Token: 0x06000216 RID: 534 RVA: 0x00011088 File Offset: 0x0000F288
    private IEnumerator BurstForceVert(Vector3 direction)
    {
        if (this.player.BurstForceIsRunning)
        {
            yield break;
        }
        this.player.BurstForceIsRunning = true;
        string state = this.GetVertBurstState(direction, this.rb.velocity.y);
        bool downward = state == "00" || state == "01";
        int count = 0;
        if (state == "10" || state == "01")
        {
            while (count < 24)
            {
                if (this.BurstLoopExitCheck())
                {
                    yield break;
                }
                this.ScaleVelocity(direction, 0.97f);
                this.rb.AddForce(direction * this.player.speed.horzBurst * 2f, ForceMode.Acceleration);
                this.SetBurstTankDepleteAmount(110f);
                count++;
                yield return Common.yieldFixedUpdate;
                float yVel = this.rb.velocity.y;
                if ((state == "10" && yVel >= 0f) || (state == "01" && yVel <= 0f))
                {
                    this.rb.velocity = new Vector3(this.rb.velocity.x, 0f, this.rb.velocity.z);
                    break;
                }
            }
        }
        int available_frames = 24 - count;
        int force_frames = Mathf.RoundToInt(2f * (float)available_frames / 3f);
        int slow_frames = available_frames - force_frames;
        count = 0;
        float loopModifier = 1f;
        float loopModChange = 1f / (float)force_frames;
        float preMag = this.rb.velocity.y;
        while (Input.GetAxisRaw(this.player.axisName.verticalBurst) != 0f && this.player.BurstTankLevel > 0f)
        {
            this.ScaleVelocity(direction + this.player.VelocityDirection * 0.5f, 0.975f);
            float forceMod = this.GetBurstVertForceModifier();
            forceMod = ((!downward) ? forceMod : (forceMod / 2f));
            float forceMag = this.player.speed.horzBurst * Mathf.Clamp(1.5f * forceMod, 1f, 1.5f);
            if (this.player.IsEitherHooked)
            {
                forceMag *= 0.5f;
            }
            this.rb.AddForce(direction * forceMag, ForceMode.Acceleration);
            this.ScaleVelocityVertical(downward, forceMod);
            this.SetBurstTankDepleteAmount(110f);
            yield return Common.yieldFixedUpdate;
        }
        this.SetBurstTankDepleteAmount(0f);
        float postMag = this.rb.velocity.y;
        float targetMag = postMag - preMag;
        if (state == "11" || state == "10")
        {
            targetMag *= 0.66f;
        }
        else
        {
            targetMag *= 0.66f;
        }
        targetMag += preMag;
        count = 0;
        loopModifier = 0f;
        loopModChange = 1f / (float)slow_frames;
        while (count < slow_frames)
        {
            if (this.BurstLoopExitCheck())
            {
                yield break;
            }
            if (this.UseVertSlowDown(preMag, postMag))
            {
                float y = Mathf.Lerp(postMag, targetMag, loopModifier);
                this.rb.velocity = new Vector3(this.rb.velocity.x, y, this.rb.velocity.z);
                loopModifier += loopModChange;
            }
            count++;
            yield return Common.yieldFixedUpdate;
        }
        this.player.BurstForceIsRunning = false;
        yield break;
    }

    // Token: 0x06000217 RID: 535 RVA: 0x000110AC File Offset: 0x0000F2AC
    private bool UseVertSlowDown(float pre, float post)
    {
        if (pre <= 0f && post <= 0f)
        {
            return pre > post;
        }
        if (pre >= 0f && post >= 0f)
        {
            return pre < post;
        }
        Debug.LogWarning(string.Concat(new object[]
        {
            "Vert burst pre/post sign are out of order!:\nPre: ",
            pre,
            "  Post: ",
            post
        }));
        return false;
    }

    // Token: 0x06000218 RID: 536 RVA: 0x00011124 File Offset: 0x0000F324
    private string GetVertBurstState(Vector3 direction, float speed)
    {
        bool flag = Vector3.Angle(direction, Vector3.up) < 90f;
        bool flag2 = speed >= 0f;
        int num = 0;
        if (flag)
        {
            num = num;
        }
        else
        {
            num++;
        }
        if (flag2)
        {
            num += 2;
        }
        else
        {
            num += 4;
        }
        switch (num)
        {
            case 2:
                return "11";
            case 3:
                return "01";
            case 4:
                return "10";
            case 5:
                return "00";
            default:
                Debug.LogError("Vert Burst State error, unknown state ID: " + num);
                return string.Empty;
        }
    }

    // Token: 0x06000219 RID: 537 RVA: 0x000111C8 File Offset: 0x0000F3C8
    private float GetBurstVertForceModifier()
    {
        float maxVertical = this.player.limit.maxVertical;
        float velocityYAbsolute = this.player.VelocityYAbsolute;
        return Mathf.Clamp(1f - velocityYAbsolute / maxVertical, 0f, 1f);
    }

    // Token: 0x0600021A RID: 538 RVA: 0x0001120C File Offset: 0x0000F40C
    private IEnumerator BurstForceHorz(int horzType)
    {
        this.player.BurstForceIsRunning = true;
        Vector3 direction = this.validBurstDirections[horzType];
        float preMag = this.player.VelocityMagnitudeXZ;
        Vector3 preDir = this.player.VelocityDirectionXZ;
        float oldY = this.rb.velocity.y;
        float targetY = oldY / 3f;
        int count = 0;
        float loopModifier = 1f;
        float loopModChange = 0.06666667f;
        switch (horzType)
        {
            case 0:
                {
                    while (count < 15)
                    {
                        if (this.BurstLoopExitCheck())
                        {
                            yield break;
                        }
                        float newY = Mathf.Lerp(oldY, targetY, 1f - loopModifier);
                        this.rb.velocity = new Vector3(this.rb.velocity.x, newY, this.rb.velocity.z);
                        float speedModifier = 1f - this.player.VelocityMagnitudeXZ / this.player.limit.maxHorizontal;
                        float timeModifier = this.burstForceCurve.Evaluate(1f - loopModifier);
                        Vector3 force = direction * this.player.speed.horzBurst * 4f * speedModifier * timeModifier;
                        this.rb.AddForce(force, ForceMode.Acceleration);
                        this.SetBurstTankDepleteAmount(110f);
                        loopModifier -= loopModChange;
                        count++;
                        yield return Common.yieldFixedUpdate;
                    }
                    this.SetBurstTankDepleteAmount(0f);
                    float postMag = this.player.VelocityMagnitudeXZ;
                    if (postMag > preMag)
                    {
                        float scale = 0.5f * (1f - preMag / this.player.limit.maxHorizontal);
                        float targetMag = preMag + (postMag - preMag) * scale;
                        loopModifier = 0f;
                        while (count < 22)
                        {
                            if (this.BurstLoopExitCheck())
                            {
                                yield break;
                            }
                            float newMag = Mathf.Lerp(postMag, targetMag, loopModifier);
                            Vector3 newVel = this.player.VelocityDirectionXZ * newMag;
                            this.rb.velocity = new Vector3(newVel.x, this.rb.velocity.y, newVel.z);
                            loopModifier += loopModChange * 2f;
                            count++;
                            yield return Common.yieldFixedUpdate;
                        }
                    }
                    break;
                }
            case 1:
            case 7:
                {
                    while (count < 15)
                    {
                        if (this.BurstLoopExitCheck())
                        {
                            yield break;
                        }
                        float newY2 = Mathf.Lerp(oldY, targetY, 1f - loopModifier);
                        this.rb.velocity = new Vector3(this.rb.velocity.x, newY2, this.rb.velocity.z);
                        this.ScaleVelocity(direction, 0.93f);
                        float speedModifier2 = 1f - this.player.VelocityMagnitudeXZ / this.player.limit.maxHorizontal;
                        float timeModifier2 = this.burstForceCurve.Evaluate(1f - loopModifier);
                        Vector3 force2 = direction * this.player.speed.horzBurst * 2.5f * speedModifier2 * timeModifier2;
                        this.rb.AddForce(force2, ForceMode.Acceleration);
                        this.SetBurstTankDepleteAmount(110f);
                        loopModifier -= loopModChange;
                        count++;
                        yield return Common.yieldFixedUpdate;
                    }
                    this.SetBurstTankDepleteAmount(0f);
                    float postMag = this.player.VelocityMagnitudeXZ;
                    if (postMag > preMag)
                    {
                        float scale2 = 0.33f * (1f - preMag / this.player.limit.maxHorizontal);
                        float targetMag2 = preMag + (postMag - preMag) * scale2;
                        loopModifier = 0f;
                        while (count < 22)
                        {
                            if (this.BurstLoopExitCheck())
                            {
                                yield break;
                            }
                            float newMag2 = Mathf.Lerp(postMag, targetMag2, loopModifier);
                            Vector3 newVel2 = this.player.VelocityDirectionXZ * newMag2;
                            this.rb.velocity = new Vector3(newVel2.x, this.rb.velocity.y, newVel2.z);
                            loopModifier += loopModChange * 2f;
                            count++;
                            yield return Common.yieldFixedUpdate;
                        }
                    }
                    break;
                }
            case 2:
            case 6:
                while (count < 15)
                {
                    if (this.BurstLoopExitCheck())
                    {
                        yield break;
                    }
                    float newY3 = Mathf.Lerp(oldY, targetY, 1f - loopModifier);
                    Vector3 newVel3 = preDir * Mathf.Lerp(preMag, preMag * 0.5f, this.burstSideCurve.Evaluate(1f - loopModifier));
                    newVel3 += Vector3.Project(this.rb.velocity, direction);
                    this.rb.velocity = new Vector3(newVel3.x, newY3, newVel3.z);
                    float timeModifier3 = this.burstForceCurve.Evaluate(1f - loopModifier);
                    Vector3 force3 = direction * this.player.speed.horzBurst * 2.5f * timeModifier3;
                    this.rb.AddForce(force3, ForceMode.Acceleration);
                    this.SetBurstTankDepleteAmount(110f);
                    loopModifier -= loopModChange;
                    count++;
                    yield return Common.yieldFixedUpdate;
                }
                this.SetBurstTankDepleteAmount(0f);
                break;
            case 3:
            case 5:
                for (int i = 0; i < 5; i++)
                {
                    if (this.BurstLoopExitCheck())
                    {
                        yield break;
                    }
                    yield return Common.yieldFixedUpdate;
                }
                while (count < 15)
                {
                    if (this.BurstLoopExitCheck())
                    {
                        yield break;
                    }
                    this.ScaleVelocity(direction, 0.97f);
                    float newY4 = Mathf.Lerp(oldY, targetY, 1f - loopModifier);
                    this.rb.velocity = new Vector3(this.rb.velocity.x, newY4, this.rb.velocity.z);
                    float timeModifier4 = this.burstForceCurve.Evaluate(1f - loopModifier);
                    Vector3 force4 = direction * this.player.speed.horzBurst * 3f * timeModifier4;
                    this.rb.AddForce(force4, ForceMode.Acceleration);
                    loopModifier -= loopModChange;
                    this.SetBurstTankDepleteAmount(110f);
                    count++;
                    yield return Common.yieldFixedUpdate;
                }
                this.SetBurstTankDepleteAmount(0f);
                break;
            case 4:
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (this.BurstLoopExitCheck())
                        {
                            yield break;
                        }
                        yield return Common.yieldFixedUpdate;
                    }
                    float spdRat = Common.GetRatio(preMag, 0f, this.player.limit.maxHorizontal * 0.75f);
                    int forceFrames = Mathf.RoundToInt(22f * spdRat);
                    loopModChange = 1f / (float)forceFrames;
                    while (count < forceFrames)
                    {
                        if (this.BurstLoopExitCheck())
                        {
                            yield break;
                        }
                        if (this.player.VelocityMagnitudeXZ < this.player.speed.run / 3f)
                        {
                            break;
                        }
                        float newY5 = Mathf.Lerp(oldY, targetY, 1f - loopModifier);
                        Vector3 force5 = preDir * Mathf.Lerp(preMag, preMag * 0.2f, this.burstForceCurve.Evaluate(loopModifier));
                        this.rb.velocity = new Vector3(force5.x, newY5, force5.z);
                        this.SetBurstTankDepleteAmount(110f);
                        loopModifier -= loopModChange;
                        count++;
                        yield return Common.yieldFixedUpdate;
                    }
                    this.SetBurstTankDepleteAmount(0f);
                    loopModifier = 1f;
                    loopModChange = 1f / (float)(22 - forceFrames);
                    while (count < 22)
                    {
                        if (this.BurstLoopExitCheck())
                        {
                            yield break;
                        }
                        float newY6 = Mathf.Lerp(oldY, targetY, 1f - loopModifier);
                        this.rb.velocity = new Vector3(this.rb.velocity.x, newY6, this.rb.velocity.z);
                        float frameModifier = Mathf.Clamp((float)(count / 22), 0.33f, 1f);
                        float speedModifier3 = 1f - Common.GetRatio(this.player.VelocityMagnitudeXZ, this.player.speed.run, this.player.limit.maxHorizontal);
                        float timeModifier5 = this.burstForceCurve.Evaluate(1f - loopModifier);
                        Vector3 force6 = direction * this.player.speed.horzBurst * 2f * frameModifier * (speedModifier3 + timeModifier5);
                        this.rb.AddForce(force6, ForceMode.Acceleration);
                        this.SetBurstTankDepleteAmount(110f);
                        loopModifier -= loopModChange;
                        count++;
                        yield return Common.yieldFixedUpdate;
                    }
                    this.SetBurstTankDepleteAmount(0f);
                    break;
                }
        }
        while (count < 22)
        {
            if (this.BurstLoopExitCheck())
            {
                yield break;
            }
            if (horzType == 6 || horzType == 2)
            {
                float ratio = Common.GetRatio(this.player.VelocityMagnitudeXZ, this.player.speed.run, this.player.limit.maxHorizontal);
                float damp = Mathf.Lerp(1f, 0.9f, ratio);
                this.ScaleVelocity(preDir, damp);
            }
            count++;
            yield return Common.yieldFixedUpdate;
        }
        this.player.BurstForceIsRunning = false;
        yield break;
    }

    // Token: 0x0600021B RID: 539 RVA: 0x0001122E File Offset: 0x0000F42E
    private bool BurstLoopExitCheck()
    {
        if (state.IsGrounded)
        {
            this.player.BurstForceIsRunning = false;
            return true;
        }
        return false;
    }

    // Token: 0x0600021C RID: 540 RVA: 0x00011250 File Offset: 0x0000F450
    private void ScaleVelocity(Vector3 burstDirection, float damp)
    {
        Vector3 b = Vector3.Project(this.rb.velocity, burstDirection);
        Vector3 a = this.rb.velocity - b;
        a *= damp;
        Vector3 velocity = a + b;
        this.rb.velocity = velocity;
    }

    // Token: 0x0600021D RID: 541 RVA: 0x000112A0 File Offset: 0x0000F4A0
    private void ScaleVelocityVertical(bool downward, float damp)
    {
        float num = (!downward) ? 0.98f : 0.985f;
        float num2 = num + (1f - num) * damp;
        this.rb.velocity = new Vector3(this.rb.velocity.x, this.rb.velocity.y * num2, this.rb.velocity.z);
    }

    // Token: 0x04000229 RID: 553
    private Transform cam;

    // Token: 0x0400022A RID: 554
    private Transform rotator;

    // Token: 0x0400022B RID: 555
    private Player player;

    // Token: 0x0400022C RID: 556
    private Rigidbody rb;

    // Token: 0x0400022D RID: 557
    private bool adAxisDown;

    // Token: 0x0400022E RID: 558
    private bool wsAxisDown;

    // Token: 0x0400022F RID: 559
    private bool qeAxisDown;

    // Token: 0x04000230 RID: 560
    private bool burstCheck;

    // Token: 0x04000231 RID: 561
    private bool useSingleTapForVertical = true;

    // Token: 0x04000232 RID: 562
    private Vector3[] validBurstDirections = new Vector3[8];

    // Token: 0x04000233 RID: 563
    private const int DIR_VERT = -1;

    // Token: 0x04000234 RID: 564
    private const int DIR_F = 0;

    // Token: 0x04000235 RID: 565
    private const int DIR_FR = 1;

    // Token: 0x04000236 RID: 566
    private const int DIR_R = 2;

    // Token: 0x04000237 RID: 567
    private const int DIR_BR = 3;

    // Token: 0x04000238 RID: 568
    private const int DIR_B = 4;

    // Token: 0x04000239 RID: 569
    private const int DIR_BL = 5;

    // Token: 0x0400023A RID: 570
    private const int DIR_L = 6;

    // Token: 0x0400023B RID: 571
    private const int DIR_FL = 7;

    // Token: 0x0400023C RID: 572
    private float burstTriggerTimer;

    // Token: 0x0400023D RID: 573
    private float burstTriggerWait = 0.2f;

    // Token: 0x0400023E RID: 574
    private int burstCount;

    // Token: 0x0400023F RID: 575
    private const float MAX_SCALE = 0.9f;

    // Token: 0x04000240 RID: 576
    private const float BTANK_MAX = 100f;

    // Token: 0x04000241 RID: 577
    private const float BURST_SUSTAIN_COST = 110f;

    // Token: 0x04000242 RID: 578
    private float burstDepletePerSecond;

    // Token: 0x04000243 RID: 579
    private bool burstTankManagerRunning;

    // Token: 0x04000244 RID: 580
    public AnimationCurve burstForceCurve;

    // Token: 0x04000245 RID: 581
    public AnimationCurve burstSideCurve;
}

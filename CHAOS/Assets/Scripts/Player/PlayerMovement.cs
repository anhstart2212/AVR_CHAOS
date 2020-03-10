using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200003A RID: 58
public class PlayerMovement : Bolt.EntityBehaviour<IChaos_PlayerState>
{
    // Token: 0x0600022D RID: 557 RVA: 0x00013930 File Offset: 0x00011B30
    private void Start()
    {
        this.player = base.GetComponent<Player>();
        m_PlayerCombat = GetComponent<PlayerCombat>();
        this.rb = base.GetComponent<Rigidbody>();
        this.acceleration = new PlayerAcceleration(this.maxAccel, this.qConst, this.lConst, this.player.limit.maxHorizontal);
        //this.StartHookActions();
    }

    // Token: 0x0600022E RID: 558 RVA: 0x00013988 File Offset: 0x00011B88
    private void Update()
    {
        if (!this.player.IsGrounded)
        {
            this.velDirBeforeLand = this.player.VelocityDirectionXZ;
        }

        // Chaos Added
        if (!this.player.IsGrounded && !this.player.IsEitherHooked && !this.player.BurstForceIsRunning)
        {
            this.rb.AddForce(-Vector3.up * this.player.speed.groundGravity, ForceMode.Acceleration);
        }
        // Chaos Added

        if (this.player.WalledState != 0)
        {
            this.airMovementIsRunning = false;
            this.groundMovementIsRunning = false;
            this.hookMovementIsRunning = false;
            return;
        }

        if (this.player.IsGrounded && !this.groundMovementIsRunning)
        {
            base.StartCoroutine(this.GroundMovement());
            this.airMovementIsRunning = false;
        }

        if (this.AirMovementQualified() && !this.airMovementIsRunning)
        {
            base.StartCoroutine(this.AirMovement());
            this.groundMovementIsRunning = false;
        }
        if (this.player.IsEitherHooked && !this.hookMovementIsRunning)
        {
            base.StartCoroutine(this.HookMovement());
        }
    }

    // Token: 0x0600022F RID: 559 RVA: 0x00013A5D File Offset: 0x00011C5D
    public void Enable()
    {
        //this.StartHookActions();
    }

    // Token: 0x06000230 RID: 560 RVA: 0x00013A68 File Offset: 0x00011C68
    public void Disable()
    {
        base.StopAllCoroutines();
        this.groundMovementIsRunning = false;
        this.airMovementIsRunning = false;
        this.hookMovementIsRunning = false;
        this.hookActionRunning = false;
        if (this.player.CurrentLeftHook != null)
        {
            this.RetractHook(true);
        }
        if (this.player.CurrentRightHook != null)
        {
            this.RetractHook(false);
        }
    }

    //public void PlayerMove(Player player, float movementX, float movementY)
    //{
    //    this.player = player;
    //    this.player.MovementX = movementX;
    //    this.player.MovementY = movementY;
    //}

    //public void PlayerHook(Player player, bool fireLeftHook, bool fireRightHook, bool jumpReelKey, bool centerHook)
    //{
    //    this.player = player;
    //    this.player.FireLeftHook = fireLeftHook;
    //    this.player.FireRightHook = fireRightHook;
    //    this.player.JumpReelKeyDown = jumpReelKey;
    //    //this.player.CenterHookKeyDown = centerHook;
    //}

    // Token: 0x06000231 RID: 561 RVA: 0x00013AD4 File Offset: 0x00011CD4
    private IEnumerator GroundMovement()
    {
        this.groundMovementIsRunning = true;
        Vector3 foreXZ = Common.RemoveYComponent(base.transform.forward);
        new Vector3(base.transform.forward.x, 0f, base.transform.forward.z);
        this.landingAngle = Vector3.Angle(foreXZ, this.player.VelocityDirectionXZ);
        this.player.SlideState = this.GetSlideState();
        float ang = this.landingAngle;
        if (ang <= 135f)
        {
            ang = 1f - ang / 135f;
        }
        else
        {
            ang = 0f;
        }
        this.speed = this.player.VelocityMagnitudeXZ;
        if (this.speed < this.GetMinSlideSpeed())
        {
            this.speed *= ang;
        }
        while (this.groundMovementIsRunning)
        {
            Vector3 runDirection = (!this.player.IsSliding && !this.player.Animator.GetBool("IsSlideAnim")) ? base.transform.forward : this.velDirBeforeLand;
            if (!this.player.IsGrounded)
            {
                this.groundMovementIsRunning = false;
            }
            else
            {
                // Chaos Added
                if (m_PlayerCombat != null && m_PlayerCombat.InCombo)
                {
                    this.rb.velocity = Vector3.zero;
                }
                yield return new WaitUntil(() => m_PlayerCombat.InCombo == false);
                // Chaos Added

                this.DetermineMoveState();
                if (this.player.IsSlopedTerrain)
                {
                    runDirection = this.SlopedRunDirection(runDirection);
                }
                this.rb.velocity = runDirection.normalized * this.speed;
                yield return Common.yieldFixedUpdate;
            }
        }
        yield break;
    }

    // Token: 0x06000232 RID: 562 RVA: 0x00013AF0 File Offset: 0x00011CF0
    private Vector3 SlopedRunDirection(Vector3 runDir)
    {
        bool flag = false;
        Vector3 a = Vector3.zero;
        Vector3 b = Vector3.zero;
        RaycastHit raycastHit;
        bool flag2 = Physics.Raycast(base.transform.position + 0.25f * runDir, Vector3.down, out raycastHit, 2f, Common.layerOGT | Common.layerNoHook);
        if (flag2)
        {
            a = raycastHit.point;
            flag2 = Physics.Raycast(base.transform.position + 0.25f * -runDir, Vector3.down, out raycastHit, 2f, Common.layerOGT | Common.layerNoHook);
            if (flag2)
            {
                b = raycastHit.point;
            }
            else
            {
                flag = true;
            }
        }
        else
        {
            flag = true;
        }
        if (!flag)
        {
            runDir = a - b;
        }
        return runDir;
    }

    // Token: 0x06000233 RID: 563 RVA: 0x00013BC0 File Offset: 0x00011DC0
    private void DetermineMoveState()
    {
        //float num = Input.GetAxisRaw(this.player.axisName.moveFrontBack);
        //float num2 = Input.GetAxisRaw(this.player.axisName.moveLeftRight);
        float num = this.player.MovementY;
        float num2 = this.player.MovementX;

        int num3 = 0;
        if (num != 0f)
        {
            num = 1f;
        }
        if (num2 != 0f)
        {
            num2 = 1f;
        }
        if (num == 0f && num2 > 0f)
        {
            num = num2;
        }
        if (num > 0f)
        {
            num3 = 1;
        }
        if (this.player.IsAnimating)
        {
            num3 = 0;
        }
        if (this.player.IsSliding)
        {
            num3 = 1;
        }
        if (num3 != 0)
        {
            if (num3 == 1)
            {
                this.SpeedLerp(this.player.speed.run);
            }
        }
        else
        {
            this.SpeedLerp(0f);
        }
    }

    // Token: 0x06000234 RID: 564 RVA: 0x00013CA4 File Offset: 0x00011EA4
    private void SpeedLerp(float target)
    {
        float num = 0.01f;
        float num2 = 0.06f;
        float num3 = 0.0375f;
        float num4 = 0.0275f;
        float num5 = 0.1f;
        float num6 = 0.04f;
        float num7 = 0.2f;
        float num8 = 120f;
        float minSlideSpeed = this.GetMinSlideSpeed();
        float slideExitSpeed = this.GetSlideExitSpeed();
        this.player.IsSliding = (this.player.IsSliding ? (this.speed > slideExitSpeed) : (this.speed >= minSlideSpeed));
        if (target == 0f)
        {
            num = num7;
        }
        else if (target == this.player.speed.run)
        {
            num = ((this.speed < this.player.speed.run) ? num5 : num6);
        }
        if (this.player.IsSliding || this.player.Animator.GetBool("IsSlideAnim"))
        {
            Vector3 target2;
            if (this.player.SlideState == 0)
            {
                target2 = this.velDirBeforeLand;
                num = num4;
            }
            else if (this.player.SlideState == 1 || this.player.SlideState == 2)
            {
                Quaternion rotation = this.player.transforms.rotator.rotation;
                this.player.transforms.rotator.rotation = Quaternion.LookRotation(this.velDirBeforeLand);
                target2 = ((this.player.SlideState != 1) ? (-this.player.transforms.rotator.right) : this.player.transforms.rotator.right);
                this.player.transforms.rotator.rotation = rotation;
                num = num3;
            }
            else
            {
                target2 = -this.velDirBeforeLand;
                num = num2;
            }
            Vector3 current = new Vector3(base.transform.forward.x, 0f, base.transform.forward.z);
            float maxRadiansDelta = num8 * 0.0174532924f * Time.deltaTime;
            base.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(current, target2, maxRadiansDelta, 0f));
        }
        bool flag = this.player.Animator.GetBool("IsSlideAnim") && !this.player.Animator.GetBool("IsSliding");
        if (flag)
        {
            num /= 4f;
        }
        if (Mathf.Abs(target - this.speed) <= 1f)
        {
            this.speed = target;
        }
        float num9 = Mathf.Lerp(this.speed, target, num);
        if (flag && num9 < this.player.speed.run)
        {
            num9 = this.player.speed.run;
        }
        this.speed = num9;
    }

    // Token: 0x06000235 RID: 565 RVA: 0x00013FA0 File Offset: 0x000121A0
    private int GetSlideState()
    {
        if (this.landingAngle <= 60f)
        {
            return 0;
        }
        if (this.landingAngle > 120f)
        {
            return 3;
        }
        if (Vector3.Angle(base.transform.right, this.player.VelocityDirectionXZ) > 90f)
        {
            return 1;
        }
        return 2;
    }

    // Token: 0x06000236 RID: 566 RVA: 0x00013FFC File Offset: 0x000121FC
    private float GetMinSlideSpeed()
    {
        float result = this.player.limit.maxHorizontal * 0.75f;
        float result2 = this.player.limit.maxHorizontal * 0.625f;
        float result3 = this.player.limit.maxHorizontal * 0.5f;
        if (this.player.SlideState == 0)
        {
            return result;
        }
        if (this.player.SlideState == 3)
        {
            return result3;
        }
        return result2;
    }

    // Token: 0x06000237 RID: 567 RVA: 0x00014074 File Offset: 0x00012274
    private float GetSlideExitSpeed()
    {
        float result = this.player.speed.run * 2.5f;
        float result2 = this.player.speed.run * 2.25f;
        float result3 = this.player.speed.run * 2f;
        if (this.player.SlideState == 0)
        {
            return result;
        }
        if (this.player.SlideState == 3)
        {
            return result3;
        }
        return result2;
    }

    // Token: 0x06000238 RID: 568 RVA: 0x000140EC File Offset: 0x000122EC
    private void ScaleVelocity(Vector3 gasDirection)
    {
        float d = 0.99f;
        Vector3 b = Vector3.Project(this.player.RigidBody.velocity, gasDirection);
        Vector3 a = this.player.RigidBody.velocity - b;
        a *= d;
        Vector3 velocity = a + b;
        this.player.RigidBody.velocity = velocity;
    }

    // Token: 0x06000239 RID: 569 RVA: 0x00014150 File Offset: 0x00012350
    private IEnumerator AirMovement()
    {
        this.airMovementIsRunning = true;
        while (this.airMovementIsRunning)
        {
            if (PlayerAnimation.InGasBurst(this.player.Animator))
            {
                yield return Common.yieldFixedUpdate;
            }
            else
            {
                Vector3 gasDirection = Common.RemoveYComponent(base.transform.forward).normalized;
                float gasSpeed = this.GetNewGasSpeed();
                if (!this.AirMovementQualified())
                {
                    this.airMovementIsRunning = false;
                }
                else
                {
                    this.ScaleVelocity(gasDirection);
                    this.rb.AddForce(gasDirection * gasSpeed, ForceMode.Acceleration);
                    yield return Common.yieldFixedUpdate;
                }
            }
        }
        yield break;
    }

    // Token: 0x0600023A RID: 570 RVA: 0x0001416C File Offset: 0x0001236C
    private float GetNewGasSpeed()
    {
        float num = 0.8f * this.player.limit.maxHorizontal;
        float num2 = Mathf.Clamp(1f - this.player.VelocityMagnitudeXZ / num, 0f, 1f);
        float num3 = Vector3.Angle(base.transform.forward, this.player.VelocityDirectionXZ) / 180f;
        float num4 = num2 + num3;
        if (num4 > 1f)
        {
            num4 = 1f;
        }
        return this.player.limit.maxFreeGas * num4;
    }

    // Token: 0x0600023B RID: 571 RVA: 0x000141FC File Offset: 0x000123FC
    private bool AirMovementQualified()
    {
        return !this.player.IsEitherHooked && this.player.IsGasVFX;
    }

    public void StartHookActions(BoltEntity entity)
    {
        if (!this.hookActionRunning)
        {
            base.StartCoroutine(this.HookCableActions(entity));
        }
    }

    // Token: 0x0600023D RID: 573 RVA: 0x0001423C File Offset: 0x0001243C
    private IEnumerator HookCableActions(BoltEntity entity)
    {
        this.player = entity.GetComponent<Player>();

        this.hookActionRunning = true;
        for (; ; )
        {
            if (this.player.IsAttacking)
            {
                if (this.player.CurrentLeftHook != null && this.player.CurrentLeftHook.GetComponent<DrawHookCable>().reachedTarget)
                {
                    this.RetractHook(true);
                }
                if (this.player.CurrentRightHook != null && this.player.CurrentRightHook.GetComponent<DrawHookCable>().reachedTarget)
                {
                    this.RetractHook(false);
                }
                yield return null;
            }
            else
            {
                if ((!this.player.IsAnimating || this.player.IsSliding) && Time.timeScale != 0f)
                {
                    // Chaos Added
                    if (state.IsCenterHook)
                    {
                        yield return new WaitForSeconds(0.05f);
                    }
                    // Chaos Added

                    if (this.player.IsLeftTargetSet && state.IsLeftHook && this.player.CurrentLeftHook == null && !this.player.BurstForceIsRunning)
                    {
                        this.FireHook(true);
                    }
                    if (this.player.IsRightTargetSet && state.IsRightHook && this.player.CurrentRightHook == null && !this.player.BurstForceIsRunning)
                    {
                        this.FireHook(false);
                    }
                    if (!state.IsLeftHook && this.player.CurrentLeftHook != null && this.player.CurrentLeftHook.GetComponent<DrawHookCable>().reachedTarget)
                    {
                        this.RetractHook(true);
                    }
                    if (!state.IsRightHook && this.player.CurrentRightHook != null && this.player.CurrentRightHook.GetComponent<DrawHookCable>().reachedTarget)
                    {
                        this.RetractHook(false);
                    }

                    // Chaos Added
                    if (!this.player.IsLeftTargetSet && state.IsLeftHook && this.player.CurrentLeftHook == null && !this.player.BurstForceIsRunning)
                    {
                        this.FireHook(true, true);

                    }
                    if (!this.player.IsRightTargetSet && state.IsRightHook && this.player.CurrentRightHook == null && !this.player.BurstForceIsRunning)
                    {
                        this.FireHook(false, true);
                    }
                    // Chaos Added
                }
                yield return null;
            }
        }
    }

    // Token: 0x0600023E RID: 574 RVA: 0x00014258 File Offset: 0x00012458
    private IEnumerator HookMovement()
    {
        this.hookMovementIsRunning = true;
        while (this.hookMovementIsRunning)
        {
            if (!this.player.IsEitherHooked)
            {
                this.hookMovementIsRunning = false;
            }
            else
            {
                if (this.player.JumpReelKeyDown && (this.player.IsLeftImpulse || this.player.IsRightImpulse))
                {
                    this.HookImpulse();
                }
                if (!this.player.BurstTurnIsRunning && !this.player.BurstForceIsRunning)
                {
                    if (this.player.IsLeftHooked)
                    {
                        this.HookForces(true);
                    }
                    if (this.player.IsRightHooked)
                    {
                        this.HookForces(false);
                    }
                }
                yield return Common.yieldFixedUpdate;
            }
        }
        yield break;
    }

    // Token: 0x0600023F RID: 575 RVA: 0x00014274 File Offset: 0x00012474
    public void FireHook(bool isLeft, bool isNull = false)
    {
        this.player.InstantGasConsumption(0);
        this.HookSpawn(isLeft, isNull);
        if (isLeft)
        {
            this.player.SoundScript.leftFire = true;
            this.leftRetractCheck = true;
        }
        else
        {
            this.player.SoundScript.rightFire = true;
            this.rightRetractCheck = true;
        }
    }

    // Token: 0x06000240 RID: 576 RVA: 0x000142D0 File Offset: 0x000124D0
    public void RetractHook(bool isLeft)
    {
        if (isLeft)
        {
            this.player.CurrentLeftHook.GetComponent<DrawHookCable>().isRetracting = true;
            if (this.leftRetractCheck)
            {
                this.player.SoundScript.leftRetract = true;
                this.leftRetractCheck = false;
            }
        }
        else
        {
            this.player.CurrentRightHook.GetComponent<DrawHookCable>().isRetracting = true;
            if (this.rightRetractCheck)
            {
                this.player.SoundScript.rightRetract = true;
                this.rightRetractCheck = false;
            }
        }
    }

    // Token: 0x06000241 RID: 577 RVA: 0x0001435C File Offset: 0x0001255C
    private void HookSpawn(bool isLeftHook, bool isNull = false)
    {
        Transform transform = null;
        Vector3 vector = Vector3.zero;
        //GameObject titanObject;
        

        if (isLeftHook && !isNull)
        {
            player.IsLeftHookTargetNull = isNull;
            transform = this.player.transforms.hookOriginLeft;
            vector = this.player.LeftTargetPosition;
            this.player.LeftAnchorPosition = vector;
            //titanObject = this.player.LeftHookTitanObject;
        }
        if(!isLeftHook && !isNull)
        {
            player.IsRightHookTargetNull = isNull;
            transform = this.player.transforms.hookOriginRight;
            vector = this.player.RightTargetPosition;
            this.player.RightAnchorPosition = vector;
            //titanObject = this.player.RightHookTitanObject;
        }
        if (isLeftHook && isNull)
        {
            player.IsLeftHookTargetNull = isNull;
            transform = this.player.transforms.hookOriginLeft;
            vector = this.player.transforms.hookTargetNull.position;
            this.player.LeftAnchorPosition = vector;
        }
        if (!isLeftHook && isNull)
        {
            player.IsRightHookTargetNull = isNull;
            transform = this.player.transforms.hookOriginRight;
            vector = this.player.transforms.hookTargetNull.position;
            this.player.RightAnchorPosition = vector;
        }

        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.hook, transform.position, transform.rotation);
        DrawHookCable component = gameObject.GetComponent<DrawHookCable>();
        component.target = vector;
        component.playerObject = base.gameObject;
        component.isLeft = isLeftHook;
        //component.titanObject = titanObject;
        if (isLeftHook && !isNull)
        {
            this.player.CurrentLeftHook = gameObject;
        }
        if (!isLeftHook && !isNull)
        {
            this.player.CurrentRightHook = gameObject;
        }
        if (isLeftHook && isNull)
        {
            this.player.CurrentLeftHook = gameObject;
            StartCoroutine(RetractLeftHookDelay(1.2f));
        }
        if (!isLeftHook && isNull)
        {
            this.player.CurrentRightHook = gameObject;
            StartCoroutine(RetractRightHookDelay(1.2f));
        }
    }

    IEnumerator RetractLeftHookDelay(float time)
    {
        yield return new WaitForSeconds(time);
       
        if (this.player.CurrentLeftHook != null)
        {
            this.RetractHook(true);
        }
    }

    IEnumerator RetractRightHookDelay(float time)
    {
        yield return new WaitForSeconds(time);

        if (this.player.CurrentRightHook != null)
        {
            this.RetractHook(false);
        }
    }

    // Token: 0x06000242 RID: 578 RVA: 0x00014448 File Offset: 0x00012648
    private void HookImpulse()
    {
        Vector3 normalized = (this.player.LeftAnchorPosition - base.transform.position).normalized;
        Vector3 normalized2 = (this.player.RightAnchorPosition - base.transform.position).normalized;
        Vector3 a = Vector3.zero;
        if (this.player.IsDoubleHooked)
        {
            a = (normalized + normalized2).normalized;
        }
        else if (this.player.IsLeftHooked)
        {
            a = normalized;
        }
        else if (this.player.IsRightHooked)
        {
            a = normalized2;
        }
        this.rb.AddForce(a * this.player.speed.impulse, ForceMode.VelocityChange);
    }

    // Token: 0x06000243 RID: 579 RVA: 0x0001451C File Offset: 0x0001271C
    private void HookForces(bool isLeft)
    {
        int reeledState = this.player.ReeledState;
        bool flag = (this.player.IsDoubleHooked && isLeft) || !this.player.IsDoubleHooked;
        GameObject gameObject;
        float maxLength;
        Vector3 vector;
        if (isLeft)
        {
            gameObject = this.player.CurrentLeftHook;
            maxLength = this.player.MaxLengthLeft;
            vector = this.player.LeftAnchorPosition;
        }
        else
        {
            gameObject = this.player.CurrentRightHook;
            maxLength = this.player.MaxLengthRight;
            vector = this.player.RightAnchorPosition;
        }
        DrawHookCable component = gameObject.GetComponent<DrawHookCable>();
        if (reeledState == 0)
        {
            float d = this.SetReelSpeeds(false);
            float d2 = this.SetReelSpeeds(true);
            if (this.player.JumpReelKeyDown && flag)
            {
                float velocityMagnitude = this.player.VelocityMagnitude;
                float angleVelocityAndHook = Vector3.Angle(this.player.VelocityDirection, this.player.HookDirection);
                float d3 = this.acceleration.GetAcceleration(velocityMagnitude, angleVelocityAndHook);

                // Chaos Added
                if (this.player.FastSpeed)
                {
                    this.rb.AddForce(this.player.HookedSteerDirection() * d * d3 * this.player.speed.fastSpeed, ForceMode.Acceleration);
                }
                else
                {
                    this.rb.AddForce(this.player.HookedSteerDirection() * d * d3, ForceMode.Acceleration);
                }
                // Chaos Added
            }
            if (flag)
            {
                this.rb.AddForce(-Vector3.up * d2, ForceMode.Acceleration);
            }
        }
        if (!this.player.IsGrounded)
        {
            this.CableLengthAdjustment(isLeft, vector, component, maxLength);
        }
        else
        {
            float magnitude = (vector - base.transform.position).magnitude;
            if (magnitude < this.player.MaxHookDistance)
            {
                this.sjl.limit = (vector - base.transform.position).magnitude + 1f;
                component.hookJoint.linearLimit = this.sjl;
                if (isLeft)
                {
                    this.player.MaxLengthLeft = magnitude;
                }
                else
                {
                    this.player.MaxLengthRight = magnitude;
                }
            }
        }
    }

    // Token: 0x06000244 RID: 580 RVA: 0x00014720 File Offset: 0x00012920
    private float SetReelSpeeds(bool isReelGrav)
    {
        if (isReelGrav && !this.player.JumpReelKeyDown)
        {
            return this.player.speed.reelGravBrake;
        }
        if (this.player.IsDoubleHooked)
        {
            if (!isReelGrav)
            {
                return this.player.speed.reelDouble;
            }
            return this.player.speed.reelGravDouble;
        }
        else
        {
            if (!isReelGrav)
            {
                return this.player.speed.reelSingle;
            }
            return this.player.speed.reelGravSingle;
        }
    }

    // Token: 0x06000245 RID: 581 RVA: 0x000147B4 File Offset: 0x000129B4
    private void CableLengthAdjustment(bool isLeft, Vector3 anchorPoint, DrawHookCable hookScript, float maxLength)
    {
        float num = 0f;
        float magnitude = (anchorPoint - this.player.PositionNextFrame).magnitude;
        if (magnitude - maxLength > 0f)
        {
            num = magnitude - maxLength;
        }
        maxLength = magnitude - num;
        this.sjl = hookScript.hookJoint.linearLimit;
        this.sjl.limit = maxLength;
        hookScript.hookJoint.linearLimit = this.sjl;
        if (isLeft)
        {
            this.player.CurrentLengthLeft = magnitude;
            this.player.MaxLengthLeft = maxLength;
        }
        else
        {
            this.player.CurrentLengthRight = magnitude;
            this.player.MaxLengthRight = maxLength;
        }
    }

    // Token: 0x04000256 RID: 598
    private Player player;

    // Token: 0x04000257 RID: 599
    private Rigidbody rb;

    // Token: 0x04000258 RID: 600
    private bool groundMovementIsRunning;

    // Token: 0x04000259 RID: 601
    private bool airMovementIsRunning;

    // Token: 0x0400025A RID: 602
    private bool hookMovementIsRunning;

    // Token: 0x0400025B RID: 603
    private float speed;

    // Token: 0x0400025C RID: 604
    private float landingAngle;

    // Token: 0x0400025D RID: 605
    public GameObject hook;

    // Token: 0x0400025E RID: 606
    private SoftJointLimit sjl;

    // Token: 0x0400025F RID: 607
    private bool leftRetractCheck;

    // Token: 0x04000260 RID: 608
    private bool rightRetractCheck;

    // Token: 0x04000261 RID: 609
    private PlayerAcceleration acceleration;

    // Token: 0x04000262 RID: 610
    private Vector3 velDirBeforeLand = Vector3.zero;

    // Token: 0x04000263 RID: 611
    private float maxAccel = 45f;

    // Token: 0x04000264 RID: 612
    private float qConst = 3f;

    // Token: 0x04000265 RID: 613
    private float lConst = 0.25f;

    // Token: 0x04000266 RID: 614
    private bool hookActionRunning;

    private PlayerCombat m_PlayerCombat;

    public bool HookActionRunning
    {
        get
        {
            return this.hookActionRunning;
        }
    }
}

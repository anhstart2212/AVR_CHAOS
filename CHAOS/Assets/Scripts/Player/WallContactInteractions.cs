using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000040 RID: 64
public class WallContactInteractions : Bolt.EntityBehaviour<IChaos_PlayerState>
{
    // Token: 0x0600026D RID: 621 RVA: 0x00017051 File Offset: 0x00015251
    private void Start()
    {
        this.player = base.GetComponent<Player>();
        this.rb = base.GetComponent<Rigidbody>();
    }

    // Token: 0x0600026E RID: 622 RVA: 0x0001706C File Offset: 0x0001526C
    private void Update()
    {
        this.player.ReeledState = this.reeledState;
        int walledState = this.player.WalledState;
        this.player.WalledState = this.WallRun();
        if (walledState != this.player.WalledState)
        {
            if (walledState == 0)
            {
                this.StartClock();
            }
            else
            {
                this.StopClock();
            }
        }
        if (this.player.IsEitherHooked)
        {
            if (!this.hookedOnWallIsRunning && this.player.IsStationary)
            {
                base.StartCoroutine(this.HookedOnWall());
            }
            else if (this.hookedOnWallIsRunning)
            {
                return;
            }
        }
    }

    // Token: 0x0600026F RID: 623 RVA: 0x00017118 File Offset: 0x00015318
    public void Enable()
    {
    }

    // Token: 0x06000270 RID: 624 RVA: 0x0001711A File Offset: 0x0001531A
    public void Disable()
    {
        base.StopAllCoroutines();
        this.hookedOnWallIsRunning = false;
    }

    // Token: 0x06000271 RID: 625 RVA: 0x0001712C File Offset: 0x0001532C
    private IEnumerator HookedOnWall()
    {
        this.hookedOnWallIsRunning = true;
        int delayCount = 0;
        bool brakingLastFrame = false;
        while (this.hookedOnWallIsRunning)
        {
            bool againstWall = Physics.Raycast(new Ray(this.FindRayOrigins(), base.transform.forward), 1f, Common.layerObstacle);
            if (this.player.IsDoubleHooked)
            {
                if (this.player.IsStationary && !this.player.IsGroundTakeOff && !state.IsGrounded && againstWall)
                {
                    if (delayCount == 10)
                    {
                        this.reeledState = 3;
                    }
                    if (delayCount < 10)
                    {
                        delayCount++;
                    }
                }
            }
            else if (this.player.IsLeftHooked)
            {
                if (delayCount > 0)
                {
                    this.reeledState = 0;
                }
                if (this.player.IsStationary && againstWall)
                {
                    this.reeledState = 1;
                }
            }
            else if (this.player.IsRightHooked)
            {
                if (delayCount > 0)
                {
                    this.reeledState = 0;
                }
                if (this.player.IsStationary && againstWall)
                {
                    this.reeledState = 2;
                }
            }
            else
            {
                this.hookedOnWallIsRunning = false;
                this.reeledState = 0;
            }
            if (state.IsJumpReelKey && brakingLastFrame)
            {
                this.reeledState = 0;
            }
            if (!this.player.IsStationary)
            {
                this.reeledState = 0;
                this.hookedOnWallIsRunning = false;
            }
            //brakingLastFrame = Input.GetButton(this.player.axisName.jumpReel);
            brakingLastFrame = state.IsJumpReelKey;
            yield return null;
        }
        yield break;
    }

    // Token: 0x06000272 RID: 626 RVA: 0x00017147 File Offset: 0x00015347
    private void StartJump(int startFrames)
    {
        if (!this.jumpRoutineRunning)
        {
            this.jumpRoutine = this.JumpRoutine(startFrames);
            base.StartCoroutine(this.jumpRoutine);
        }
    }

    // Token: 0x06000273 RID: 627 RVA: 0x0001716E File Offset: 0x0001536E
    private void StopJump()
    {
        if (this.jumpRoutineRunning)
        {
            base.StopCoroutine(this.jumpRoutine);
        }
    }

    // Token: 0x06000274 RID: 628 RVA: 0x00017188 File Offset: 0x00015388
    private IEnumerator JumpRoutine(int stopFrames)
    {
        this.jumpRoutineRunning = true;
        int frameCount = 0;
        while (this.jumpRoutineRunning)
        {
            Ray wallRay = this.GetWallHitRay();
            if (!this.RayIsValid(wallRay))
            {
                break;
            }
            if (frameCount == stopFrames)
            {
                Vector3 jumpForce = this.GetJumpForce(wallRay);
                this.rb.AddForce(jumpForce, ForceMode.VelocityChange);
                this.jumpRoutineRunning = false;
            }
            frameCount++;
            yield return Common.yieldFixedUpdate;
        }
        this.jumpRoutineRunning = false;
        yield break;
    }

    // Token: 0x06000275 RID: 629 RVA: 0x000171AC File Offset: 0x000153AC
    private IEnumerator WallStopRoutine()
    {
        this.wallStopRoutineRunning = true;
        int frameCount = 0;
        Vector3 startVel = this.rb.velocity * 0.5f;
        float t = 0f;
        while (this.wallStopRoutineRunning)
        {
            this.OverrideRotation(this.GetWallHitRay().direction, 1f);
            if (this.player.IsEitherHooked)
            {
                break;
            }
            if (/*Input.GetButtonDown(this.player.axisName.jumpReel)*/ state.IsJumpReelKey)
            {
                this.StopJump();
                this.StartJump(16);
                break;
            }
            t = (float)frameCount / 28f;
            this.rb.velocity = Vector3.Lerp(startVel, Vector3.zero, this.wallStopCurve.Evaluate(t));
            if (frameCount == 28)
            {
                this.wallStopRoutineRunning = false;
            }
            frameCount++;
            yield return Common.yieldFixedUpdate;
        }
        this.wallStopRoutineRunning = false;
        yield break;
    }

    // Token: 0x06000276 RID: 630 RVA: 0x000171C8 File Offset: 0x000153C8
    private int WallRun()
    {
        if (this.player.WalledState == 0)
        {
            this.runEntry = false;
        }
        if (this.jumpRoutineRunning)
        {
            return PlayerAnimation.WALL_JUMP;
        }
        if (this.wallStopRoutineRunning)
        {
            return PlayerAnimation.WALL_STOP;
        }
        if (state.IsGrounded || this.player.ReeledState != 0)
        {
            return PlayerAnimation.WALL_NONE;
        }
        Ray wallHitRay = this.GetWallHitRay();
        if (!this.RayIsValid(wallHitRay))
        {
            return PlayerAnimation.WALL_NONE;
        }
        bool flag = this.WallJump(wallHitRay);
        if (flag)
        {
            this.StartJump(6);
            return PlayerAnimation.WALL_JUMP;
        }
        Vector3 velocityDirection = this.player.VelocityDirection;
        Vector3 vector = Common.RemoveYComponent(wallHitRay.direction);
        float num = Vector3.Angle(Vector3.up, velocityDirection);
        float num2;
        if (num < 90f)
        {
            num2 = 1f - Common.GetRatio(num, 0f, 90f);
        }
        else
        {
            num2 = -1f * Common.GetRatio(num, 90f, 135f);
        }
        float a = this.player.VelocityYAbsolute / this.player.limit.maxVertical;
        float b = this.player.VelocityMagnitudeXZ / this.player.limit.maxHorizontal;
        float ratio = Common.GetRatio(Mathf.Max(a, b), (!this.clockRunning) ? 0.4f : 0.25f, 0.75f);
        float newSide = (Vector3.Cross(vector, this.player.VelocityDirectionXZ).y >= 0f) ? 1f : -1f;
        if (num2 <= -1f || ratio <= 0f)
        {
            return PlayerAnimation.WALL_NONE;
        }
        bool flag2 = this.CheckForStop(wallHitRay);
        if (flag2)
        {
            return PlayerAnimation.WALL_NONE;
        }
        PlayerAnimation.SetWallMovementParametersWithLerp(this.player.Animator, num2, newSide, ratio);
        this.OverrideRotation(vector, num2);
        if (!this.player.IsEitherHooked || !state.IsJumpReelKey)
        {
            this.ScaleVelocity();
            this.LockPositionToWall(wallHitRay);
        }
        return PlayerAnimation.WALL_RUN;
    }

    // Token: 0x06000277 RID: 631 RVA: 0x000173F8 File Offset: 0x000155F8
    private bool WallJump(Ray wallRay)
    {
        if (this.player.WalledState != PlayerAnimation.WALL_RUN)
        {
            return false;
        }
        float num = this.player.VelocityYAbsolute / this.player.limit.maxVertical;
        float num2 = this.player.VelocityMagnitudeXZ / this.player.limit.maxHorizontal;
        bool flag;
        if (num > num2)
        {
            flag = (num <= 0.33f);
        }
        else
        {
            flag = (num2 <= 0.25f);
        }
        flag = false;
        if (!flag && !this.jumpStarted)
        {
            this.jumpStarted = true;
        }
        if (this.jumpStarted && (flag || /*Input.GetButtonDown(this.player.axisName.jumpReel)*/ state.IsJumpReelKey))
        {
            this.jumpStarted = false;
            return true;
        }
        return false;
    }

    // Token: 0x06000278 RID: 632 RVA: 0x000174CC File Offset: 0x000156CC
    private bool CheckForStop(Ray wallRay)
    {
        if (!this.runEntry)
        {
            float num = Vector3.Angle(this.player.VelocityDirection, wallRay.direction);
            if (num >= 160f && !this.wallStopRoutineRunning)
            {
                base.StartCoroutine(this.WallStopRoutine());
            }
            this.runEntry = true;
            return num >= 160f;
        }
        return false;
    }

    // Token: 0x06000279 RID: 633 RVA: 0x00017534 File Offset: 0x00015734
    private void LockPositionToWall(Ray wallRay)
    {
        Vector3 position = base.transform.position;
        Vector3 point = wallRay.GetPoint(0.3f);
        point.y = position.y;
        base.transform.position = Vector3.Lerp(position, point, 0.1f);
    }

    // Token: 0x0600027A RID: 634 RVA: 0x00017580 File Offset: 0x00015780
    private void OverrideRotation(Vector3 normal, float vertAngNorm = 1f)
    {
        Vector3 direction;
        if (vertAngNorm > 0f)
        {
            Vector3 b = -Common.RemoveYComponent(normal).normalized;
            Vector3 velocityDirectionXZ = this.player.VelocityDirectionXZ;
            direction = Vector3.Lerp(velocityDirectionXZ, b, vertAngNorm);
        }
        else
        {
            direction = this.player.VelocityDirectionXZ;
        }
        direction = direction.normalized;
        this.player.RotationScript.RotatePlayer(3, direction);
    }

    // Token: 0x0600027B RID: 635 RVA: 0x000175EC File Offset: 0x000157EC
    private void ScaleVelocity()
    {
        float num = 0.01999998f;
        float num2 = 0.99f - num * this.speedSlowCurve.Evaluate(this.runTimeNormalized);
        float num3 = 0.01999998f;
        float num4 = 0.99f - num3 * this.speedSlowCurve.Evaluate(this.runTimeNormalized);
        this.rb.velocity = new Vector3(this.rb.velocity.x * num2, this.rb.velocity.y * num4, this.rb.velocity.z * num2);
    }

    // Token: 0x0600027C RID: 636 RVA: 0x0001768B File Offset: 0x0001588B
    private void StartClock()
    {
        this.StopClock();
        this.runClock = this.RunClock();
        base.StartCoroutine(this.runClock);
        this.clockRunning = true;
    }

    // Token: 0x0600027D RID: 637 RVA: 0x000176B3 File Offset: 0x000158B3
    private void StopClock()
    {
        if (this.runClock == null)
        {
            return;
        }
        if (this.clockRunning)
        {
            base.StopCoroutine(this.runClock);
        }
        this.clockRunning = false;
    }

    // Token: 0x0600027E RID: 638 RVA: 0x000176E0 File Offset: 0x000158E0
    private IEnumerator RunClock()
    {
        this.runTimeNormalized = 0f;
        float runTime = 0f;
        while (this.runTimeNormalized < 1f)
        {
            runTime += Time.deltaTime;
            this.runTimeNormalized = Mathf.Clamp01(runTime / 1f);
            yield return null;
        }
        yield break;
    }

    // Token: 0x0600027F RID: 639 RVA: 0x000176FC File Offset: 0x000158FC
    private Vector3 GetJumpForce(Ray wallRay)
    {
        float num = Mathf.Clamp01(this.player.Animator.GetFloat("WallRunVertical"));
        Vector3 vector = wallRay.direction + Vector3.up * num + this.player.VelocityDirection * (1f - num);
        Vector3.Normalize(vector);
        float num2 = Mathf.Abs(4f);
        float d = Mathf.Min(7f, 3f) + num2 * (1f - num);
        return vector * d;
    }

    // Token: 0x06000280 RID: 640 RVA: 0x0001778B File Offset: 0x0001598B
    private bool RayIsValid(Ray ray)
    {
        return !(ray.origin == Vector3.zero) || !(ray.direction == Vector3.zero);
    }

    // Token: 0x06000281 RID: 641 RVA: 0x000177BC File Offset: 0x000159BC
    private Ray GetWallHitRay()
    {
        int layerMask = Common.layerObstacle | Common.layerNoHook;
        Vector3 vector = this.FindRayOrigins();
        Vector3 vector2 = new Vector3(base.transform.position.x, vector.y, base.transform.position.z);
        float num = Vector3.Magnitude(vector - vector2);
        Vector3 vector3 = Vector3.Cross(this.player.VelocityDirectionXZ, Vector3.up);
        Vector3 b = -vector3;
        Vector3 velocityDirectionXZ = this.player.VelocityDirectionXZ;
        Vector3 a = velocityDirectionXZ + vector3;
        Vector3 a2 = velocityDirectionXZ + b;
        float num2 = 0.9375f + num;
        float num3 = 1.25f + num;
        Ray[] array = new Ray[]
        {
            new Ray(vector, velocityDirectionXZ * num2),
            new Ray(vector, a * num3),
            new Ray(vector, a2 * num3)
        };
        for (int i = 0; i < array.Length; i++)
        {
            Debug.DrawRay(vector2, array[i].direction * ((i != 0) ? num3 : num2), Color.magenta);
        }
        for (int j = 0; j < array.Length; j++)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(array[j], out raycastHit, (j != 0) ? num3 : num2, layerMask))
            {
                return new Ray(raycastHit.point, raycastHit.normal);
            }
        }
        return new Ray(Vector3.zero, Vector3.zero);
    }

    // Token: 0x06000282 RID: 642 RVA: 0x00017980 File Offset: 0x00015B80
    private Vector3 FindRayOrigins()
    {
        Vector3 positionNextFrame = this.player.PositionNextFrame;
        positionNextFrame.y = base.transform.position.y - 0.7f;
        return positionNextFrame;
    }

    // Token: 0x040002A9 RID: 681
    private Player player;

    // Token: 0x040002AA RID: 682
    private Rigidbody rb;

    // Token: 0x040002AB RID: 683
    private const int REEL_NONE = 0;

    // Token: 0x040002AC RID: 684
    private const int REEL_LEFT = 1;

    // Token: 0x040002AD RID: 685
    private const int REEL_RIGHT = 2;

    // Token: 0x040002AE RID: 686
    private const int REEL_BOTH = 3;

    // Token: 0x040002AF RID: 687
    private int reeledState;

    // Token: 0x040002B0 RID: 688
    private float leftCableLength;

    // Token: 0x040002B1 RID: 689
    private float rightCableLength;

    // Token: 0x040002B2 RID: 690
    private bool hookedOnWallIsRunning;

    // Token: 0x040002B3 RID: 691
    private const int WALL_NONE = 0;

    // Token: 0x040002B4 RID: 692
    private const int WALL_UP = 1;

    // Token: 0x040002B5 RID: 693
    private const int WALL_LEFT = 2;

    // Token: 0x040002B6 RID: 694
    private const int WALL_RIGHT = 3;

    // Token: 0x040002B7 RID: 695
    private const int WALL_STOP = 4;

    // Token: 0x040002B8 RID: 696
    public AnimationCurve wallStopCurve;

    // Token: 0x040002B9 RID: 697
    public AnimationCurve speedSlowCurve;

    // Token: 0x040002BA RID: 698
    private const int RUN_JUMP_FRAMES = 6;

    // Token: 0x040002BB RID: 699
    private const int STOP_JUMP_FRAMES = 16;

    // Token: 0x040002BC RID: 700
    private IEnumerator jumpRoutine;

    // Token: 0x040002BD RID: 701
    private bool jumpRoutineRunning;

    // Token: 0x040002BE RID: 702
    private bool wallStopRoutineRunning;

    // Token: 0x040002BF RID: 703
    private bool jumpStarted;

    // Token: 0x040002C0 RID: 704
    private bool runEntry;

    // Token: 0x040002C1 RID: 705
    private const float max_run_time = 1f;

    // Token: 0x040002C2 RID: 706
    private float runTimeNormalized;

    // Token: 0x040002C3 RID: 707
    private IEnumerator runClock;

    // Token: 0x040002C4 RID: 708
    private bool clockRunning;
}

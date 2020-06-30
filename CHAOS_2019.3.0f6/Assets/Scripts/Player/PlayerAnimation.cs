using System;
using UnityEngine;

// Token: 0x02000033 RID: 51
public static class PlayerAnimation
{
    // Token: 0x060001E3 RID: 483 RVA: 0x0000F21C File Offset: 0x0000D41C
    public static bool MinimumVelocities(Animator anim, float vert, float horz)
    {
        return anim.GetFloat("VertVelocity") >= vert && anim.GetFloat("HorzVelocity") >= horz;
    }

    // Token: 0x060001E4 RID: 484 RVA: 0x0000F244 File Offset: 0x0000D444
    public static bool InIdleRunOrAir(Animator anim, bool includeHookMove)
    {
        return Common.CurrentStateName(anim, 0, "Idle") || Common.CurrentStateName(anim, 0, "Run") || Common.CurrentStateTag(anim, 0, PlayerAnimation.TAGS.airborne) || (Common.CurrentStateTag(anim, 0, PlayerAnimation.TAGS.hooked) && includeHookMove);
    }

    // Token: 0x060001E6 RID: 486 RVA: 0x0000F31A File Offset: 0x0000D51A
    public static bool InGasBurst(Animator anim)
    {
        return Common.CurrentOrNextStateTag(anim, 0, PlayerAnimation.TAGS.gasBurst);
    }

    // Token: 0x060001E8 RID: 488 RVA: 0x0000F35C File Offset: 0x0000D55C
    public static void SetWallMovementParametersWithLerp(Animator anim, float newVertAng, float newSide, float newSpeed)
    {
        anim.SetFloat(PlayerAnimation.PARAMS.wallRunVertical, Mathf.Lerp(anim.GetFloat(PlayerAnimation.PARAMS.wallRunVertical), newVertAng, 0.2f));
        anim.SetFloat(PlayerAnimation.PARAMS.wallRunSide, Mathf.Lerp(anim.GetFloat(PlayerAnimation.PARAMS.wallRunSide), newSide, 0.2f));
        anim.SetFloat(PlayerAnimation.PARAMS.wallRunSpeed, Mathf.Lerp(anim.GetFloat(PlayerAnimation.PARAMS.wallRunSpeed), newSpeed, 0.2f));
    }

    // Token: 0x060001E9 RID: 489 RVA: 0x0000F3EA File Offset: 0x0000D5EA
    public static void SetWallMovementState(Animator anim, int state)
    {
        anim.SetInteger(PlayerAnimation.PARAMS.wallMoveState, state);
    }

    // Token: 0x060001EA RID: 490 RVA: 0x0000F400 File Offset: 0x0000D600
    public static void SetAttackParametersWithLerp(Animator anim, float newSide, float newHeight, float newFacing, float newDistance)
    {
 
    }

    // Token: 0x060001EB RID: 491 RVA: 0x0000F4BC File Offset: 0x0000D6BC
    public static void SetAttackParameters(Animator anim, float newSide, float newHeight, float newFacing, float newDistance)
    {

    }

    // Token: 0x060001EC RID: 492 RVA: 0x0000F50E File Offset: 0x0000D70E
    public static void SetAttackState(Animator anim, bool primed, bool released)
    {

    }

    public static void SetAttackState(Animator anim)
    {

    }

    // Token: 0x04000204 RID: 516
    public static PlayerAnimation.Tags TAGS = new PlayerAnimation.Tags();

    // Token: 0x04000205 RID: 517
    public static PlayerAnimation.Params PARAMS = new PlayerAnimation.Params();

    // Token: 0x04000206 RID: 518
    public static readonly int WALL_NONE = 0;

    // Token: 0x04000207 RID: 519
    public static readonly int WALL_RUN = 1;

    // Token: 0x04000208 RID: 520
    public static readonly int WALL_JUMP = 2;

    // Token: 0x04000209 RID: 521
    public static readonly int WALL_STOP = 3;

    // Token: 0x02000034 RID: 52
    public class Tags
    {
        // Token: 0x0400020C RID: 524
        public readonly string grabEvade = "GrabEvade";

        // Token: 0x0400020D RID: 525
        public readonly string slide = "Slide";

        // Token: 0x0400020E RID: 526
        public readonly string hookAnimation = "HookAnimation";

        // Token: 0x0400020F RID: 527
        public readonly string hooked = "Hooked";

        // Token: 0x04000210 RID: 528
        public readonly string airborne = "Airborne";

        // Token: 0x04000211 RID: 529
        public readonly string gasBurst = "GasBurst";

        // Token: 0x04000212 RID: 530
        public readonly string acrobatics = "Acrobatics";

        // Token: 0x04000213 RID: 531
        public readonly string wallMovement = "WallMovement";

        // Token: 0x04000214 RID: 532
        public readonly string grabbed = "Grabbed";
    }

    // Token: 0x02000035 RID: 53
    public class Params
    {
        // Token: 0x04000215 RID: 533
        public readonly string wallMoveState = "WallMoveState";

        // Token: 0x04000216 RID: 534
        public readonly string wallRunVertical = "WallRunVertical";

        // Token: 0x04000217 RID: 535
        public readonly string wallRunSide = "WallRunSide";

        // Token: 0x04000218 RID: 536
        public readonly string wallRunSpeed = "WallRunSpeed";

        // Token: 0x0400021F RID: 543
        public readonly string grabbedState = "GrabbedState";

    }
}


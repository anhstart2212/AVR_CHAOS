using System;
using UnityEngine;

// Token: 0x0200003D RID: 61
public class PlayerRotations : MonoBehaviour
{
    // Token: 0x06000254 RID: 596 RVA: 0x00015D06 File Offset: 0x00013F06
    private void Start()
    {
        this.player = base.GetComponent<Player>();
        this.cam = this.player.transforms.playerCamera;
        this.rotator = this.player.transforms.rotator;
    }

    // Token: 0x06000255 RID: 597 RVA: 0x00015D40 File Offset: 0x00013F40
    private void Update()
    {
        if (this.player.IsAnimating)
        {
            this.RotatePlayer(-1, this.player.VelocityDirection);
            return;
        }
        if (this.player.ReeledState != 0)
        {
            this.RotatePlayer(2, Common.RemoveYComponent(this.HookDirection()));
            return;
        }
        if (this.player.WalledState != PlayerAnimation.WALL_NONE)
        {
            return;
        }
        if (this.player.BurstTurnIsRunning || this.player.BurstForceIsRunning)
        {
            return;
        }
        Vector3 direction = Vector3.zero;
        int num;
        if (!this.player.IsGrounded)
        {
            if (this.player.ReeledState == 0)
            {
                if (this.player.WalledState != 2 && this.player.WalledState != 3)
                {
                    if (this.player.IsEitherHooked)
                    {
                        if (this.player.JumpReelKeyDown)
                        {
                            if (!this.player.IsStationary)
                            {
                                num = 0;
                                direction = this.player.VelocityDirection;
                            }
                            else
                            {
                                num = 2;
                                direction = this.HookDirection();
                            }
                        }
                        else if (!this.player.IsStationary)
                        {
                            num = 1;
                            direction = this.player.HookDirection + this.player.RigidBody.velocity / 10f;
                        }
                        else
                        {
                            num = 2;
                            direction = this.HookDirection();
                        }
                    }
                    else if (!this.player.IsMoving)
                    {
                        num = -1;
                        direction = Vector3.zero;
                    }
                    else
                    {
                        num = -1;
                    }
                }
                else
                {
                    num = 2;
                    direction = this.player.VelocityDirectionXZ;
                }
            }
            else
            {
                num = 2;
                direction = this.HookDirection();
            }
        }
        else
        {
            num = -1;
        }
        if (num == -1)
        {
            int num2 = 0;
            float axisRaw = Input.GetAxisRaw(this.player.axisName.moveFrontBack);
            float axisRaw2 = Input.GetAxisRaw(this.player.axisName.moveLeftRight);
            if (axisRaw2 < 0f)
            {
                num2++;
            }
            else if (axisRaw2 > 0f)
            {
                num2 += 2;
            }
            if (axisRaw < 0f)
            {
                num2 += 4;
            }
            else if (axisRaw > 0f)
            {
                num2 += 8;
            }
            num = 2;
            direction = this.DetermineMoveDirection(num2);
        }
        this.RotatePlayer(num, direction);
    }

    // Token: 0x06000256 RID: 598 RVA: 0x00015F8D File Offset: 0x0001418D
    public void Enable()
    {
    }

    // Token: 0x06000257 RID: 599 RVA: 0x00015F8F File Offset: 0x0001418F
    public void Disable()
    {
    }

    // Token: 0x06000258 RID: 600 RVA: 0x00015F94 File Offset: 0x00014194
    private Vector3 DetermineMoveDirection(int inputIndex)
    {
        Vector3 v;
        switch (inputIndex)
        {
            case 1:
                v = -this.cam.right;
                goto IL_122;
            case 2:
                v = this.cam.right;
                goto IL_122;
            case 4:
                v = -this.cam.forward;
                goto IL_122;
            case 5:
                v = -this.cam.forward - this.cam.right;
                goto IL_122;
            case 6:
                v = -this.cam.forward + this.cam.right;
                goto IL_122;
            case 8:
                v = this.cam.forward;
                goto IL_122;
            case 9:
                v = this.cam.forward - this.cam.right;
                goto IL_122;
            case 10:
                v = this.cam.forward + this.cam.right;
                goto IL_122;
        }
        v = base.transform.forward;
        IL_122:
        return Common.RemoveYComponent(v);
    }

    // Token: 0x06000259 RID: 601 RVA: 0x000160CC File Offset: 0x000142CC
    public void RotatePlayer(int type, Vector3 direction)
    {
        float num = Time.deltaTime;
        this.rotator.LookAt(base.transform.position + direction);
        switch (type)
        {
            case 0:
                num *= 5f;
                this.rotator.eulerAngles = new Vector3(this.rotator.eulerAngles.x, this.rotator.eulerAngles.y, this.HookEulerZ());
                break;
            case 1:
                num *= 5f;
                this.rotator.eulerAngles = new Vector3(this.rotator.eulerAngles.x, this.rotator.eulerAngles.y, this.HookEulerZ());
                break;
            case 2:
                num *= 5f;
                break;
            case 3:
                num *= 5f;
                break;
            default:
                if (this.player.IsGrounded)
                {
                    num *= 20f;
                }
                else
                {
                    num *= 4f;
                }
                break;
        }
        if (type == -1)
        {
            base.transform.rotation = Quaternion.Slerp(base.transform.rotation, new Quaternion(0f, base.transform.rotation.y, 0f, base.transform.rotation.w), num);
        }
        else
        {
            base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.rotator.rotation, num);
        }
    }

    // Token: 0x0600025A RID: 602 RVA: 0x00016274 File Offset: 0x00014474
    private float HookEulerZ()
    {
        Vector3 to = Common.RemoveYComponent(this.player.HookDirection);
        Vector3 from = Common.RemoveYComponent(base.transform.right);
        float num = Vector3.Angle(from, to);
        float num2 = Vector3.Angle(this.player.HookDirection, to);
        float num3 = Vector3.Angle(this.player.VelocityDirectionXZ, to);
        int num4;
        if (num <= 88.5f)
        {
            num4 = 2;
        }
        else if (num < 91.5f)
        {
            num4 = 0;
        }
        else
        {
            num4 = 1;
        }
        num2 /= 2f;
        num3 /= 2f;
        if (num2 > 37.5f)
        {
            num2 = 37.5f;
        }
        if (num3 > 37.5f)
        {
            num3 = 37.5f;
        }
        num2 = 37.5f - num2;
        float num5 = num3 + num2;
        if (num5 > 75f)
        {
            num5 = 75f;
        }
        if (this.player.Height < this.player.limit.takeOffHeight)
        {
            num5 *= 0.33f;
        }
        if (num4 == 1)
        {
            return num5;
        }
        if (num4 == 2)
        {
            return 360f - num5;
        }
        return 0f;
    }

    // Token: 0x0600025B RID: 603 RVA: 0x0001639C File Offset: 0x0001459C
    private Vector3 HookDirection()
    {
        Vector3 result;
        if (this.player.IsDoubleHooked)
        {
            result = this.HookedPointDirection(2);
        }
        else if (this.player.IsLeftHooked)
        {
            result = this.HookedPointDirection(0);
        }
        else
        {
            result = this.HookedPointDirection(1);
        }
        return result;
    }

    // Token: 0x0600025C RID: 604 RVA: 0x000163EC File Offset: 0x000145EC
    private Vector3 HookedPointDirection(int hookType)
    {
        Vector3 position = this.player.transforms.hookOriginLeft.position;
        Vector3 position2 = this.player.transforms.hookOriginRight.position;
        Vector3 leftAnchorPosition = this.player.LeftAnchorPosition;
        Vector3 rightAnchorPosition = this.player.RightAnchorPosition;
        Vector3 a;
        Vector3 b;
        if (hookType == 0)
        {
            a = leftAnchorPosition;
            b = position;
        }
        else if (hookType == 1)
        {
            a = rightAnchorPosition;
            b = position2;
        }
        else
        {
            a = base.transform.position + Vector3.Cross(rightAnchorPosition - leftAnchorPosition, Vector3.up);
            b = position + (position2 - position) * 0.5f;
            if (Mathf.Round(leftAnchorPosition.x - rightAnchorPosition.x) * 100f * 0.01f == 0f && Mathf.Round(leftAnchorPosition.z - rightAnchorPosition.z) * 100f * 0.01f == 0f)
            {
                a = leftAnchorPosition;
                b = position;
            }
        }
        Vector3 vector = a - b;
        return new Vector3(vector.x, 0f, vector.z);
    }

    // Token: 0x04000281 RID: 641
    private Transform cam;

    // Token: 0x04000282 RID: 642
    private Player player;

    // Token: 0x04000283 RID: 643
    private Transform rotator;
}

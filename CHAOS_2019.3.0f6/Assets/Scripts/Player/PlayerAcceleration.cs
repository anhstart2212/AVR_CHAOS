using System;
using UnityEngine;

// Token: 0x0200003B RID: 59
public class PlayerAcceleration
{
    // Token: 0x06000246 RID: 582 RVA: 0x000151DC File Offset: 0x000133DC
    public PlayerAcceleration(float maxAccel, float quadraticConstant, float linearConstant, float maxSpeed)
    {
        this.B = maxAccel;
        this.Q = quadraticConstant * quadraticConstant * maxAccel / (maxSpeed * maxSpeed);
        this.L1 = linearConstant;
        this.L2 = 1f / (linearConstant * maxAccel) - 1f;
        this.M = maxSpeed;
        this.intercept = this.GetIntercept();
    }

    // Token: 0x06000247 RID: 583 RVA: 0x00015238 File Offset: 0x00013438
    private float GetIntercept()
    {
        float q = this.Q;
        float num = this.L1 * this.L2 * this.B / this.M;
        float num2 = this.L1 * this.B - this.B;
        float f = num * num - 4f * q * num2;
        float num3 = Mathf.Sqrt(f);
        return (-num + num3) / (2f * q);
    }

    // Token: 0x06000248 RID: 584 RVA: 0x000152A0 File Offset: 0x000134A0
    public float GetAcceleration(float speed, float angleVelocityAndHook)
    {
        float num;
        if (speed < this.intercept)
        {
            num = -this.Q * (speed * speed) + this.B;
        }
        else
        {
            num = this.L1 * this.B * (this.L2 * speed / this.M + 1f);
        }
        if (num < 1f)
        {
            num = 1f;
        }
        angleVelocityAndHook = angleVelocityAndHook;
        if (angleVelocityAndHook < 0f)
        {
            angleVelocityAndHook = 0f;
        }
        float num2 = this.B * (angleVelocityAndHook / 360f);
        num += num2;
        if (num > this.B)
        {
            num = this.B;
        }
        return num;
    }

    // Token: 0x1700006B RID: 107
    // (get) Token: 0x06000249 RID: 585 RVA: 0x00015342 File Offset: 0x00013542
    public float Intercept
    {
        get
        {
            return this.intercept;
        }
    }

    // Token: 0x04000267 RID: 615
    private float B;

    // Token: 0x04000268 RID: 616
    private float Q;

    // Token: 0x04000269 RID: 617
    private float L1;

    // Token: 0x0400026A RID: 618
    private float L2;

    // Token: 0x0400026B RID: 619
    private float M;

    // Token: 0x0400026C RID: 620
    private float intercept;
}

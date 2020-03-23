using System;
using UnityEngine;

// Token: 0x02000049 RID: 73
public class PlayerTarget
{
    // Token: 0x060002B1 RID: 689 RVA: 0x0001A4D4 File Offset: 0x000186D4
    public PlayerTarget(GameObject targetObj)
    {
        this.gameObject = targetObj;
        this.transform = targetObj.transform;
        this.playerScript = targetObj.GetComponent<Player>();
        this.rigidbody = targetObj.GetComponent<Rigidbody>();
    }

    // Token: 0x060002B2 RID: 690 RVA: 0x0001A507 File Offset: 0x00018707
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    // Token: 0x060002B3 RID: 691 RVA: 0x0001A50F File Offset: 0x0001870F
    public Player GetPlayerScript()
    {
        return this.playerScript;
    }

    // Token: 0x060002B4 RID: 692 RVA: 0x0001A517 File Offset: 0x00018717
    public Transform GetTransform()
    {
        return this.transform;
    }

    // Token: 0x060002B5 RID: 693 RVA: 0x0001A51F File Offset: 0x0001871F
    public Rigidbody GetRigidbody()
    {
        return this.rigidbody;
    }

    // Token: 0x060002B6 RID: 694 RVA: 0x0001A527 File Offset: 0x00018727
    public static bool IsValid(PlayerTarget pt)
    {
        return pt != null && !(pt.GetGameObject() == null);
    }

    //public Vector3 GetFuturePosition(TitanMain self, int framesToPredict)
    //{
    //    Player player = self.ActiveTarget.GetPlayerScript();
    //    float num = this.LateralRatio(self);
    //    float num2 = player.VelocityMagnitudeXZ / player.limit.maxHorizontal;
    //    float num3 = player.VelocityYAbsolute / player.limit.maxVertical;
    //    float num4 = Mathf.Clamp(this.transform.position.y - self.BoneHead.position.y, 0f, self.TitanClass.ReachDistance);
    //    num4 /= self.TitanClass.ReachDistance;
    //    float num5 = Vector3.Angle(-player.VelocityDirectionXZ, Common.RemoveYComponent(self.transform.forward));
    //    num5 = Common.GetRatio(num5, 0f, 45f);
    //    int num6 = framesToPredict + (int)(10f * num) + (int)(5f * num2) + (int)(3f * num3) + (int)(3f * num4) + (int)(12f * num5);
    //    return this.transform.position + this.rigidbody.velocity * Time.fixedDeltaTime * (float)num6;
    //}

    // Token: 0x060002B8 RID: 696 RVA: 0x0001A673 File Offset: 0x00018873
    private float ApproachAngle(Vector3 forward, Vector3 velocity)
    {
        forward = Common.RemoveYComponent(forward);
        velocity = Common.RemoveYComponent(velocity);
        return Vector3.Angle(velocity, forward);
    }

    //private float LateralRatio(TitanMain self)
    //{
    //    Vector3 vector = Common.GetVectorTo(self.transform, this.transform);
    //    Vector3 vector2 = self.transform.forward;
    //    vector = Common.RemoveYComponent(vector);
    //    vector2 = Common.RemoveYComponent(vector2);
    //    Vector3 b = Vector3.Project(vector, vector2);
    //    float num = (vector - b).magnitude;
    //    float reachDistance = self.TitanClass.ReachDistance;
    //    if (num > reachDistance)
    //    {
    //        num = reachDistance;
    //    }
    //    return 1f - num / reachDistance;
    //}

    // Token: 0x04000317 RID: 791
    private GameObject gameObject;

    // Token: 0x04000318 RID: 792
    private Player playerScript;

    // Token: 0x04000319 RID: 793
    private Transform transform;

    // Token: 0x0400031A RID: 794
    private Rigidbody rigidbody;

    // Token: 0x0400031B RID: 795
    public Vector3 direction;

    // Token: 0x0400031C RID: 796
    public float sqrDistance;

    // Token: 0x0400031D RID: 797
    public float distance;

    // Token: 0x0400031E RID: 798
    public float aggro;
}

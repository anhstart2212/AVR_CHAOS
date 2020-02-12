using System;
using UnityEngine;

// Token: 0x0200008B RID: 139
public static class Common
{
    // Token: 0x06000515 RID: 1301 RVA: 0x0002B1FC File Offset: 0x000293FC
    public static bool CurrentStateName(Animator anim, int layer, string name)
    {
        return anim.GetCurrentAnimatorStateInfo(layer).IsName(name);
    }

    // Token: 0x06000516 RID: 1302 RVA: 0x0002B21C File Offset: 0x0002941C
    public static bool CurrentStateTag(Animator anim, int layer, string tag)
    {
        return anim.GetCurrentAnimatorStateInfo(layer).IsTag(tag);
    }

    // Token: 0x06000517 RID: 1303 RVA: 0x0002B23C File Offset: 0x0002943C
    public static bool NextStateName(Animator anim, int layer, string name)
    {
        return anim.GetNextAnimatorStateInfo(layer).IsName(name);
    }

    // Token: 0x06000518 RID: 1304 RVA: 0x0002B25C File Offset: 0x0002945C
    public static bool NextStateTag(Animator anim, int layer, string tag)
    {
        return anim.GetNextAnimatorStateInfo(layer).IsTag(tag);
    }

    // Token: 0x06000519 RID: 1305 RVA: 0x0002B279 File Offset: 0x00029479
    public static bool CurrentOrNextStateName(Animator anim, int layer, string name)
    {
        return Common.CurrentStateName(anim, layer, name) || Common.NextStateName(anim, layer, name);
    }

    // Token: 0x0600051A RID: 1306 RVA: 0x0002B293 File Offset: 0x00029493
    public static bool CurrentOrNextStateTag(Animator anim, int layer, string tag)
    {
        return Common.CurrentStateTag(anim, layer, tag) || Common.NextStateTag(anim, layer, tag);
    }

    // Token: 0x0600051B RID: 1307 RVA: 0x0002B2AD File Offset: 0x000294AD
    public static Vector3 RemoveYComponent(Vector3 v)
    {
        return new Vector3(v.x, 0f, v.z);
    }

    // Token: 0x0600051C RID: 1308 RVA: 0x0002B2C7 File Offset: 0x000294C7
    public static Vector3 RemoveXComponent(Vector3 v)
    {
        return new Vector3(0f, v.y, v.z);
    }

    // Token: 0x0600051D RID: 1309 RVA: 0x0002B2E1 File Offset: 0x000294E1
    public static Vector3 RemoveZComponent(Vector3 v)
    {
        return new Vector3(v.x, v.y, 0f);
    }

    // Token: 0x0600051E RID: 1310 RVA: 0x0002B2FB File Offset: 0x000294FB
    public static Vector3 GetVectorTo(Transform tail, Transform head)
    {
        return head.position - tail.position;
    }

    // Token: 0x0600051F RID: 1311 RVA: 0x0002B30E File Offset: 0x0002950E
    public static Vector3 GetVectorTo(Vector3 tail, Vector3 head)
    {
        return head - tail;
    }

    // Token: 0x06000520 RID: 1312 RVA: 0x0002B317 File Offset: 0x00029517
    public static bool IsOnTheLeft(Vector3 rightDirection, Vector3 targetDirection)
    {
        return Vector3.Angle(Common.RemoveYComponent(rightDirection), Common.RemoveYComponent(targetDirection)) >= 90f;
    }

    // Token: 0x06000521 RID: 1313 RVA: 0x0002B334 File Offset: 0x00029534
    public static bool CloserTo(Vector3 origin, Vector3 pointA, Vector3 pointB)
    {
        float num = Vector3.SqrMagnitude(origin - pointA);
        float num2 = Vector3.SqrMagnitude(origin - pointB);
        return num < num2;
    }

    // Token: 0x06000522 RID: 1314 RVA: 0x0002B35F File Offset: 0x0002955F
    public static Transform GetRootObject(Transform current)
    {
        while (current.parent != null)
        {
            current = current.parent;
        }
        return current;
    }

    // Token: 0x06000523 RID: 1315 RVA: 0x0002B380 File Offset: 0x00029580
    public static float GetRatio(float value, float lower, float upper)
    {
        value = Mathf.Clamp(value, lower, upper);
        value -= lower;
        upper -= lower;
        return Mathf.Clamp01(value / upper);
    }

    // Token: 0x06000524 RID: 1316 RVA: 0x0002B39E File Offset: 0x0002959E
    public static void SetCursor(bool locked)
    {
        Cursor.lockState = ((!locked) ? CursorLockMode.Confined : CursorLockMode.Locked);
        Cursor.visible = !locked;
    }

    // Token: 0x06000525 RID: 1317 RVA: 0x0002B3C4 File Offset: 0x000295C4
    public static void DrawCube(Vector3 center, float length, Color color)
    {
        float d = length / 2f;
        Vector3 vector = center + Vector3.left * d + Vector3.up * d - Vector3.forward * d;
        Vector3 vector2 = center + Vector3.right * d + Vector3.up * d - Vector3.forward * d;
        Vector3 vector3 = center + Vector3.left * d + Vector3.down * d - Vector3.forward * d;
        Vector3 vector4 = center + Vector3.right * d + Vector3.down * d - Vector3.forward * d;
        Vector3 vector5 = vector + Vector3.forward * length;
        Vector3 vector6 = vector2 + Vector3.forward * length;
        Vector3 vector7 = vector3 + Vector3.forward * length;
        Vector3 end = vector4 + Vector3.forward * length;
        Debug.DrawLine(vector, vector5, color);
        Debug.DrawLine(vector2, vector6, color);
        Debug.DrawLine(vector3, vector7, color);
        Debug.DrawLine(vector4, end, color);
        Debug.DrawLine(vector, vector2, color);
        Debug.DrawLine(vector5, vector6, color);
        Debug.DrawLine(vector3, vector4, color);
        Debug.DrawLine(vector7, end, color);
        Debug.DrawLine(vector, vector3, color);
        Debug.DrawLine(vector5, vector7, color);
        Debug.DrawLine(vector2, vector4, color);
        Debug.DrawLine(vector6, end, color);
    }

    // Token: 0x06000526 RID: 1318 RVA: 0x0002B55E File Offset: 0x0002975E
    public static void DrawRay(Ray Ray, float distance, Color color)
    {
        Debug.DrawRay(Ray.origin, Ray.direction * distance, color);
    }

    // Token: 0x0400053F RID: 1343
    public static int layerObstacle = 1024;

    // Token: 0x04000540 RID: 1344
    public static int layerGround = 512;

    // Token: 0x04000541 RID: 1345
    public static int layerTitan = 2048;

    // Token: 0x04000542 RID: 1346
    public static int layerPlayer = 256;

    // Token: 0x04000543 RID: 1347
    public static int layerBoundary = 4096;

    // Token: 0x04000544 RID: 1348
    public static int layerNoHook = 8192;

    // Token: 0x04000545 RID: 1349
    public static int layerOGT = 3584;

    // Token: 0x04000546 RID: 1350
    public static CardinalDirection direction = new CardinalDirection();

    // Token: 0x04000547 RID: 1351
    public static YieldInstruction yieldFixedUpdate = new WaitForFixedUpdate();

    // Token: 0x04000548 RID: 1352
    public static YieldInstruction yieldLateUpdate = new WaitForEndOfFrame();
}

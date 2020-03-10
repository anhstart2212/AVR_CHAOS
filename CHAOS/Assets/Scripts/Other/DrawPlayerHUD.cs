using System;
using UnityEngine;

// Token: 0x0200002D RID: 45
public class DrawPlayerHUD : MonoBehaviour
{
    // Token: 0x06000117 RID: 279 RVA: 0x0000B9E3 File Offset: 0x00009BE3
    private void Start()
    {
        this.launcherScripts = base.GetComponentsInChildren<LauncherControl>();
        this.player = base.GetComponent<Player>();
    }

    // Token: 0x06000118 RID: 280 RVA: 0x0000B9FD File Offset: 0x00009BFD
    public void Disable()
    {
        this.skipDraw = true;
    }

    // Token: 0x06000119 RID: 281 RVA: 0x0000BA06 File Offset: 0x00009C06
    public void Enable()
    {
        this.skipDraw = false;
    }

    // Token: 0x0600011A RID: 282 RVA: 0x0000BA0F File Offset: 0x00009C0F
    private void Update()
    {
    }

    // Token: 0x0600011B RID: 283 RVA: 0x0000BA14 File Offset: 0x00009C14
    private void LateUpdate()
    {
        if (this.player.transforms.playerCamera == null)
        {
            return;
        }

        this.rightInRange = this.launcherScripts[0].inRange;
        this.leftInRange = this.launcherScripts[1].inRange;
        this.rightTargetPosition = this.launcherScripts[0].targetPoint;
        this.leftTargetPosition = this.launcherScripts[1].targetPoint;
        this.rightAcquired = this.launcherScripts[0].targetAcquired;
        this.leftAcquired = this.launcherScripts[1].targetAcquired;
        this.rightTargetScreenPosition = Camera.main.WorldToScreenPoint(this.rightTargetPosition);
        this.leftTargetScreenPosition = Camera.main.WorldToScreenPoint(this.leftTargetPosition);
        this.activeLeftCrosshair = ((!this.leftInRange) ? this.leftRedCrosshair : this.leftGreenCrosshair);
        this.activeRightCrosshair = ((!this.rightInRange) ? this.rightRedCrosshair : this.rightGreenCrosshair);
        this.activeCenterCrosshair = ((!this.player.TitanAimLock) ? this.centerCrosshair : this.centerCrosshairLocked);
        Vector3 normalized = (this.leftTargetPosition - this.player.transforms.playerCamera.position).normalized;
        normalized = new Vector3(normalized.x, 0f, normalized.z);
        Vector3 normalized2 = (this.rightTargetPosition - this.player.transforms.playerCamera.position).normalized;
        normalized2 = new Vector3(normalized2.x, 0f, normalized2.z);
        Vector3 to = new Vector3(this.player.transforms.playerCamera.forward.x, 0f, this.player.transforms.playerCamera.forward.z);
        float num = Vector3.Angle(normalized, to);
        float num2 = Vector3.Angle(normalized2, to);
        float num3 = 90f;
        this.leftBehindCamera = (num >= num3);
        this.rightBehindCamera = (num2 >= num3);
        this.leftTargetDistance = (this.leftTargetPosition - base.transform.position).magnitude;
        this.rightTargetDistance = (this.rightTargetPosition - base.transform.position).magnitude;
        float num4 = Common.GetRatio(this.leftTargetDistance, 5f, this.player.MaxHookDistance * 0.75f);
        num4 = Mathf.Round(num4 * 0.5f * 100f) * 0.01f;
        this.leftTargetScale = this.crosshairScale - this.crosshairScale * num4;
        num4 = Common.GetRatio(this.rightTargetDistance, 5f, this.player.MaxHookDistance * 0.75f);
        num4 = Mathf.Round(num4 * 0.5f * 100f) * 0.01f;
        this.rightTargetScale = this.crosshairScale - this.crosshairScale * num4;
    }

    // Token: 0x0600011C RID: 284 RVA: 0x0000BD34 File Offset: 0x00009F34
    public void OnGUI()
    {
        if (!m_IsDraw)
        {
            return;
        }

        if (this.skipDraw)
        {
            return;
        }
        if (this.activeCenterCrosshair == null)
        {
            return;
        }
        GUI.DrawTexture(new Rect(((float)Screen.width - (float)this.activeCenterCrosshair.width * this.crosshairScale) / 2f, ((float)Screen.height - (float)this.activeCenterCrosshair.height * this.crosshairScale) / 2f, (float)this.activeCenterCrosshair.width * this.crosshairScale, (float)this.activeCenterCrosshair.height * this.crosshairScale), this.activeCenterCrosshair);
        if (this.activeLeftCrosshair == null || this.activeRightCrosshair == null)
        {
            return;
        }
        if (!this.leftBehindCamera && this.leftAcquired && this.leftTargetScreenPosition.x >= 0f && this.leftTargetScreenPosition.x <= (float)Screen.width && this.leftTargetScreenPosition.y >= 0f && this.leftTargetScreenPosition.y <= (float)Screen.height)
        {
            GUI.DrawTexture(new Rect(this.leftTargetScreenPosition.x - (float)this.activeLeftCrosshair.width * this.leftTargetScale, (float)Screen.height - this.leftTargetScreenPosition.y - (float)this.activeLeftCrosshair.height * this.leftTargetScale / 2f, (float)this.activeLeftCrosshair.width * this.leftTargetScale, (float)this.activeLeftCrosshair.height * this.leftTargetScale), this.activeLeftCrosshair);
        }
        if (!this.rightBehindCamera && this.rightAcquired && this.rightTargetScreenPosition.x >= 0f && this.rightTargetScreenPosition.x <= (float)Screen.width && this.rightTargetScreenPosition.y >= 0f && this.rightTargetScreenPosition.y <= (float)Screen.height)
        {
            GUI.DrawTexture(new Rect(this.rightTargetScreenPosition.x, (float)Screen.height - this.rightTargetScreenPosition.y - (float)this.activeRightCrosshair.height * this.rightTargetScale / 2f, (float)this.activeRightCrosshair.width * this.rightTargetScale, (float)this.activeRightCrosshair.height * this.rightTargetScale), this.activeRightCrosshair);
        }
    }

    public void DrawOwnerHookTarget(bool isDraw)
    {
        m_IsDraw = isDraw;
    }

    // Token: 0x04000163 RID: 355
    public Texture2D leftGreenCrosshair;

    // Token: 0x04000164 RID: 356
    public Texture2D rightGreenCrosshair;

    // Token: 0x04000165 RID: 357
    public Texture2D leftRedCrosshair;

    // Token: 0x04000166 RID: 358
    public Texture2D rightRedCrosshair;

    // Token: 0x04000167 RID: 359
    public Texture2D centerCrosshair;

    // Token: 0x04000168 RID: 360
    public Texture2D centerCrosshairLocked;

    // Token: 0x04000169 RID: 361
    private Texture2D activeLeftCrosshair;

    // Token: 0x0400016A RID: 362
    private Texture2D activeRightCrosshair;

    // Token: 0x0400016B RID: 363
    private Texture2D activeCenterCrosshair;

    // Token: 0x0400016C RID: 364
    public float crosshairScale;

    // Token: 0x0400016D RID: 365
    private float leftTargetScale;

    // Token: 0x0400016E RID: 366
    private float rightTargetScale;

    // Token: 0x0400016F RID: 367
    private Vector3 leftTargetScreenPosition;

    // Token: 0x04000170 RID: 368
    private Vector3 rightTargetScreenPosition;

    // Token: 0x04000171 RID: 369
    private Player player;

    // Token: 0x04000172 RID: 370
    private LauncherControl[] launcherScripts = new LauncherControl[2];

    // Token: 0x04000173 RID: 371
    private bool leftInRange;

    // Token: 0x04000174 RID: 372
    private bool rightInRange;

    // Token: 0x04000175 RID: 373
    private bool leftBehindCamera;

    // Token: 0x04000176 RID: 374
    private bool rightBehindCamera;

    // Token: 0x04000177 RID: 375
    private Vector3 leftTargetPosition;

    // Token: 0x04000178 RID: 376
    private Vector3 rightTargetPosition;

    // Token: 0x04000179 RID: 377
    private float leftTargetDistance;

    // Token: 0x0400017A RID: 378
    private float rightTargetDistance;

    // Token: 0x0400017B RID: 379
    private bool leftAcquired;

    // Token: 0x0400017C RID: 380
    private bool rightAcquired;

    // Token: 0x0400017D RID: 381
    private Vector3 camDirection;

    // Token: 0x0400017E RID: 382
    public bool skipDraw;

    private bool m_IsDraw;
}

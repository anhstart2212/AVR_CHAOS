using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class CameraControl : MonoBehaviour
{
    // Token: 0x060000FB RID: 251 RVA: 0x00009FA0 File Offset: 0x000081A0
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(base.transform.position, this.cameraTargetPosition);
        //this.playerTransform.position.y = this.cameraTrailPosition.y;
        //this.playerTransform.position = new Vector3(playerTransform.position.x, this.cameraTrailPosition.y, playerTransform.position.z);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(base.transform.position, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.cameraTrailPosition, 0.1f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(this.cameraTargetPosition, 0.1f);
    }

    // Token: 0x060000FC RID: 252 RVA: 0x0000A049 File Offset: 0x00008249
    private void Awake()
    {
        this.newFollowDistance = this.defaultFollowDistance;
        this.originalFollowDistance = this.defaultFollowDistance;
    }

    // Token: 0x060000FD RID: 253 RVA: 0x0000A064 File Offset: 0x00008264
    private void Start()
    {
        this.playerTransform = base.transform.parent;
        this.playerScript = this.playerTransform.GetComponent<Player>();
        this.myCamera = base.gameObject.GetComponent<Camera>();
        base.transform.parent = null;
        this.defaultFOV = this.myCamera.fieldOfView;
        this.camTargOffX = this.maxOffsetX;
        this.camTargOffY = this.maxOffsetY;
        base.StartCoroutine(this.CameraMotionLag());
    }

    // Token: 0x060000FE RID: 254 RVA: 0x0000A0E6 File Offset: 0x000082E6
    public void SetFollowDistance(float newDist)
    {
        this.defaultFollowDistance = newDist;
    }

    // Token: 0x060000FF RID: 255 RVA: 0x0000A0EF File Offset: 0x000082EF
    public void ResetFollowDistance()
    {
        this.defaultFollowDistance = this.originalFollowDistance;
    }

    // Token: 0x06000100 RID: 256 RVA: 0x0000A100 File Offset: 0x00008300
    public void ResetToDefault()
    {
        this.ResetFollowDistance();
        this.myCamera.fieldOfView = this.defaultFOV;
        this.newFollowDistance = this.originalFollowDistance;
        this.cameraTargetPosition = this.playerTransform.position + new Vector3(this.maxOffsetX, this.maxOffsetY, 0f);
        base.transform.position = this.cameraTargetPosition - base.transform.forward * this.originalFollowDistance;
        this.cameraTrailPosition = base.transform.position;
        this.resetTrails = true;
    }

    // Token: 0x06000101 RID: 257 RVA: 0x0000A1A0 File Offset: 0x000083A0
    private void Update()
    {
        if (this.playerTransform.gameObject == null)
        {
            return;
        }
        Vector3 a = this.playerTransform.position + base.transform.up * this.camTargOffY + base.transform.right * this.camTargOffX;
        this.cameraTargetPosition = Vector3.Lerp(a, this.cameraTrailPosition, 0.1f);
        int layerMask = (!this.playerScript.IsGrabbed) ? Common.layerOGT : (Common.layerObstacle | Common.layerGround);
        Vector3 vector = this.cameraTargetPosition;
        Vector3 a2 = vector;
        bool flag = false;
        Vector3 vector2 = new Vector3(this.playerTransform.position.x, vector.y, this.playerTransform.position.z);
        this.SetCamDirections();
        for (int i = 0; i < this.csvs.Length; i++)
        {
            this.csvPositions[i] = vector2 + this.csvs[i].vector * 2f;
        }
        float a3 = this.hitDistance;
        RaycastHit raycastHit;
        foreach (CameraControl.CSV csv in this.csvs)
        {
            Vector3 end = vector2 + csv.vector * 2f;
            if (Physics.Linecast(vector2, end, out raycastHit, layerMask))
            {
                flag = true;
                float num = csv.magnitude * 2f - raycastHit.distance;
                if (num > this.hitDistance)
                {
                    this.hitDistance = num;
                }
            }
        }
        if (flag && Physics.Linecast(vector2, vector2 - this.csvs[0].vector * 2f, out raycastHit, layerMask))
        {
            this.hitDistance = this.maxOffsetX;
        }
        float t = 0.1f;
        if (!flag)
        {
            this.hitDistance = 0f;
            t = 0.05f;
        }
        this.hitDistance = Mathf.Lerp(a3, this.hitDistance, t);
        a2 -= this.csvs[0].direction * this.hitDistance;
        this.cameraTargetPosition = a2;
        this.RotateCamera(this.cameraTargetPosition);
        this.newFollowDistance = this.NewCameraDistance(0.08f);
        this.myCamera.fieldOfView = this.NewCameraFoV(0.08f);
        vector = this.cameraTargetPosition - base.transform.forward * this.newFollowDistance;
        if (Physics.Linecast(this.cameraTargetPosition, vector, out raycastHit, layerMask))
        {
            this.newFollowDistance = raycastHit.distance - 0.01f;
        }
        Vector3 vector3 = this.cameraTargetPosition - base.transform.forward * this.newFollowDistance;
        if (this.isScreenShaking)
        {
            vector3 = Vector3.Lerp(vector3, vector3 + UnityEngine.Random.insideUnitSphere * 1.5f * this.screenShakeFactor, 0.08f);
        }
        if (this.isGrabZooming)
        {
            vector3 = Vector3.Lerp(base.transform.position, vector3, 0.075f);
        }
        base.transform.position = vector3;
    }

    // Token: 0x06000102 RID: 258 RVA: 0x0000A510 File Offset: 0x00008710
    private CameraControl.CSV[] SetCamDirections()
    {
        Vector3 normalized = Common.RemoveYComponent(-base.transform.forward).normalized;
        Vector3 normalized2 = Common.RemoveYComponent(base.transform.right).normalized;
        float magnitude = Mathf.Sqrt(Mathf.Pow(Mathf.Tan(-0.5235988f), 2f) + 1f);
        float magnitude2 = Mathf.Sqrt(Mathf.Pow(Mathf.Tan(0.5235988f), 2f) + 1f);
        Vector3 direction = normalized2;
        Vector3 direction2 = Vector3.RotateTowards(normalized2, normalized, -0.5235988f, 0f);
        Vector3 direction3 = Vector3.RotateTowards(normalized2, normalized, 0.5235988f, 0f);
        int num = 0;
        this.csvs[num++].Set(direction, 1f);
        this.csvs[num++].Set(direction2, magnitude);
        this.csvs[num++].Set(direction3, magnitude2);
        return this.csvs;
    }

    // Token: 0x06000103 RID: 259 RVA: 0x0000A610 File Offset: 0x00008810
    private float NewCameraDistance(float LERP)
    {
        float run = this.playerScript.speed.run;
        float maxHorizontal = this.playerScript.limit.maxHorizontal;
        float num = this.defaultFollowDistance;
        float camMaxFollowDist = this.playerScript.limit.camMaxFollowDist;
        float num2 = this.playerScript.VelocityMagnitudeXZ + this.playerScript.VelocityYAbsolute * 0.5f;
        float num3 = (0.75f * camMaxFollowDist - num) / (maxHorizontal / 3f - run);
        float num4 = 3f * (camMaxFollowDist - 0.75f * camMaxFollowDist) / maxHorizontal;
        float b;
        if (num2 < run)
        {
            b = num;
        }
        else if (num2 < maxHorizontal / 3f)
        {
            b = num3 * (num2 - run) + num;
        }
        else
        {
            b = num4 * (num2 - maxHorizontal / 3f) + 0.75f * camMaxFollowDist;
        }
        return Mathf.Lerp(this.newFollowDistance, b, LERP);
    }

    // Token: 0x06000104 RID: 260 RVA: 0x0000A6F8 File Offset: 0x000088F8
    private float NewCameraFoV(float LERP)
    {
        float run = this.playerScript.speed.run;
        float num = this.playerScript.limit.maxHorizontal / 2f + run;
        float num2 = Mathf.Clamp((this.playerScript.VelocityMagnitude - run) / num, 0f, 1f);
        float b = this.defaultFOV + 10f * num2;
        return Mathf.Lerp(this.myCamera.fieldOfView, b, LERP);
    }

    // Token: 0x06000105 RID: 261 RVA: 0x0000A774 File Offset: 0x00008974
    private void RotateCamera(Vector3 targetPosition)
    {
        float x = base.transform.rotation.eulerAngles.x;
        float num = this.playerScript.speed.look * 0.2f * Time.timeScale;
        float num2 = 1f / Time.fixedDeltaTime;
        float angle = this.playerScript.MouseAxisX * num * num2 * Time.deltaTime;
        float angle2 = -this.playerScript.MouseAxisY * num * num2 * Time.deltaTime;
        base.transform.RotateAround(targetPosition, Vector3.up, angle);
        if ((x >= 0f && x < this.lowerBound) || (x > this.upperBound && x <= 360f))
        {
            base.transform.RotateAround(targetPosition, base.transform.right, angle2);
        }
        else if (x >= this.lowerBound && x < 180f && this.playerScript.MouseAxisY > 0f)
        {
            base.transform.RotateAround(targetPosition, base.transform.right, angle2);
        }
        else if (x <= this.upperBound && x > 180f && this.playerScript.MouseAxisY < 0f)
        {
            base.transform.RotateAround(targetPosition, base.transform.right, angle2);
        }
        else if (x < 0f)
        {
            base.transform.RotateAround(targetPosition, base.transform.right, angle2);
        }
        base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y, 0f);
    }

    // Token: 0x06000106 RID: 262 RVA: 0x0000A94C File Offset: 0x00008B4C
    public void StartScreenShake(float initialIntensity)
    {
        if (this.isScreenShaking)
        {
            base.StopCoroutine(this.screenShake);
        }
        this.screenShake = this.ScreenShake(initialIntensity);
        base.StartCoroutine(this.screenShake);
    }

    // Token: 0x06000107 RID: 263 RVA: 0x0000A97F File Offset: 0x00008B7F
    public void StartGrabZoom()
    {
        if (this.isGrabZooming)
        {
            return;
        }
        this.grabZoom = this.GrabZoom();
        base.StartCoroutine(this.grabZoom);
    }

    // Token: 0x06000108 RID: 264 RVA: 0x0000A9A8 File Offset: 0x00008BA8
    private IEnumerator ScreenShake(float initialIntensity)
    {
        this.isScreenShaking = true;
        float fps = 1f / Time.fixedDeltaTime;
        float framesToRun = 0.4f * fps;
        int count = 0;
        float shakeDec = initialIntensity / framesToRun;
        this.screenShakeFactor = initialIntensity;
        while ((float)count < framesToRun)
        {
            this.screenShakeFactor -= shakeDec;
            count++;
            yield return null;
        }
        this.screenShakeFactor = 0f;
        this.isScreenShaking = false;
        yield break;
    }

    // Token: 0x06000109 RID: 265 RVA: 0x0000A9CC File Offset: 0x00008BCC
    private IEnumerator GrabZoom()
    {
        this.isGrabZooming = true;
        float currDist = this.defaultFollowDistance;
        float step = (currDist - 4f) * (Time.fixedDeltaTime / 6f);
        while (this.playerScript.IsGrabbed)
        {
            currDist = Mathf.Clamp(currDist - step, 4f, currDist);
            this.SetFollowDistance(currDist);
            yield return null;
        }
        this.ResetFollowDistance();
        this.isGrabZooming = false;
        yield break;
    }

    // Token: 0x0600010A RID: 266 RVA: 0x0000A9E8 File Offset: 0x00008BE8
    private IEnumerator CameraMotionLag()
    {
        int counter = 0;
        int i = 0;
        int j = -1;
        Vector3 kDir = Vector3.zero;
        Vector3[] pastPositions = new Vector3[this.playerScript.limit.camMaxTrailDist];
        this.cameraTrailPosition = this.playerTransform.position + base.transform.up * this.camTargOffY + base.transform.right * this.camTargOffX;
        for (int k = 0; k < pastPositions.Length; k++)
        {
            pastPositions[k] = Vector3.zero;
        }
        for (; ; )
        {
            if (this.resetTrails)
            {
                for (int l = 0; l < pastPositions.Length; l++)
                {
                    pastPositions[l] = Vector3.zero;
                }
                kDir = Vector3.zero;
                j = -1;
                i = 0;
                counter = 0;
                this.cameraTrailPosition = this.playerTransform.position + base.transform.up * this.camTargOffY + base.transform.right * this.camTargOffX;
                this.resetTrails = false;
            }
            pastPositions[i] = this.playerTransform.position + base.transform.up * this.camTargOffY + base.transform.right * this.camTargOffX;
            i++;
            if (i >= pastPositions.Length)
            {
                i = 0;
                counter = -1;
                j = 0;
            }
            if (j > -1)
            {
                if (j == pastPositions.Length - 1)
                {
                    kDir = pastPositions[0] - pastPositions[j];
                }
                else
                {
                    kDir = pastPositions[j + 1] - pastPositions[j];
                }
                float t = 0.1f;
                Vector3 b;
                if (this.playerScript.IsGrounded)
                {
                    b = this.playerTransform.position + base.transform.up * this.camTargOffY + base.transform.right * this.camTargOffX;
                }
                else
                {
                    b = pastPositions[j] + kDir;
                }
                this.cameraTrailPosition = Vector3.Lerp(this.cameraTrailPosition, b, t);
                j++;
            }
            yield return Common.yieldFixedUpdate;
            counter++;
        }
        yield break;
    }

    // Token: 0x17000015 RID: 21
    // (get) Token: 0x0600010B RID: 267 RVA: 0x0000AA03 File Offset: 0x00008C03
    public Vector3 CameraTargetPosition
    {
        get
        {
            return this.cameraTargetPosition;
        }
    }

    // Token: 0x0400012D RID: 301
    private Transform playerTransform;

    // Token: 0x0400012E RID: 302
    private Camera myCamera;

    // Token: 0x0400012F RID: 303
    private Player playerScript;

    // Token: 0x04000130 RID: 304
    private RaycastHit contact;

    // Token: 0x04000131 RID: 305
    public float lowerBound;

    // Token: 0x04000132 RID: 306
    public float upperBound;

    // Token: 0x04000133 RID: 307
    public float defaultFollowDistance;

    // Token: 0x04000134 RID: 308
    private float originalFollowDistance;

    // Token: 0x04000135 RID: 309
    private float newFollowDistance;

    // Token: 0x04000136 RID: 310
    private float defaultFOV;

    // Token: 0x04000137 RID: 311
    private bool resetTrails;

    // Token: 0x04000138 RID: 312
    private Vector3 cameraTrailPosition;

    // Token: 0x04000139 RID: 313
    private Vector3 cameraTargetPosition;

    // Token: 0x0400013A RID: 314
    public float maxOffsetX = 0.55f;

    // Token: 0x0400013B RID: 315
    public float maxOffsetY = 0.4f;

    // Token: 0x0400013C RID: 316
    private float camTargOffX;

    // Token: 0x0400013D RID: 317
    private float camTargOffY;

    // Token: 0x0400013E RID: 318
    private float hitDistance;

    // Token: 0x0400013F RID: 319
    private Vector3[] csvPositions = new Vector3[3];

    // Token: 0x04000140 RID: 320
    private CameraControl.CSV[] csvs = new CameraControl.CSV[]
    {
        new CameraControl.CSV(),
        new CameraControl.CSV(),
        new CameraControl.CSV()
    };

    // Token: 0x04000141 RID: 321
    private IEnumerator screenShake;

    // Token: 0x04000142 RID: 322
    private bool isScreenShaking;

    // Token: 0x04000143 RID: 323
    private float screenShakeFactor;

    // Token: 0x04000144 RID: 324
    private IEnumerator grabZoom;

    // Token: 0x04000145 RID: 325
    private bool isGrabZooming;

    // Token: 0x0200002B RID: 43
    private class CSV
    {
        // Token: 0x0600010C RID: 268 RVA: 0x0000AA0B File Offset: 0x00008C0B
        public CSV()
        {
            this.direction = Vector3.zero;
            this.magnitude = 0f;
            this.vector = Vector3.zero;
        }

        // Token: 0x0600010D RID: 269 RVA: 0x0000AA34 File Offset: 0x00008C34
        public void Set(Vector3 direction, float magnitude)
        {
            this.direction = direction;
            this.magnitude = magnitude;
            this.vector = direction * magnitude;
        }

        // Token: 0x04000146 RID: 326
        public Vector3 direction;

        // Token: 0x04000147 RID: 327
        public float magnitude;

        // Token: 0x04000148 RID: 328
        public Vector3 vector;
    }
}

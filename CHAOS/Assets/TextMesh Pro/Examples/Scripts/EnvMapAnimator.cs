using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Token: 0x020000AE RID: 174
public class EnvMapAnimator : MonoBehaviour
{
    // Token: 0x060005AE RID: 1454 RVA: 0x0003076B File Offset: 0x0002E96B
    private void Awake()
    {
        this.m_textMeshPro = base.GetComponent<TMP_Text>();
        this.m_material = this.m_textMeshPro.fontSharedMaterial;
    }

    // Token: 0x060005AF RID: 1455 RVA: 0x0003078C File Offset: 0x0002E98C
    private IEnumerator Start()
    {
        Matrix4x4 matrix = default(Matrix4x4);
        for (; ; )
        {
            matrix.SetTRS(Vector3.zero, Quaternion.Euler(Time.time * this.RotationSpeeds.x, Time.time * this.RotationSpeeds.y, Time.time * this.RotationSpeeds.z), Vector3.one);
            this.m_material.SetMatrix("_EnvMatrix", matrix);
            yield return null;
        }
        yield break;
    }

    // Token: 0x0400065A RID: 1626
    public Vector3 RotationSpeeds;

    // Token: 0x0400065B RID: 1627
    private TMP_Text m_textMeshPro;

    // Token: 0x0400065C RID: 1628
    private Material m_material;
}

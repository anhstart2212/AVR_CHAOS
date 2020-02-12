using System;
using UnityEngine;

// Token: 0x0200008D RID: 141
[Serializable]
public class CardinalDirection
{
    // Token: 0x0600052A RID: 1322 RVA: 0x0002B654 File Offset: 0x00029854
    public CardinalDirection()
    {
        this.elements = 8;
        this.directionsArray = new Vector3[8];
        this.N = Vector3.forward;
        this.S = Vector3.back;
        this.W = Vector3.left;
        this.E = Vector3.right;
        this.NE = (Vector3.forward + Vector3.right).normalized;
        this.NW = (Vector3.forward + Vector3.left).normalized;
        this.SE = (Vector3.back + Vector3.right).normalized;
        this.SW = (Vector3.back + Vector3.left).normalized;
        this.directionsArray[0] = this.N;
        this.directionsArray[1] = this.S;
        this.directionsArray[2] = this.W;
        this.directionsArray[3] = this.E;
        this.directionsArray[4] = this.NE;
        this.directionsArray[5] = this.NW;
        this.directionsArray[6] = this.SE;
        this.directionsArray[7] = this.SW;
    }

    // Token: 0x0400054B RID: 1355
    public Vector3 N;

    // Token: 0x0400054C RID: 1356
    public Vector3 S;

    // Token: 0x0400054D RID: 1357
    public Vector3 W;

    // Token: 0x0400054E RID: 1358
    public Vector3 E;

    // Token: 0x0400054F RID: 1359
    public Vector3 NW;

    // Token: 0x04000550 RID: 1360
    public Vector3 NE;

    // Token: 0x04000551 RID: 1361
    public Vector3 SW;

    // Token: 0x04000552 RID: 1362
    public Vector3 SE;

    // Token: 0x04000553 RID: 1363
    public Vector3[] directionsArray;

    // Token: 0x04000554 RID: 1364
    public int elements;
}

using System;
using UnityEngine;

// Token: 0x02000016 RID: 22
public class ObstacleGenerator : MonoBehaviour
{
    // Token: 0x06000091 RID: 145 RVA: 0x000061D0 File Offset: 0x000043D0
    [ContextMenu("GenerateGrid")]
    public void GenerateGrid()
    {
        this.rows = (int)(this.gridSize.y / ObstacleGenerator.unit);
        this.cols = (int)(this.gridSize.x / ObstacleGenerator.unit);
        int num = 0;
        this.grid = new ObstacleGenerator.GridCell[this.rows, this.cols];
        for (int i = 0; i < this.rows; i++)
        {
            for (int j = 0; j < this.cols; j++)
            {
                this.grid[i, j] = new ObstacleGenerator.GridCell(num, i, j, new Vector2((float)j * ObstacleGenerator.unit, (float)i * ObstacleGenerator.unit));
                num++;
            }
        }
        this.obstacleHandler.CreateObstacleLibrary();
        Debug.Log(string.Concat(new object[]
        {
            "Grid Size: ",
            this.rows,
            "x",
            this.cols,
            "\n Num Cells: ",
            this.rows * this.cols
        }));
    }

    // Token: 0x06000092 RID: 146 RVA: 0x000062E8 File Offset: 0x000044E8
    private void OnDrawGizmos()
    {
        if (!this.debugMode)
        {
            return;
        }
        if (this.rows == 0 || this.cols == 0)
        {
            return;
        }
        if (this.gridHolder == null)
        {
            return;
        }
        for (int i = 0; i < this.rows; i++)
        {
            for (int j = 0; j < this.cols; j++)
            {
                Gizmos.color = Color.green;
                Vector3 from = new Vector3((float)j * ObstacleGenerator.unit + ObstacleGenerator.unit, 0.25f, (float)i * ObstacleGenerator.unit + ObstacleGenerator.unit) + this.gridHolder.position;
                Gizmos.DrawLine(from, new Vector3(from.x - ObstacleGenerator.unit, 0.25f, from.z));
                Gizmos.DrawLine(from, new Vector3(from.x, 0.25f, from.z - ObstacleGenerator.unit));
            }
        }
    }

    // Token: 0x06000093 RID: 147 RVA: 0x000063E4 File Offset: 0x000045E4
    [ContextMenu("SpawnObstacles")]
    private void SpawnObstacles()
    {
        if (this.rows == 0 || this.cols == 0)
        {
            return;
        }
        if (this.gridHolder == null)
        {
            return;
        }
        for (int i = 0; i < this.rows; i++)
        {
            for (int j = 0; j < this.cols; j++)
            {
                ObstacleGenerator.GridCell gridCell = this.grid[i, j];
                if (!gridCell.Valid)
                {
                    return;
                }
                gridCell.Valid = false;
                float value = UnityEngine.Random.value;
                if (value >= 0.6f)
                {
                    ObstacleHandler.Obstacle obstacle = this.obstacleHandler.GetObstacle();
                    GameObject gameObject = this.obstacleHandler.GetGameObject(obstacle);
                    gameObject.transform.parent = this.gridHolder;
                    gameObject.transform.localPosition = new Vector3(gridCell.position.x, 0f, gridCell.position.y);
                    int width = obstacle.width;
                    int height = obstacle.height;
                    for (int k = 0; k < height; k++)
                    {
                        if (i + k >= this.rows)
                        {
                            break;
                        }
                        int num = 0;
                        while (k < width)
                        {
                            if (j + num >= this.cols)
                            {
                                break;
                            }
                            this.grid[i + k, j + num].Valid = false;
                            num++;
                        }
                    }
                }
            }
        }
    }

    // Token: 0x040000A3 RID: 163
    public static float unit = 10f;

    // Token: 0x040000A4 RID: 164
    public bool debugMode;

    // Token: 0x040000A5 RID: 165
    public Vector2 gridSize;

    // Token: 0x040000A6 RID: 166
    public Transform gridHolder;

    // Token: 0x040000A7 RID: 167
    public ObstacleHandler obstacleHandler;

    // Token: 0x040000A8 RID: 168
    private int rows;

    // Token: 0x040000A9 RID: 169
    private int cols;

    // Token: 0x040000AA RID: 170
    private ObstacleGenerator.GridCell[,] grid;

    // Token: 0x02000017 RID: 23
    public class GridCell
    {
        // Token: 0x06000095 RID: 149 RVA: 0x00006566 File Offset: 0x00004766
        public GridCell(int id, int row, int col, Vector2 position)
        {
            this.id = id;
            this.row = row;
            this.col = col;
            this.position = position;
            this.Valid = true;
        }

        // Token: 0x1700000E RID: 14
        // (get) Token: 0x06000096 RID: 150 RVA: 0x00006592 File Offset: 0x00004792
        // (set) Token: 0x06000097 RID: 151 RVA: 0x0000659A File Offset: 0x0000479A
        public bool Valid { get; set; }

        // Token: 0x040000AB RID: 171
        public int id;

        // Token: 0x040000AC RID: 172
        public int row;

        // Token: 0x040000AD RID: 173
        public int col;

        // Token: 0x040000AE RID: 174
        public Vector2 position;
    }
}

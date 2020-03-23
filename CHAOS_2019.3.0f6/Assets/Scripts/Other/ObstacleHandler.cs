using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000018 RID: 24
[Serializable]
public class ObstacleHandler
{
    // Token: 0x06000099 RID: 153 RVA: 0x000065CC File Offset: 0x000047CC
    public void CreateObstacleLibrary()
    {
        if (this.obstacles.Length == 0)
        {
            return;
        }
        this.idToObstacle.Clear();
        this.typeToIDs.Clear();
        this.typeToIDs.Add(ObstacleHandler.ObstacleType.Tower, new List<int>());
        this.typeToIDs.Add(ObstacleHandler.ObstacleType.Building, new List<int>());
        this.typeToIDs.Add(ObstacleHandler.ObstacleType.Wall, new List<int>());
        int num = 0;
        foreach (ObstacleHandler.Obstacle obstacle in this.obstacles)
        {
            obstacle.ID = num;
            this.typeToIDs[obstacle.type].Add(obstacle.ID);
            this.idToObstacle.Add(obstacle.ID, obstacle);
            num++;
        }
    }

    // Token: 0x0600009A RID: 154 RVA: 0x0000668C File Offset: 0x0000488C
    public ObstacleHandler.Obstacle GetObstacle()
    {
        float value = UnityEngine.Random.value;
        ObstacleHandler.ObstacleType key;
        if (value <= 0.5f)
        {
            key = ObstacleHandler.ObstacleType.Tower;
        }
        else if (value <= 0.8f)
        {
            key = ObstacleHandler.ObstacleType.Building;
        }
        else
        {
            key = ObstacleHandler.ObstacleType.Wall;
        }
        int index = UnityEngine.Random.Range(0, this.typeToIDs[key].Count);
        int key2 = this.typeToIDs[key][index];
        return this.idToObstacle[key2];
    }

    // Token: 0x0600009B RID: 155 RVA: 0x000066FC File Offset: 0x000048FC
    public GameObject GetGameObject(ObstacleHandler.Obstacle o)
    {
        return this.GetGameObjectFromPool(o);
    }

    // Token: 0x0600009C RID: 156 RVA: 0x00006705 File Offset: 0x00004905
    public void ReturnGameObject(GameObject obj)
    {
        this.ReturnGameObjectToPool(obj);
    }

    // Token: 0x0600009D RID: 157 RVA: 0x00006710 File Offset: 0x00004910
    private GameObject GetGameObjectFromPool(ObstacleHandler.Obstacle o)
    {
        int id = o.ID;
        if (!this.objectPool.ContainsKey(id))
        {
            this.objectPool.Add(id, new List<GameObject>());
        }
        foreach (GameObject gameObject in this.objectPool[id])
        {
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
                return gameObject;
            }
        }
        GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(o.prefab, Vector3.zero, Quaternion.identity);
        this.objectPool[id].Add(gameObject2);
        return gameObject2;
    }

    // Token: 0x0600009E RID: 158 RVA: 0x000067DC File Offset: 0x000049DC
    private void ReturnGameObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
    }

    // Token: 0x040000B0 RID: 176
    public ObstacleHandler.Obstacle[] obstacles;

    // Token: 0x040000B1 RID: 177
    private Dictionary<int, ObstacleHandler.Obstacle> idToObstacle = new Dictionary<int, ObstacleHandler.Obstacle>();

    // Token: 0x040000B2 RID: 178
    private Dictionary<ObstacleHandler.ObstacleType, List<int>> typeToIDs = new Dictionary<ObstacleHandler.ObstacleType, List<int>>();

    // Token: 0x040000B3 RID: 179
    private const float chance_tower = 0.5f;

    // Token: 0x040000B4 RID: 180
    private const float chance_building = 0.3f;

    // Token: 0x040000B5 RID: 181
    private const float chance_wall = 0.2f;

    // Token: 0x040000B6 RID: 182
    private Dictionary<int, List<GameObject>> objectPool = new Dictionary<int, List<GameObject>>();

    // Token: 0x02000019 RID: 25
    public enum ObstacleType
    {
        // Token: 0x040000B8 RID: 184
        Tower,
        // Token: 0x040000B9 RID: 185
        Building,
        // Token: 0x040000BA RID: 186
        Wall
    }

    // Token: 0x0200001A RID: 26
    [Serializable]
    public class Obstacle
    {
        // Token: 0x1700000F RID: 15
        // (get) Token: 0x060000A0 RID: 160 RVA: 0x000067ED File Offset: 0x000049ED
        // (set) Token: 0x060000A1 RID: 161 RVA: 0x000067F5 File Offset: 0x000049F5
        public int ID { get; set; }

        // Token: 0x040000BC RID: 188
        public ObstacleHandler.ObstacleType type;

        // Token: 0x040000BD RID: 189
        public GameObject prefab;

        // Token: 0x040000BE RID: 190
        public int width;

        // Token: 0x040000BF RID: 191
        public int height;
    }
}

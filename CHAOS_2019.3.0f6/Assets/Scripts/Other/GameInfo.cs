using System;

// Token: 0x0200000B RID: 11
public static class GameInfo
{
    // Token: 0x06000041 RID: 65 RVA: 0x00004074 File Offset: 0x00002274
    public static void Reset()
    {
        GameInfo.CurrentTime = 0f;
        GameInfo.finishTime = 0f;
        GameInfo.titansOnScreen = 0;
        GameInfo.proximityToGate = 0;
        GameInfo.waypointsRemaining = 0;
        GameInfo.playerScore = 0;
        GameInfo.titanKills = 0;
        GameInfo.killStreak = 0;
        GameInfo.killStreakTimer = 0;
        GameInfo.bestKillStreak = 0;
        GameInfo.GameOver = false;
        GameInfo.NeedsUpdate = false;
        GameInfo.StartParameter = string.Empty;
    }

    // Token: 0x17000004 RID: 4
    // (get) Token: 0x06000042 RID: 66 RVA: 0x000040DB File Offset: 0x000022DB
    // (set) Token: 0x06000043 RID: 67 RVA: 0x000040E2 File Offset: 0x000022E2
    public static float CurrentTime
    {
        get
        {
            return GameInfo.currentTime;
        }
        set
        {
            if (!GameInfo.GameOver)
            {
                GameInfo.finishTime = value;
            }
            GameInfo.currentTime = value;
        }
    }

    // Token: 0x17000005 RID: 5
    // (get) Token: 0x06000044 RID: 68 RVA: 0x000040FA File Offset: 0x000022FA
    public static float FinishTime
    {
        get
        {
            return GameInfo.finishTime;
        }
    }

    // Token: 0x17000006 RID: 6
    // (get) Token: 0x06000045 RID: 69 RVA: 0x00004101 File Offset: 0x00002301
    // (set) Token: 0x06000046 RID: 70 RVA: 0x00004108 File Offset: 0x00002308
    public static int TitansOnScreen
    {
        get
        {
            return GameInfo.titansOnScreen;
        }
        set
        {
            GameInfo.NeedsUpdate = true;
            GameInfo.titansOnScreen = value;
        }
    }

    // Token: 0x17000007 RID: 7
    // (get) Token: 0x06000047 RID: 71 RVA: 0x00004116 File Offset: 0x00002316
    // (set) Token: 0x06000048 RID: 72 RVA: 0x0000411D File Offset: 0x0000231D
    public static int ProximityToGate
    {
        get
        {
            return GameInfo.proximityToGate;
        }
        set
        {
            GameInfo.proximityToGate = value;
        }
    }

    // Token: 0x17000008 RID: 8
    // (get) Token: 0x06000049 RID: 73 RVA: 0x00004125 File Offset: 0x00002325
    // (set) Token: 0x0600004A RID: 74 RVA: 0x0000412C File Offset: 0x0000232C
    public static int WaypointsRemaining
    {
        get
        {
            return GameInfo.waypointsRemaining;
        }
        set
        {
            GameInfo.NeedsUpdate = true;
            GameInfo.waypointsRemaining = value;
        }
    }

    // Token: 0x17000009 RID: 9
    // (get) Token: 0x0600004B RID: 75 RVA: 0x0000413A File Offset: 0x0000233A
    // (set) Token: 0x0600004C RID: 76 RVA: 0x00004141 File Offset: 0x00002341
    public static int PlayerScore
    {
        get
        {
            return GameInfo.playerScore;
        }
        set
        {
            GameInfo.NeedsUpdate = true;
            GameInfo.playerScore = value;
        }
    }

    // Token: 0x1700000A RID: 10
    // (get) Token: 0x0600004D RID: 77 RVA: 0x0000414F File Offset: 0x0000234F
    // (set) Token: 0x0600004E RID: 78 RVA: 0x00004156 File Offset: 0x00002356
    public static int TitanKills
    {
        get
        {
            return GameInfo.titanKills;
        }
        set
        {
            GameInfo.NeedsUpdate = true;
            GameInfo.titanKills = value;
        }
    }

    // Token: 0x1700000B RID: 11
    // (get) Token: 0x0600004F RID: 79 RVA: 0x00004164 File Offset: 0x00002364
    // (set) Token: 0x06000050 RID: 80 RVA: 0x0000416B File Offset: 0x0000236B
    public static int KillStreak
    {
        get
        {
            return GameInfo.killStreak;
        }
        set
        {
            if (value > GameInfo.bestKillStreak)
            {
                GameInfo.bestKillStreak = value;
            }
            GameInfo.NeedsUpdate = true;
            GameInfo.killStreak = value;
        }
    }

    // Token: 0x1700000C RID: 12
    // (get) Token: 0x06000051 RID: 81 RVA: 0x0000418A File Offset: 0x0000238A
    // (set) Token: 0x06000052 RID: 82 RVA: 0x00004191 File Offset: 0x00002391
    public static int KillStreakTimer
    {
        get
        {
            return GameInfo.killStreakTimer;
        }
        set
        {
            GameInfo.killStreakTimer = value;
        }
    }

    // Token: 0x1700000D RID: 13
    // (get) Token: 0x06000053 RID: 83 RVA: 0x00004199 File Offset: 0x00002399
    public static int BestKillStreak
    {
        get
        {
            return GameInfo.bestKillStreak;
        }
    }

    // Token: 0x04000044 RID: 68
    public static int GameVersion = 2;

    // Token: 0x04000045 RID: 69
    private static float currentTime;

    // Token: 0x04000046 RID: 70
    private static float finishTime;

    // Token: 0x04000047 RID: 71
    private static int titansOnScreen;

    // Token: 0x04000048 RID: 72
    private static int proximityToGate;

    // Token: 0x04000049 RID: 73
    private static int waypointsRemaining;

    // Token: 0x0400004A RID: 74
    private static int playerScore;

    // Token: 0x0400004B RID: 75
    private static int titanKills;

    // Token: 0x0400004C RID: 76
    private static int killStreak;

    // Token: 0x0400004D RID: 77
    private static int killStreakTimer;

    // Token: 0x0400004E RID: 78
    private static int bestKillStreak;

    // Token: 0x0400004F RID: 79
    public static bool GameOver;

    // Token: 0x04000050 RID: 80
    public static bool NeedsUpdate;

    // Token: 0x04000051 RID: 81
    public static string StartParameter = string.Empty;
}

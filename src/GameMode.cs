using Godot;

namespace DesktopBadmintonGame;

public enum GameDifficulty
{
    Easy,   // 简单模式：显示轨迹预测
    Hard    // 困难模式：不显示轨迹预测
}

public static class GameMode
{
    public static GameDifficulty CurrentDifficulty { get; private set; } = GameDifficulty.Hard;
    
    public static void SetDifficulty(GameDifficulty difficulty)
    {
        CurrentDifficulty = difficulty;
        GD.Print($"游戏模式切换到: {(difficulty == GameDifficulty.Easy ? "简单模式" : "困难模式")}");
    }
    
    public static void ToggleDifficulty()
    {
        CurrentDifficulty = CurrentDifficulty == GameDifficulty.Easy ? GameDifficulty.Hard : GameDifficulty.Easy;
        GD.Print($"游戏模式切换到: {(CurrentDifficulty == GameDifficulty.Easy ? "简单模式" : "困难模式")}");
    }
    
    public static bool IsEasyMode()
    {
        return CurrentDifficulty == GameDifficulty.Easy;
    }
    
    public static bool IsHardMode()
    {
        return CurrentDifficulty == GameDifficulty.Hard;
    }
    
    public static string GetDifficultyName()
    {
        return CurrentDifficulty == GameDifficulty.Easy ? "简单模式" : "困难模式";
    }
}
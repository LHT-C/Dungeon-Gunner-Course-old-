public enum Orientation
{
    north,
    east,
    south,
    west,
    none
}

public enum AimDirection
{
    Up,
    Down, 
    Left, 
    Right,
    UpRight,
    UpLeft
}

public enum GameState
{
    gameStarted,
    playingLevel,
    engagingEnemies,
    bossStage,
    engagingBoss,
    levelCompleted,
    gameWon,
    gameLost,
    gamePaused,
    dungeonOverviewMap,
    restartGame
}
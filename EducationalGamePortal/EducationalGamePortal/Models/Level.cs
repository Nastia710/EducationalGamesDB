using System;
using System.Collections.Generic;

namespace EducationalGamePortal.Models;

public partial class Level
{
    public int Id { get; set; }

    public int GameId { get; set; }

    public int LevelNumber { get; set; }

    public string Title { get; set; } = null!;

    public string Difficulty { get; set; } = null!;

    public int MaxScore { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
}

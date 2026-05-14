using System;
using System.Collections.Generic;

namespace EducationalGamePortal.Models;

public partial class GameSession
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int LevelId { get; set; }

    public int Score { get; set; }

    public int TimeSpent { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime PlayedAt { get; set; }

    public virtual Level Level { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace EducationalGamePortal.Models;

public partial class Achievement
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string CriteriaType { get; set; } = null!;

    public int CriteriaValue { get; set; }

    public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}

using System;
using System.Collections.Generic;

namespace EducationalGamePortal.Models;

public partial class Game
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Level> Levels { get; set; } = new List<Level>();
}

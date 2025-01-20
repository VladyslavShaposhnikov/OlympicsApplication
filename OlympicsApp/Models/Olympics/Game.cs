using System;
using System.Collections.Generic;

namespace OlympicsApp.Models.Olympics;

public partial class Game
{
    public int Id { get; set; }

    public int? GamesYear { get; set; }

    public string? GamesName { get; set; }

    public string? Season { get; set; }

    public virtual ICollection<GamesCompetitor> GamesCompetitors { get; set; } = new List<GamesCompetitor>();
}

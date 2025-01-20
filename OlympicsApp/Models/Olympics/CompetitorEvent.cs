using System;
using System.Collections.Generic;

namespace OlympicsApp.Models.Olympics;

public partial class CompetitorEvent
{
    public int? EventId { get; set; }

    public int? CompetitorId { get; set; }

    public int? MedalId { get; set; }

    public virtual GamesCompetitor? Competitor { get; set; }

    public virtual Event? Event { get; set; }

    public virtual Medal? Medal { get; set; }
}

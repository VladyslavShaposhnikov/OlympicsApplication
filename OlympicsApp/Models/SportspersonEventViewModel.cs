using OlympicsApp.Models.Olympics;

namespace OlympicsApp.Models;

public class SportspersonEventViewModel
{
    public int? EventId { get; set; }
    public int? Event { get; set; }
    public int? MedalId { get; set; }
    public string SportName { get; set; }
    public string EventName { get; set; }
    public int? Olympiad { get; set; }
    public string? Season { get; set; }
    public int? AgeAtEvent { get; set; }
    public string? Medal { get; set; }
}
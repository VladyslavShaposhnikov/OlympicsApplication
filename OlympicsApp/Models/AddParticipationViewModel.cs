using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

public class AddParticipationViewModel
{
    [Required(ErrorMessage = "Please select a medal.")]
    public int SelectedMedalId { get; set; }

    [Required(ErrorMessage = "Please select an Olympiad.")]
    public int SelectedOlympiadId { get; set; }

    [Required(ErrorMessage = "Please select an event.")]
    public int SelectedEventId { get; set; }

    public int? AgeAtEvent { get; set; } // Optional

    public List<SelectListItem> Events { get; set; } = new();
    public List<SelectListItem> Olympiads { get; set; } = new();
    public List<SelectListItem> Medals { get; set; } = new();
}
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OlympicsApp.Models;
using OlympicsApp.Models.Olympics;

namespace OlympicsApp.Controllers;
[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly OlympicsContext _context;

    public HomeController(ILogger<HomeController> logger, OlympicsContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Sportspeople(int page = 1, int pageSize = 30)
    {
        // Calculate total items and pages
        var totalItems = _context.People.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        // Get the data for the current page
        var sportspeople = _context.People
            .Include(p => p.GamesCompetitors)
            .OrderBy(p => p.FullName) // Sort the data
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        // Prepare the ViewModel
        var sportspeopleList = sportspeople.Select(person => new SportspersonViewModel
        {
            FullName = person.FullName,
            Weight = person.Weight,
            Height = person.Height,
            Gender = person.Gender,
            GoldMedals = _context.CompetitorEvents.Count(m => m.Competitor.PersonId == person.Id && m.Medal.MedalName == "Gold"),
            SilverMedals = _context.CompetitorEvents.Count(m => m.Competitor.PersonId == person.Id && m.Medal.MedalName == "Silver"),
            BronzeMedals = _context.CompetitorEvents.Count(m => m.Competitor.PersonId == person.Id && m.Medal.MedalName == "Bronze"),
            NumberOfParticipations = _context.CompetitorEvents.Count(ce =>
                ce.Competitor.PersonId == person.Id),
            SportspersonId = person.Id
        }).ToList();

        var pagesToShow = new List<int>();
        const int maxPagesToShow = 5;

        // Always include the first page
        pagesToShow.Add(1);

        // Add pages around the current page
        for (int i = page - 2; i <= page + 2; i++)
        {
            if (i > 1 && i < totalPages) // Exclude 1 and the last page
            {
                pagesToShow.Add(i);
            }
        }

        // Always include the last page
        if (totalPages > 1)
        {
            pagesToShow.Add(totalPages);
        }

        // Pass data to the view
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.PagesToShow = pagesToShow.OrderBy(x => x).Distinct().ToList();

        return View(sportspeopleList);
    }

    public IActionResult SportspersonEvents(int sportspersonId)
    {
        // Filter CompetitorEvents for the specific sportspersonId
        var competitorEvents = _context.CompetitorEvents
            .Include(ce => ce.Competitor)
            .ThenInclude(c => c.Games) // Include related Games for the competitor
            .Include(ce => ce.Event)
            .ThenInclude(e => e.Sport) // Include related Sport for the event
            .Include(ce => ce.Medal) // Include Medal information
            .Where(ce => ce.Competitor.PersonId == sportspersonId) // Filter by sportspersonId
            .ToList();
        // Transform the data into the ViewModel
        @ViewBag.Name = _context.People.FirstOrDefault(p => p.Id == sportspersonId).FullName;
        var sportspersonEvents = competitorEvents.Select(ce => new SportspersonEventViewModel
        {
            EventId = ce.CompetitorId,
            Event = ce.EventId,
            MedalId = ce.MedalId,
            SportName = ce.Event.Sport.SportName,
            EventName = ce.Event.EventName,
            Olympiad = ce.Competitor.Games.GamesYear,
            Season = ce.Competitor.Games.Season,
            AgeAtEvent = ce.Competitor.Age,
            Medal = ce.Medal?.MedalName ?? "none" // Safely handle null Medal
        }).ToList();

        return View(sportspersonEvents);
    }

    [HttpGet]
    public IActionResult AddParticipation(int sportspersonId)
    {
        var model = new AddParticipationViewModel
        {
            Events = _context.Events.Include(e => e.Sport)
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"{e.Sport.SportName} - {e.EventName}"
                }).ToList(),

            Olympiads = _context.Games
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = $"{g.GamesYear} - {g.Season}"
                }).ToList(),
            
            Medals = _context.Medals
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.MedalName
                }).ToList()
        };

        ViewBag.SportspersonId = sportspersonId;
        return View(model);
    }

    
    [HttpPost]
    public IActionResult AddParticipation(int sportspersonId, AddParticipationViewModel model)
    {
        if (ModelState.IsValid)
        {
            var competitor = _context.GamesCompetitors.FirstOrDefault(gc => gc.PersonId == sportspersonId);

            if (competitor == null)
            {
                ModelState.AddModelError(string.Empty, "The specified sportsperson could not be found.");
                return View(model);
            }

            // Create a new CompetitorEvent record
            var newParticipation = new CompetitorEvent
            {
                EventId = model.SelectedEventId,
                MedalId = model.SelectedMedalId,
                CompetitorId = competitor.Id,
            };

            _context.Database.ExecuteSqlInterpolated($@"
            INSERT INTO competitor_event (competitor_id, event_id, medal_id) 
            VALUES ({newParticipation.CompetitorId}, {newParticipation.EventId}, {newParticipation.MedalId})");

            return RedirectToAction("SportspersonEvents", new { sportspersonId });
        }

        // Repopulate dropdowns if validation fails
        model.Events = _context.Events
            .Include(e => e.Sport)
            .Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = $"{e.Sport.SportName} - {e.EventName}"
            }).ToList();

        model.Olympiads = _context.Games
            .Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = $"{g.GamesYear} - {g.Season}"
            }).ToList();

        model.Medals = _context.Medals
            .Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.MedalName
            }).ToList();

        return View(model);
    }

    
    [HttpPost]
    public IActionResult DeleteParticipation(int competitorId, int eventId, int medalId)
    {
        if (competitorId <= 0 || eventId <= 0 || medalId <= 0)
        {
            return BadRequest("Invalid parameters.");
        }
        // raw SQL to delete the record
        _context.Database.ExecuteSqlInterpolated($@"
        DELETE FROM competitor_event 
        WHERE competitor_id = {competitorId} 
          AND event_id = {eventId} 
          AND medal_id = {medalId}");

        // Redirect to a relevant view, e.g., Sportspeople
        return RedirectToAction("Sportspeople");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
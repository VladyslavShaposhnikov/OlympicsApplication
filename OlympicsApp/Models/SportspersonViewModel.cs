namespace OlympicsApp.Models.Olympics;

public class SportspersonViewModel
{
    public string FullName { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public string Gender { get; set; }
    public int GoldMedals { get; set; }
    public int SilverMedals { get; set; }
    public int BronzeMedals { get; set; }
    public int NumberOfParticipations { get; set; }
    public int SportspersonId { get; set; }
}
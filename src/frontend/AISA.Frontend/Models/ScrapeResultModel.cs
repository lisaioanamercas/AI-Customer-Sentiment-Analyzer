namespace AISA.Frontend.Models;

/// <summary>
/// Rezultatul unui import de recenzii prin scraping.
/// </summary>
public class ScrapeResultModel
{
    public int NewCount { get; set; }
    public int SkippedCount { get; set; }
}

namespace AISA.Application.Reviews.DTOs;

/// <summary>
/// Rezultatul unui import de recenzii prin scraping.
/// </summary>
public class ScrapeResultDto
{
    /// <summary>Numărul de recenzii nou importate și analizate.</summary>
    public int NewCount { get; set; }

    /// <summary>Numărul de recenzii ignorate (deja existente în DB).</summary>
    public int SkippedCount { get; set; }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace AM.Services.Portfolio.Domain.Entities;

public class Report
{
    [Key]
    public long Id { get; set; }

    public long ReportFileId { get; init; }
    public ReportFile ReportFile { get; set; } = null!;

    public int AccountId { get; init; }
    public virtual Account Account { get; init; } = null!;


    public DateOnly DateStart { get; set; }
    public DateOnly DateEnd { get; set; }
}
﻿using CommonServices.Models.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace IM.Service.Company.Analyzer.DataAccess.Entities
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Report : ReportIdentity
    {
        public virtual Ticker Ticker { get; set; } = null!;
        public string SourceType { get; init; } = null!;


        [Column(TypeName = "Decimal(18,4)")]
        public decimal Result { get; set; }

        public virtual Status Status { get; set; } = null!;
        public byte StatusId { get; set; }
    }
}
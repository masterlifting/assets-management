﻿using System;
using IM.Service.Common.Net.Models.Entity.Companies.Interfaces;

namespace IM.Service.Common.Net.Models.Dto.Http.Companies
{
    public class StockVolumePostDto : StockVolumePutDto, ICompanyDateIdentity
    {
        public string CompanyId { get; init; } = null!;
        public DateTime Date { get; init; }
    }
}
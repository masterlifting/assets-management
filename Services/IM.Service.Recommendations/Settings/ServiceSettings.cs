﻿using IM.Service.Common.Net.Models.Configuration;
using IM.Service.Recommendations.Settings.Sections;

namespace IM.Service.Recommendations.Settings;

public class ServiceSettings
{
    public ClientSettings ClientSettings { get; set; } = null!;
    public ConnectionStrings ConnectionStrings { get; set; } = null!;
}
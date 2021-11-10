﻿using IM.Service.Common.Net.RepositoryService;

namespace IM.Service.Recommendations.DataAccess.Repository
{
    public class RepositorySet<T> : Repository<T, DatabaseContext> where T : class
    {
        public RepositorySet(DatabaseContext context, IRepositoryHandler<T> handler) : base(context, handler) { }
    }
}

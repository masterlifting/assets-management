﻿using System;
using System.Collections.Generic;

namespace DataSetter.DataAccess.Entities
{
    public partial class Ticker
    {
        public Ticker()
        {
            Prices = new HashSet<Price>();
            StockTransactions = new HashSet<StockTransaction>();
        }

        public long Id { get; set; }
        public DateTime DateUpdate { get; set; }
        public string Name { get; set; } = null!;
        public long CompanyId { get; set; }
        public long ExchangeId { get; set; }
        public long LotId { get; set; }

        public virtual Company Company { get; set; } = null!;
        public virtual Exchange Exchange { get; set; } = null!;
        public virtual Lot Lot { get; set; } = null!;
        public virtual ICollection<Price> Prices { get; set; }
        public virtual ICollection<StockTransaction> StockTransactions { get; set; }
    }
}
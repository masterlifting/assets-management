namespace IM.Service.Company.Data.Models.Client.Price.MoexModels
{
    public class MoexHistoryPriceResultModel
    {
        public MoexHistoryPriceResultModel(MoexHistoryPriceData? data, string ticker)
        {
            Data = data;
            Ticker = ticker;
        }

        public MoexHistoryPriceData? Data { get; }
        public string Ticker { get; }
    }
}
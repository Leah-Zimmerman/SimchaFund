using SimchaFund.Data;

namespace SimchaFund.Web.Models
{
    public class SimchasViewModel
    {
        public List<Simcha> Simchas { get; set; }
        public int ContributorCount { get; set; }
        public int ContributorCountTotal { get; set; }
        public decimal Total { get; set; }
    }
}

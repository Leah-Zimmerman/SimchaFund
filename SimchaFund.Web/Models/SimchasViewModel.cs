using SimchaFund.Data;

namespace SimchaFund.Web.Models
{
    public class SimchasViewModel
    {
        public List<Simcha> Simchas { get; set; }
        public int ContributorsForSimcha { get; set; }
        public int ContributorCount { get; set; }
        public int ContributorCountTotal { get; set; }
        public decimal Total { get; set; }
        public string FullName { get; set; }
        public decimal Balance { get; set; }
        public List<Deposit> Deposits { get; set; }
        public List<Contribution> Contributions { get; set; }
        public List<Contributor> SimchaContributors { get; set; }
        public string SimchaName { get; set; }
        public int SimchaId { get; set; }
                
    }
}

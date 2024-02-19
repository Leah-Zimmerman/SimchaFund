using SimchaFund.Data;
namespace SimchaFund.Web.Models

{
    public class ContributorsViewModel
    {
        public List<Contributor> Contributors { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalBalance { get; set; }
    }
}

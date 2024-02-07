using Microsoft.AspNetCore.Mvc;
using SimchaFund.Data;
using SimchaFund.Web.Models;
using System.Diagnostics;

namespace SimchaFund.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=SimchaFund; Integrated Security=true;";
        public IActionResult Index()
        {
            var sm = new SimchaManager(_connectionString);
            var svm = new SimchasViewModel
            {
                Simchas = sm.GetSimchas(),
                ContributorCountTotal = sm.GetContributorTotal()
            };
            return View(svm);
        }

        public IActionResult Contributors()
        {
            var sm = new SimchaManager(_connectionString);
            var cvm = new ContributorsViewModel
            {
                Contributors = sm.GetContributors(),
            };
            return View(cvm);
        }
        public IActionResult AddSimcha(string name, DateTime date)
        {
            var sm = new SimchaManager(_connectionString);
            sm.AddSimcha(name, date);
            return Redirect("/home/index");
        }
        public IActionResult MakeDeposit(int contributorId, decimal amount, DateTime date)
        {
            var sm = new SimchaManager(_connectionString);
            sm.MakeDeposit(contributorId, amount, date);
            return Redirect("/home/contributors");
        }
        
    }
}
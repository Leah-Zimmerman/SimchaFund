using Microsoft.AspNetCore.Mvc;
using SimchaFund.Data;
using SimchaFund.Web.Models;
using System.Diagnostics;

namespace SimchaFund.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=SimchaFund; Integrated Security=true;";

        public IActionResult Index(int simchaId)
        {
            var sm = new SimchaManager(_connectionString);
            var svm = new SimchasViewModel
            {
                Simchas = sm.GetSimchasWithContributorCountAndTotal(),
                ContributorCountTotal = sm.GetContributorTotal()
            };
            return View(svm);
        }

        public IActionResult Contributors()
        {
            var sm = new SimchaManager(_connectionString);
            decimal totalBalance = 0;
            foreach(var c in sm.GetContributorsWithBalance())
            {
                totalBalance += c.Balance;
            }
            var cvm = new ContributorsViewModel
            {
                Contributors = sm.GetContributorsWithBalance(),
                TotalBalance = totalBalance
            };
            return View(cvm);
        }
        [HttpPost]
        public IActionResult AddSimcha(string name, DateTime date)
        {
            var sm = new SimchaManager(_connectionString);
            sm.AddSimcha(name, date);
            return Redirect("/home/index");
        }
        [HttpPost]
        public IActionResult MakeDeposit(int contributorId, decimal amount, DateTime date)
        {
            var sm = new SimchaManager(_connectionString);
            sm.MakeDeposit(contributorId, amount, date);
            return Redirect("/home/contributors");
        }
        [HttpPost]
        public IActionResult AddContributor(string firstName, string lastName, string cell, bool alwaysInclude)
        {
            var sm = new SimchaManager(_connectionString);
            sm.AddContributor(firstName, lastName, cell, alwaysInclude);
            return Redirect("/home/contributors");
        }
        [HttpPost]
        public IActionResult EditContributor(int contributorId, string firstName, string lastName, string cell, bool alwaysInclude)
        {
            var sm = new SimchaManager(_connectionString);
            sm.UpdateContributor(contributorId, firstName, lastName, cell, alwaysInclude);
            return Redirect("/home/contributors");
        }
        public IActionResult ShowHistory(int contributorId)
        {
            var sm = new SimchaManager(_connectionString);
            var contributions = sm.GetContributionsForId(contributorId);
            foreach (var c in contributions)
            {
                c.SimchaName = sm.GetSimchaNameById(c.SimchaId);
            }
            var svm = new SimchasViewModel
            {
                FullName = sm.GetFullNameForId(contributorId),
                Balance = sm.GetBalanceForId(contributorId),
                Deposits = sm.GetDepositsForId(contributorId),
                Contributions = contributions
            };

            return View(svm);
        }
        public IActionResult Contribute(int simchaId)
        {
            var sm = new SimchaManager(_connectionString);
            var svm = new SimchasViewModel
            {
                SimchaName = sm.GetSimchaNameById(simchaId),
                SimchaId = simchaId,
                SimchaContributors = sm.GetContributorsForSimcha(simchaId)
            };
            return View(svm);

        }
        public IActionResult UpdateContributions(List<ContributorRow> contributorRows, int SimchaId)
        {
            var sm = new SimchaManager(_connectionString);
            sm.DeleteContributionsForSimchaId(SimchaId);
            for (int x = 0; x < contributorRows.Count; x++)
            {
                if (contributorRows[x].Contribute == true)
                {
                    sm.AddContributionsForSimcha(SimchaId, contributorRows[x].ContributorId, contributorRows[x].Amount);
                }
            }
            return Redirect("/home/index");
        }

    }
}
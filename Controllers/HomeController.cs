using System.Threading.Tasks;
using Codesanook.ThailandAdministrativeDivisionTool.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrchardCore.EF;
using OrchardCore.EF.Filters;

namespace Codesanook.ThailandAdministrativeDivisionTool.Controllers
{
    public class HomeController : Controller
    {
        private readonly OrchardDbContext dbContext;

        public HomeController(OrchardDbContext dbContext) => this.dbContext = dbContext;

        [ServiceFilter(typeof(TransactionActionServiceFilter))]
        public async Task<ActionResult> Index()
        {
            var province1 = new Province()
            {
                Code = "BKK",
                NameInThai = "กรุงเทพมหานคร",
                NameInEnglish = "Bangkok"
            };

            var province2 = new Province()
            {
                Code = "KBV",
                NameInThai = "กระบี่",
                NameInEnglish = "Krabi"
            };

            dbContext.Set<Province>().Add(province1);
            await dbContext.SaveChangesAsync();

            dbContext.Set<Province>().Add(province2);
            await dbContext.SaveChangesAsync();

            var provinces = await dbContext.Set<Province>().ToListAsync();
            return View(provinces.AsReadOnly());
        }
    }
}

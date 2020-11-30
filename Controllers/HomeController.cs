using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codesanook.ThailandAdministrativeDivisionTool.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrchardCore.EF;
using OrchardCore.EF.Filters;
using OrchardCore.Users.Indexes;
using OrchardCore.Users.Models;
using YesSql;
using YesSql.Services;

namespace Codesanook.ThailandAdministrativeDivisionTool.Controllers
{
    public class HomeController : Controller
    {
        private readonly OrchardDbContext dbContext;
        private readonly ISession session;

        public HomeController(OrchardDbContext dbContext, ISession session)
        {
            this.dbContext = dbContext;
            this.session = session;
        }

        [ServiceFilter(typeof(TransactionActionServiceFilter))]
        public async Task<ActionResult> Index()
        {
            var provinces = await dbContext.Set<Province>().ToListAsync();
            var userIdList = provinces.Select(x => x.UserId);
            // https://github.com/sebastienros/yessql/blob/dev/test/YesSql.Tests/CoreTests.cs#L718
            var users = await session.Query<User, UserIndex>().Where(x => x.UserId.IsIn(userIdList)).ListAsync();

            var provincesWithUsers =
                from p in provinces
                join u in users.ToList()
                on p.UserId equals u.UserId
                select new
                {
                    p.Id,
                    p.NameInThai,
                    p.NameInEnglish,
                    Username = u.UserName,
                    UserEmail = u.Email,
                };

            return Json(provincesWithUsers);
        }

        [ServiceFilter(typeof(TransactionActionServiceFilter))]
        public async Task<ActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var province = new Province()
            {
                UserId = userId,
                Code = "BKK",
                NameInThai = "กรุงเทพมหานคร",
                NameInEnglish = "Bangkok"
            };

            dbContext.Set<Province>().Add(province);
            await dbContext.SaveChangesAsync();

            return Json(province);
        }
    }
}

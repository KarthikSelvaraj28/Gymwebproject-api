using Gymwebproject.DB;
using Gymwebproject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gymwebproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentsController : ControllerBase
    {

        private readonly Gymdbcontext obj;

        public EquipmentsController(Gymdbcontext context)
        {
            obj = context;
        }
        [HttpGet]
        [Route("List")]

public async Task<ActionResult<IEnumerable<Equipmentsdb>>> GetEquipmentsdb()
        {
            return await obj.Equipmentsdb.ToListAsync();
        }


    }
}





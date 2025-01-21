using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using Wpm.Management.Api.DataAccess;

namespace Wpm.Management.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PetsController(ManagementDbContext dbContext, ILogger<PetsController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var all = await dbContext.Pets.Include(p => p.Breed).ToListAsync();
            return Ok(all);
        }
        [HttpGet("{id}", Name = nameof(GetById))]
        public async Task<IActionResult> GetById(int id)
        {
            var pet = await dbContext.Pets.Include(p => p.Breed)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            return Ok(pet);
        }


        [HttpPost]
        public async Task<IActionResult> Create(NewPet newPet)
        {
            try
            {
                var pet = newPet.ToPet();
                await dbContext.Pets.AddAsync(pet);
                await dbContext.SaveChangesAsync();

                return CreatedAtRoute(nameof(GetById), new { id = pet.Id }, newPet);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
        public record NewPet(string Name, int Age, int BreedId)
        {
            public Pet ToPet()
            {
                return new Pet() { Name = Name, Age = Age, BreedId = BreedId };
            }
        }
    }
}
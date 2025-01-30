using DevOps.Data;
using DevOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevOps.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActionResourceStatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActionResourceStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ActionResourceStatus
        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<ActionResourceStatus>>> GetActionResourceStatuses()
        {
            return await _context.ActionResourceStatuses.ToListAsync();
        }*/

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActionResourceStatus>>> GetActionResourceStatuses(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? search = null, 
    [FromQuery] string? sortBy = null,
    [FromQuery] bool isAscending = true)
        {
            var query = _context.ActionResourceStatuses.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.Action.Contains(search) ||
                                         x.Resource.Contains(search) ||
                                         x.Status.Contains(search) ||
                                         (x.Description != null && x.Description.Contains(search)));
            }

            // Sorting
            query = sortBy switch
            {
                "Action" => isAscending ? query.OrderBy(x => x.Action) : query.OrderByDescending(x => x.Action),
                "Resource" => isAscending ? query.OrderBy(x => x.Resource) : query.OrderByDescending(x => x.Resource),
                "Status" => isAscending ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
                "Month" => isAscending ? query.OrderBy(x => x.Month) : query.OrderByDescending(x => x.Month),
                "Year" => isAscending ? query.OrderBy(x => x.Year) : query.OrderByDescending(x => x.Year),
                _ => query.OrderBy(x => x.Id) // Default sort by Id
            };

            // Pagination
            var totalRecords = await query.CountAsync();
            var paginatedData = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Data = paginatedData,
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize
            });
        }

        // GET: api/ActionResourceStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ActionResourceStatus>> GetActionResourceStatus(int id)
        {
            var actionResourceStatus = await _context.ActionResourceStatuses.FindAsync(id);

            if (actionResourceStatus == null)
            {
                return NotFound();
            }

            return actionResourceStatus;
        }

       // POST: api/ActionResourceStatus
        [HttpPost]
        public async Task<ActionResult<ActionResourceStatus>> PostActionResourceStatus(ActionResourceStatus actionResourceStatus)
        {
            _context.ActionResourceStatuses.Add(actionResourceStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActionResourceStatus), new { id = actionResourceStatus.Id }, actionResourceStatus);
        }

        // PUT: api/ActionResourceStatus/5
        [HttpPut("{id}")] 
        public async Task<IActionResult> PutActionResourceStatus(int id, ActionResourceStatus actionResourceStatus)
        {
            if (id != actionResourceStatus.Id)
            {
                return BadRequest();
            }

            _context.Entry(actionResourceStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActionResourceStatusExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/ActionResourceStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActionResourceStatus(int id)
        {
            var actionResourceStatus = await _context.ActionResourceStatuses.FindAsync(id);
            if (actionResourceStatus == null)
            {
                return NotFound();
            }

            _context.ActionResourceStatuses.Remove(actionResourceStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActionResourceStatusExists(int id)
        {
            return _context.ActionResourceStatuses.Any(e => e.Id == id);
        }
    }
}

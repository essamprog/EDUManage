using EduManage.Core.Entities;
using EduManage.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduManage.Web.Controllers;

/// <summary>
/// Public-facing Instructors Directory — no authentication required.
/// </summary>
public class InstructorsController : Controller
{
    private readonly AppDbContext _context;

    public InstructorsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /instructors
    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.InstructorProfiles
            .Include(p => p.User)
            .Include(p => p.Courses)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(p =>
                (p.User != null && p.User.FullName.ToLower().Contains(term)) ||
                (p.Expertise != null && p.Expertise.ToLower().Contains(term)));
        }

        var profiles = await query
            .OrderByDescending(p => p.TotalStudents)
            .ToListAsync();

        ViewData["Search"] = search;
        return View(profiles);
    }
}

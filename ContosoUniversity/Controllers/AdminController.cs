using ContosoUniversity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly SchoolContext _context;

        public AdminController(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var studentCount = await _context.Students.CountAsync();
            var courseCount = await _context.Courses.CountAsync();
            var instructorCount = await _context.Instructors.CountAsync();
            var enrollmentCount = await _context.Enrollments.CountAsync();

            var courseTitles = await _context.Courses
                .Select(c => c.Title)
                .ToListAsync();

            var courseEnrollmentCounts = await _context.Courses
                .Select(c => c.Enrollments.Count)
                .ToListAsync();

            ViewBag.StudentCount = studentCount;
            ViewBag.CourseCount = courseCount;
            ViewBag.InstructorCount = instructorCount;
            ViewBag.EnrollmentCount = enrollmentCount;
            ViewBag.CourseTitles = courseTitles;
            ViewBag.CourseEnrollmentCounts = courseEnrollmentCounts;

            return View();
        }
    }
}


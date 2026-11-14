using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentCourseManagementSystem.Data;
using StudentCourseManagementSystem.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace StudentCourseManagementSystem.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .Include(s => s.StudentCourses)
                    .ThenInclude(sc => sc.Course)
                .ToListAsync();
            return View(students);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null) return NotFound();

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Students/AssignCourses/5
        public async Task<IActionResult> AssignCourses(int id)
        {
            var student = await _context.Students
                .Include(s => s.StudentCourses)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null) return NotFound();

            ViewBag.Courses = await _context.Courses.ToListAsync();
            return View(student);
        }

        // POST: Students/AssignCourses/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignCourses(int id, int[] courseIds)
        {
            var student = await _context.Students
                .Include(s => s.StudentCourses)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null) return NotFound();

            // Remove existing assignments
            student.StudentCourses.Clear();

            // Add new assignments
            foreach (var courseId in courseIds)
            {
                student.StudentCourses.Add(new StudentCourse { StudentId = id, CourseId = courseId });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

using ASPNETCore_DB.Interfaces;
using ASPNETCore_DB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPNETCore_DB.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudent _studentRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentController(IStudent studentRepo, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            try
            {
                _studentRepo = studentRepo;
                _httpContextAccessor = httpContextAccessor;
                _webHostEnvironment = webHostEnvironment;
            }
            catch (Exception ex)
            {
                throw new Exception("Constructor not initialized - IStudent studentRepo");
            }
        }

        // Admin only: view all students
        [Authorize(Roles = "Admin")]
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["StudentNumberSortParm"] = String.IsNullOrEmpty(sortOrder) ? "number_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var students = _studentRepo.GetAll();

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.StudentNumber.Contains(searchString));
            }

            students = sortOrder switch
            {
                "number_desc" => students.OrderByDescending(s => s.StudentNumber),
                "Name" => students.OrderBy(s => s.Surname),
                "name_desc" => students.OrderByDescending(s => s.Surname),
                "Date" => students.OrderBy(s => s.EnrollmentDate),
                "date_desc" => students.OrderByDescending(s => s.EnrollmentDate),
                _ => students.OrderBy(s => s.StudentNumber),
            };

            int pageSize = 3;
            return View(PaginatedList<Student>.Create(students.AsQueryable(), pageNumber ?? 1, pageSize));
        }

        // User only: enroll (but redirect to details if already enrolled)
        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult Create()
        {
            // Check if user already enrolled
            var studentExist = _studentRepo.ByEmail(this.User.Identity.Name.ToString());

            if (studentExist != null)
            {
                return RedirectToAction("Details", "Student", studentExist.StudentNumber);
            }
            else
            {
                Student student = new Student();
                string fileName = "DefaultPic.PNG";
                student.Photo = fileName;
                student.EnrollmentDate = DateTime.Now;
                student.Email = this.User.Identity.Name.ToString();
                return View(student);
            }
        }

        // User only: save enrollment
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student); // return form if validation fails
            }

            var files = HttpContext.Request.Form.Files;
            string webRootPath = _webHostEnvironment.WebRootPath;
            string upload = webRootPath + WebConstants.ImagePath;

            if (files.Count > 0) // check if file exists before using files[0]
            {
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(
                    Path.Combine(upload, fileName + extension),
                    FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                student.Photo = fileName + extension;
            }
            else
            {
                student.Photo = "DefaultPic.PNG"; // fallback default image
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _studentRepo.Create(student);
                }
                else
                {
                    // show which fields failed validation
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    throw new Exception("ModelState invalid: " + string.Join(", ", errors));
                }
            }
            catch (Exception ex)
            {
                // show the real inner exception
                throw new Exception("Student record not saved: " + ex.Message + " | " + ex.InnerException?.Message);
            }

            return RedirectToAction("Details");
        }
        // User only: edit
        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _studentRepo.GetById(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // User only: save edit
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(string photoName, Student student)
        {
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                string upload = webRootPath + WebConstants.ImagePath;
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);

                var oldFile = Path.Combine(upload, photoName);
                if (System.IO.File.Exists(oldFile))
                {
                    System.IO.File.Delete(oldFile);
                }

                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                student.Photo = fileName + extension;
            }
            else
            {
                student.Photo = photoName;
            }

            try
            {
                _studentRepo.Edit(student);
            }
            catch (Exception ex)
            {
                throw new Exception("Student record not saved.");
            }

            return RedirectToAction("Details");
        }

        public IActionResult Details(string id)
        {
            Student student;

            if (string.IsNullOrEmpty(id))
            {
                // no id — look up by logged-in user's email
                student = _studentRepo.ByEmail(this.User.Identity.Name.ToString());
            }
            else
            {
                student = _studentRepo.GetById(id);
            }

            // student not found — send them to enroll instead of crashing
            if (student == null)
            {
                return RedirectToAction("Create");
            }

            return View(student);
        }

        // Admin only: confirm delete page
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _studentRepo.GetById(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // Admin only: execute delete
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var student = _studentRepo.GetById(id);

            if (student != null)
            {
                if (student.Photo != null && student.Photo != "DefaultPic.PNG")
                {
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string upload = webRootPath + WebConstants.ImagePath;
                    var oldFile = Path.Combine(upload, student.Photo);
                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }
                }

                _studentRepo.Delete(id);
            }

            return RedirectToAction("Index");
        }
    }
}
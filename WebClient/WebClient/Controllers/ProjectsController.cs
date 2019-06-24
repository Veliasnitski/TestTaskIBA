using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebClient.Models;
using WebClient.Repositories;


namespace WebClient.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly WebAPIRepository<ExtendedProject> apiAccessExtProject = new WebAPIRepository<ExtendedProject>("users");
        private readonly WebAPIRepository<ExtendedUser> apiAccessExtendedUser = new WebAPIRepository<ExtendedUser>("users");
        private readonly WebAPIRepository<Project> apiAccessProject = new WebAPIRepository<Project>("projects");
        private readonly WebAPIRepository<User> apiAccessUser = new WebAPIRepository<User>("users");
        private readonly WebAPIRepository<UserProject> apiAccessUserProject = new WebAPIRepository<UserProject>("users");
        private readonly WebAPIRepository<MyTask> apiAccessTask = new WebAPIRepository<MyTask>("projects");
        private readonly WebAPIRepository<ProjectTask> apiAccessProjectTask = new WebAPIRepository<ProjectTask>("projects");
        private readonly WebAPIRepository<ExtendedTask> apiAccessExtendedTask = new WebAPIRepository<ExtendedTask>("task");

        public async Task<IActionResult> MyProjects()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            List<Claim> claims = claimsIdentity.Claims.ToList();
            apiAccessExtProject.UrlAPI = "users/" + claims[1].Value.ToString() + "/projects";
            ViewData["IdUser"] = claims[1].Value.ToString();
            ViewData["MyProjects"] = (await apiAccessExtProject.GetAllAsync()).OrderBy(el => el.Project.Status);
            return View();
        }

        [HttpGet("project/{id}")]
        public async Task<IActionResult> Project(int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            List<Claim> claims = claimsIdentity.Claims.ToList();
            apiAccessProject.UrlAPI = "projects";
            Project p = await apiAccessProject.GetAsync(id.ToString());
            ViewData["Project"] = p;
            apiAccessExtendedUser.UrlAPI = "users/"+id.ToString()+ "/extendedusers";
            ViewData["Users"] = await apiAccessExtendedUser.GetAllAsync();
            apiAccessUserProject.UrlAPI = "users/" + claims[1].Value + "/project/"+id.ToString();
            var newEl = await apiAccessUserProject.GetAsync("");
            ViewData["MyRole"] = newEl.Role ;
            apiAccessExtendedTask.UrlAPI = "tasks/project/" + id.ToString() + "/user/" + claims[1].Value;
            ViewData["Tasks"] = (await apiAccessExtendedTask.GetAllAsync()).OrderByDescending(el=> el.Task.DateStart);
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddProject(Project p)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                List<Claim>  claims = claimsIdentity.Claims.ToList();
                ExtendedProject newProject = new ExtendedProject
                {
                    AdminId = int.Parse(claims[1].Value),
                    Project = p
                };
                apiAccessExtProject.UrlAPI = "projects/add";
                await apiAccessExtProject.AddAsync(newProject);
            }
            return RedirectToAction("MyProjects");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProject(int idProject)
        {
            apiAccessExtProject.UrlAPI = "projects";
            await apiAccessExtProject.DeleteAsync(idProject);
            return RedirectToAction("MyProjects");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int idUser, int idProject)
        {
            apiAccessUserProject.UrlAPI = "users/"+idUser.ToString()+"/project/"+idProject.ToString();
            var el = await apiAccessUserProject.GetAsync("");
            apiAccessUserProject.UrlAPI = "users/userproject";
            await apiAccessUserProject.DeleteAsync(el.Id);

            return RedirectToAction("Project", new { id = idProject });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProject(Project p)
        {
            apiAccessProject.UrlAPI = "projects/update";
            await apiAccessProject.UpdateAsync(p);

            return RedirectToAction("MyProjects");
        }

        [HttpPost]
        public async Task<IActionResult> AddUserInProject(string login, int idProject)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            List<Claim> claims = claimsIdentity.Claims.ToList();
            apiAccessUser.UrlAPI = "users/login";
            var user = await apiAccessUser.GetAsync(login);
            if (user!= null && user.IdUser!=int.Parse(claims[1].Value))
            {
                apiAccessUserProject.UrlAPI = "users/adddevelopinproject";
                UserProject newUserProject = new UserProject()
                {
                    IdProject = idProject,
                    IdUser = user.IdUser,
                    Role = (int)Role.Developer
                };
                await apiAccessUserProject.AddAsync(newUserProject);
            }
            return RedirectToAction("Project", new { id = idProject });
        }
        
        [HttpPost]
        public async Task<IActionResult> AddTask(MyTask task, int idProject)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                List<Claim> claims = claimsIdentity.Claims.ToList();
                var newTask = new ExtendedTask()
                {
                    IdUser = int.Parse(claims[1].Value),
                    IdProject = idProject,
                    Task = task
                };
                newTask.Task.DateStart = DateTime.Now;
                apiAccessExtendedTask.UrlAPI = "tasks/add";
                await apiAccessExtendedTask.AddAsync(newTask);
            }
            return RedirectToAction("Project", new { id = idProject });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask(MyTask task, int idProject)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                List<Claim> claims = claimsIdentity.Claims.ToList();
                apiAccessTask.UrlAPI = "tasks/update";
                await apiAccessTask.UpdateAsync(task);
            }
            return RedirectToAction("Project", new { id = idProject });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask(int idTask, int idProject)
        {
            apiAccessTask.UrlAPI = "tasks";
            await apiAccessTask.DeleteAsync(idTask);
            return RedirectToAction("Project", new { id = idProject });
        }
    }
}
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
    public class TasksController : Controller
    {
        private readonly WebAPIRepository<Project> apiAccessProject = new WebAPIRepository<Project>("projects");
        private readonly WebAPIRepository<User> apiAccessUser = new WebAPIRepository<User>("users");
        private readonly WebAPIRepository<ExtendedUser> apiAccessExtendedUser = new WebAPIRepository<ExtendedUser>("users");
        private readonly WebAPIRepository<UserProject> apiAccessUserProject = new WebAPIRepository<UserProject>("users");
        private readonly WebAPIRepository<MyTask> apiAccessTask = new WebAPIRepository<MyTask>("tasks");
        private readonly WebAPIRepository<ProjectTask> apiAccessProjectTask = new WebAPIRepository<ProjectTask>("projects");
        private readonly WebAPIRepository<ExtendedTask> apiAccessExtendedTask = new WebAPIRepository<ExtendedTask>("task");
        private readonly WebAPIRepository<ExtendedComment> apiAccessExtendedComment = new WebAPIRepository<ExtendedComment>("tasks");
        private readonly WebAPIRepository<Comment> apiAccessComment = new WebAPIRepository<Comment>("tasks");


        public async Task<IActionResult> MyTasks()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            List<Claim> claims = claimsIdentity.Claims.ToList();
            apiAccessTask.UrlAPI = "tasks/" + claims[1].Value.ToString()+ "/mytasks";
            ViewData["MyTasks"] = (await apiAccessTask.GetAllAsync()).OrderBy(el=> el.Priority);
            return View();
        }

        [HttpGet("task/{id}")]
        public async Task<IActionResult> Task(int id)
        {
            apiAccessTask.UrlAPI = "tasks";
            ViewData["Task"] = await apiAccessTask.GetAsync(id.ToString());
            apiAccessProject.UrlAPI = "projects/task";
            var p = await apiAccessProject.GetAsync(id.ToString());
            ViewData["Project"] = p;
            var claimsIdentity = User.Identity as ClaimsIdentity;
            List<Claim> claims = claimsIdentity.Claims.ToList();
            apiAccessUserProject.UrlAPI = "users/" + claims[1].Value + "/project";
            var newEl = await apiAccessUserProject.GetAsync(p.IdProject.ToString());
            ViewData["MyRole"] = newEl.Role;
            apiAccessExtendedUser.UrlAPI = "users/extuserbytask/" + id.ToString();
            ViewData["Users"] = await apiAccessExtendedUser.GetAllAsync();
            apiAccessExtendedUser.UrlAPI = "users/" + p.IdProject.ToString() + "/extendedusers";
            ViewData["PossibleUsers"] = await apiAccessExtendedUser.GetAllAsync();
            apiAccessExtendedComment.UrlAPI = "tasks/" + id.ToString() + "/comments";
            ViewData["Comments"] = await apiAccessExtendedComment.GetAllAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(int idUser, int idTask, int idProject)
        {
            apiAccessProjectTask.UrlAPI = "tasks/addprojecttask";
            await apiAccessProjectTask.AddAsync(new ProjectTask
            {
                IdProject = idProject,
                IdTask = idTask,
                IdUser = idUser,
                DateStart = DateTime.Now
            });

            return RedirectToAction("Task", new { id = idTask });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int idUser, int idTask)
        {
            apiAccessProjectTask.UrlAPI = "tasks/"+idTask.ToString()+"/user/"+idUser.ToString();
            var entity = await apiAccessProjectTask.GetAsync("");
            apiAccessProjectTask.UrlAPI = "tasks/deleteuser";
            await apiAccessProjectTask.DeleteAsync(entity.Id);

            return RedirectToAction("Task", new { id = idTask });
        }

        [HttpPost]
        public async Task<IActionResult> Done(int idTask)
        {
            apiAccessTask.UrlAPI = "tasks";
            var t = await apiAccessTask.GetAsync(idTask.ToString());
            apiAccessTask.UrlAPI = "tasks/update";
            if (t.Status == (int)Status.Active)
            {
                t.DateEnd = DateTime.Now;
                t.Status = (int)Status.Сompleted;
            }
            else
            {
                t.Status = (int)Status.Active;
                t.DateEnd = null;
            }


            await apiAccessTask.UpdateAsync(t);

            return RedirectToAction("Task", new { id = idTask });
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(Comment cm)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            List<Claim> claims = claimsIdentity.Claims.ToList();
            cm.IdUser = int.Parse(claims[1].Value);
            cm.DateCreate = DateTime.Now;
            apiAccessComment.UrlAPI = "tasks/addcomment";
            await apiAccessComment.AddAsync(cm);

            return RedirectToAction("Task", new { id = cm.IdTask });
        }
    }



}
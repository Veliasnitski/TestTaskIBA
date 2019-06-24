using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebManagerTasks.Data.Models;
using WebManagerTasks.ViewModels;
using WebManagerTasks.Services;

namespace WebManagerTasks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IService<Project> projectService;
        private readonly IService<User> userService;
        private readonly IService<UserProject> userProjectService;
        private readonly IService<ProjectTask> projectTaskService;
        public ProjectsController(
            IService<Project> projectService,
            IService<UserProject> userProjectService,
            IService<User> userService,
            IService<ProjectTask> projectTaskService
            )
        {
            this.projectService = projectService;
            this.userProjectService = userProjectService;
            this.userService = userService;
            this.projectTaskService = projectTaskService;
        }
        // GET: api/Projects
        [HttpGet]
        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await projectService.GetAllAsync();
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<Project> GetAsync([FromRoute]int id)
        {
            return await projectService.GetAsync(el => el.IdProject == id);
        }

        [HttpGet("task/{idTask}")]
        public async Task<Project> GetProjectByIdTaskAsync([FromRoute]int idTask)
        {
            var taskProject = await projectTaskService.GetAsync(el => el.IdTask == idTask);
            return await projectService.GetAsync(el => el.IdProject == taskProject.IdProject);
        }

        [HttpPost("add")]
        public async Task AddProject([FromBody] ExtendedProject item)
        {
            await projectService.AddAsync(item.Project);
            Project el = await projectService.GetAsync(p => item.Project.Description == p.Description
                && item.Project.Name == p.Name
                && item.Project.Status == p.Status);

            await userProjectService.AddAsync(
                new UserProject { IdUser = item.AdminId, IdProject = el.IdProject, Role = (int)Role.Administrator });
        }
        // POST: api/Projects
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Projects/update
        [HttpPut("update")]
        public void UpdateProject([FromBody] Project value)
        {
            projectService.UpdateAsync(value);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            projectService.DeleteAsync(id);
        }
    }
}

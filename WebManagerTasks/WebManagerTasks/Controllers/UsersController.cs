using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebManagerTasks.Data.Models;
using WebManagerTasks.Data.Repositories;
using WebManagerTasks.ViewModels;
using WebManagerTasks.Services;

namespace WebManagerTasks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IService<User> userService;
        private readonly IService<UserProject> userProjectService;
        private readonly IService<Project> projectService;
        private readonly IService<ProjectTask> projectTaskService;
        private object sha256;

        public UsersController(
            IService<User> userService,
            IService<UserProject> userProjectService,
            IService<Project> projectService,
            IService<ProjectTask> projectTaskService
            )
        {
            this.userService = userService;
            this.userProjectService = userProjectService;
            this.projectService = projectService;
            this.projectTaskService = projectTaskService;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await userService.GetAllAsync();
        }

        [HttpGet("{id}/projects")]
        public async Task<IEnumerable<ExtendedProject>> GetProjectsByIdUserAsync([FromRoute]int id)
        {
            var list = await userProjectService.GetAllAsync(el => el.IdUser == id);
            List<ExtendedProject> result = new List<ExtendedProject>();
            foreach (var item in list)
            {
                Project prj = await projectService.GetAsync(el => item.IdProject == el.IdProject);
                ExtendedProject tmp = new ExtendedProject()
                {
                    Project = prj,
                    AdminId = (await userProjectService.GetAsync(el =>
                                         el.IdProject == prj.IdProject &&
                                         el.Role == (int)Role.Administrator)).IdUser
                };

                result.Add(tmp);
            }
            return result;
        }


        [HttpGet("{idProject}/extendedusers")]
        public async Task<List<ExtendedUser>> GetLoginsByIdProjectAsync(int idProject)
        {
            var users = await userProjectService.GetAllAsync(el => el.IdProject == idProject);
            var result = new List<ExtendedUser>();
            foreach (var item in users)
            {
                User tmp = await userService.GetAsync(el => el.IdUser == item.IdUser);
                result.Add(new ExtendedUser()
                {
                    IdProject = idProject,
                    Role = item.Role,
                    User = tmp
                });
            }

            return result;
        }
        

        [HttpGet("{id}/project/{idProject}")]
        public async Task<UserProject> GetUserProjectsByIdUserIdProjectAsync([FromRoute]int id, int idProject)
        {
            return await userProjectService.GetAsync(el => el.IdUser == id && el.IdProject == idProject);
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public async Task<User> GetAsync([FromRoute]int id)
        {
            return await userService.GetAsync(el => el.IdUser == id);
        }

        [HttpGet("login/{login}")]
        public async Task<User> GetUsersByLogin([FromRoute]string login)
        {
            return await userService.GetAsync(el => el.Login == login); ;
        }

        [HttpGet("extuserbytask/{idTask}")]
        public async Task<List<ExtendedUser>> GetUsersByIdTask([FromRoute]int idTask)
        {
            var projectTasks = await projectTaskService.GetAllAsync(el => el.IdTask == idTask);
            List<ExtendedUser> users = new List<ExtendedUser>();
            foreach (var item in projectTasks)
            {
                User user = await userService.GetAsync(el => el.IdUser == item.IdUser);
                users.Add(new ExtendedUser() {
                    User = user,
                    IdProject = item.IdProject,
                    Role = (await userProjectService.GetAsync(el=> 
                        el.IdUser==user.IdUser &&
                        el.IdProject == item.IdProject)).Role
                });
            }
            return users;
        }

        [HttpPost("adddevelopinproject")]
        public async Task AddUserProject([FromBody]UserProject item)
        {
            var user = await userService.GetAsync(el => el.IdUser == item.IdUser);
            var userProject = await userProjectService.GetAsync(
                el => el.IdProject == item.IdProject &&
                el.IdUser == item.IdUser);
            if (userProject == null) await userProjectService.AddAsync(item);
            else await userProjectService.UpdateAsync(item);
        }

        [HttpPost]
        public void AddUser([FromBody] User user)
        {
            var sha256 = new SHA256Managed();
            user.Password = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(user.Password)));
            userService.AddAsync(user);
        }


        [HttpDelete("{id}")]
        public async Task DeleteAsync(int id)
        {
            await userService.DeleteAsync(id);
        }

        [HttpDelete("userproject/{id}")]
        public async Task UserProjectDeleteAsync(int id)
        {
            await userProjectService.DeleteAsync(id);
        }
    }
}

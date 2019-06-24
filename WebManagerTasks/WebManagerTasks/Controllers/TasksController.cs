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
    public class TasksController : ControllerBase
    {
        private readonly IService<Project> projectService;
        private readonly IService<User> userService;
        private readonly IService<MyTask> taskService;
        private readonly IService<ProjectTask> projectTaskService;
        private readonly IService<Comment> commentService;
        public TasksController(
            IService<Project> projectService,
            IService<MyTask> taskService,
            IService<User> userService,
            IService<ProjectTask> projectTaskService,
            IService<Comment> commentService
            )
        {
            this.projectService = projectService;
            this.taskService = taskService;
            this.userService = userService;
            this.projectTaskService = projectTaskService;
            this.commentService = commentService;
        }

        [HttpPost("add")]
        public async Task AddProjectAsync([FromBody] ExtendedTask item)
        {
            await taskService.AddAsync(item.Task);
            var task = await taskService.GetAsync(el =>
                el.Name == item.Task.Name &&
                el.Description == item.Task.Description &&
                el.DateStart == item.Task.DateStart &&
                el.Status == item.Task.Status);
            ProjectTask tmp = await projectTaskService.GetAsync(el =>
                item.IdUser == el.IdUser &&
                item.IdProject == el.IdProject &&
                task.IdTask == el.IdTask);
            if (tmp == null)
            {
                await projectTaskService.AddAsync(new ProjectTask
                {
                    IdProject = item.IdProject,
                    IdUser = item.IdUser,
                    DateStart = item.Task.DateStart,
                    IdTask = task.IdTask
                });
            }
        }

        [HttpPost("addprojecttask")]
        public async Task AddProjectAsync([FromBody] ProjectTask item)
        {
            var entity = await projectTaskService.GetAsync(el =>
                el.IdProject == item.IdProject &&
                el.IdTask == item.IdTask &&
                el.IdUser == item.IdUser
            );
            if (entity == null)
            {
                await projectTaskService.AddAsync(item);
            }
        }

        [HttpPost("addcomment")]
        public async Task AddCommentAsync([FromBody] Comment item)
        {
            await commentService.AddAsync(item);
        }

        [HttpGet("{id}")]
        public async Task<MyTask> GetTaskAsync([FromRoute] int id)
        {
            return await taskService.GetAsync(el => el.IdTask == id);
        }

        [HttpGet("{idUser}/mytasks")]
        public async Task<IEnumerable<MyTask>> GetMyTasksByIdUserAsync([FromRoute] int idUser)
        {
            var users = await projectTaskService.GetAllAsync(el => el.IdUser == idUser);
            var result = new List<MyTask>();
            foreach(var item in users)
            {
                MyTask tmp = await taskService.GetAsync(el=> el.IdTask == item.IdTask && el.Status==(int)Status.Active);
                if (tmp!=null) result.Add(tmp);
            }
            return result;
        }

        [HttpGet("{id}/comments")]
        public async Task<List<ExtendedComment>> GetCommentsAsync([FromRoute] int id)
        {
            var comments = await commentService.GetAllAsync(el => el.IdTask == id);
            List<ExtendedComment> result = new List<ExtendedComment>();
            foreach (var comment in comments)
            {
                User tmp = await userService.GetAsync(el => el.IdUser == comment.IdUser);
                result.Add(new ExtendedComment() {
                    Comment = comment,
                    User = tmp
                }); 
            }
            return result;
        }

        [HttpGet("project/{idProject}/user/{idUser}")]
        public async Task<List<ExtendedTask>> GetTasksAsync([FromRoute] int idProject, int idUser)
        {
            var tasksProjects = await projectTaskService.GetAllAsync(el =>
               el.IdProject == idProject &&
               el.IdUser == idUser
            );
            var list = new List<ExtendedTask>();
            foreach (var item in tasksProjects)
            {
                list.Add(new ExtendedTask()
                {
                    Task = await taskService.GetAsync(el => el.IdTask == item.IdTask),
                    IdUser = idUser,
                    IdProject = idProject
                });
            }
            return list;
        }

        [HttpGet("{idTask}/user/{idUser}")]
        public async Task<ProjectTask> GetProjectTaskAsync([FromRoute] int idTask, int idUser)
        {
            return await projectTaskService.GetAsync(el => el.IdTask == idTask && el.IdUser == idUser);
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(int id)
        {
            await taskService.DeleteAsync(id);
        }

        [HttpDelete("deleteuser/{id}")]
        public async Task DeleteUserAsync(int id)
        {
            await projectTaskService.DeleteAsync(id);
        }

        [HttpPut("update")]
        public void UpdateTask([FromBody] MyTask task)
        {
            taskService.UpdateAsync(task);
        }
    }
}
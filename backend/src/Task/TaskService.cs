using System.Collections.Generic;
using System.Threading.Tasks;
using backend.src.ApplicationUser;

namespace backend.src.Task
{
    public class TaskService
    {
        private readonly TaskRepo _repo;

        public TaskService(TaskRepo repo)
        {
            _repo = repo;
        }

        public async Task<List<TaskEntity>> ListTasks()
        {
            return await _repo.GetAll();
        }

        public async Task<TaskEntity> GetTask(int id)
        {
            return await _repo.GetById(id);
        }

        public async Task<TaskEntity> CreateTask(TaskEntity task)
        {
            return await _repo.Add(task);
        }

        public async Task<bool> UpdateTask(int id, TaskEntity updatedTask)
        {
            return await _repo.Update(id, updatedTask);
        }

        public async Task<bool> DeleteTask(int id)
        {
            return await _repo.Delete(id);
        }

        public async Task<bool> AssignTask(int taskId, List<AppUser> assignees)
        {
            return await _repo.Assign(taskId, assignees);
        }
    }
}
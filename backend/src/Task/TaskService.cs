using System.Collections.Generic;
using backend.src.ApplicationUser;

namespace backend.src.Task
{
    public class TaskService
    {
        private readonly TaskRepo _repo = new TaskRepo();

        public List<TaskEntity> ListTasks()
        {
            return _repo.GetAll();
        }

        public TaskEntity GetTask(int id)
        {
            return _repo.GetById(id);
        }

        public TaskEntity CreateTask(TaskEntity task)
        {
            return _repo.Add(task);
        }

        public bool UpdateTask(int id, TaskEntity updatedTask)
        {
            return _repo.Update(id, updatedTask);
        }

        public bool DeleteTask(int id)
        {
            return _repo.Delete(id);
        }

        public bool AssignTask(int taskId, List<AppUser> assignees) // DÜZELTİN
        {
            return _repo.Assign(taskId, assignees);
        }
    }
}
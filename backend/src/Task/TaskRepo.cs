using System.Collections.Generic;
using System.Linq;

namespace api.src.Task
{
    public class TaskRepo
    {
        private static List<TaskEntity> _tasks = new List<TaskEntity>();
        private static int _nextId = 1;

        public List<TaskEntity> GetAll()
        {
            return _tasks;
        }

        public TaskEntity GetById(int id)
        {
            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        public TaskEntity Add(TaskEntity task)
        {
            task.Id = _nextId++;
            _tasks.Add(task);
            return task;
        }

        public bool Update(int id, TaskEntity updatedTask)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;

            task.Title = updatedTask.Title;
            task.Description = updatedTask.Description;
            task.Assignees = updatedTask.Assignees;
            task.StartDate = updatedTask.StartDate;
            task.EndDate = updatedTask.EndDate;
            task.CreatedBy = updatedTask.CreatedBy;
            return true;
        }

        public bool Delete(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;
            _tasks.Remove(task);
            return true;
        }

        public bool Assign(int taskId, List<User> assignees)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return false;
            task.Assignees = assignees;
            return true;
        }
    }
}
using System.Collections.Generic;
using api.src.User;
using System;

namespace api.src.Task
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

        public bool AssignTask(int taskId, List<UserEntity> assignees) // DÜZELTİN
        {
            return _repo.Assign(taskId, assignees);
        }

        public bool UpdateTitle(int id, string title)
        {
            var task = _repo.GetById(id);
            if (task == null) return false;
            task.Title = title;
            return _repo.Update(id, task);
        }

        public bool UpdateDescription(int id, string description)
        {
            var task = _repo.GetById(id);
            if (task == null) return false;
            task.Description = description;
            return _repo.Update(id, task);
        }

        public bool UpdateStartDate(int id, DateTime startDate)
        {
            var task = _repo.GetById(id);
            if (task == null) return false;
            task.StartDate = startDate;
            return _repo.Update(id, task);
        }

        public bool UpdateEndDate(int id, DateTime endDate)
        {
            var task = _repo.GetById(id);
            if (task == null) return false;
            task.EndDate = endDate;
            return _repo.Update(id, task);
        }

        public bool UpdateStatus(int id, TaskEntity.Status status)
        {
            var task = _repo.GetById(id);
            if (task == null) return false;
            task.Status = status;
            return _repo.Update(id, task);
        }
    }
}
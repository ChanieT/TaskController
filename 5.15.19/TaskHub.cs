using Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _5._15._19
{
    public class TaskHub : Hub
    {
        private string _conn;
        private TasksRepository _repo;
        public TaskHub(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("ConStr");
            _repo = new TasksRepository(_conn);
        }

        public void NewAssignment(Assignment assignment)
        {
            Assignment a = _repo.AddTask(assignment);
            Clients.All.SendAsync("NewTask", a);
        }

        public void AcceptAssignment(Assignment assignment)
        {
            Assignment a = _repo.AssignTaskToUser(assignment);
            Clients.Others.SendAsync("AssignmentGotAccepted", a);
            Clients.Caller.SendAsync("IAcceptedAssignment", a);
        }

        public void FinishedAssignment(Assignment assignment)
        {
            _repo.FinishedAssignment(assignment);
            Clients.All.SendAsync("RemoveFromList", assignment);
        }
    }
}

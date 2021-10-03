using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pw.Auth.Views
{
    public class UserTaskView
    {
        public UserTaskView() { }
        public UserTaskView(int userId, int taskId, string task, bool isSelected)
        {
            this.UserId = userId;
            this.TaskId = taskId;
            this.Task = task;
            this.IsSelected = isSelected;
        }
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public string Task { get; set; }
        public bool IsSelected { get; set; }

    }
}

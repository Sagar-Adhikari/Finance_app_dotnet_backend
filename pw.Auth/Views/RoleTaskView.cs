

namespace pw.Auth.Views
{
    public class RoleTaskView
    {
        public RoleTaskView() { }
        public RoleTaskView(int roleId, int taskId, string task, bool isSelected)
        {
            this.RoleId = roleId;
            this.TaskId = taskId;
            this.Task = task;
            this.IsSelected = isSelected;
        }
        public int RoleId { get; set; }
        public int TaskId { get; set; }
        public string Task { get; set; }
        public bool IsSelected { get; set; }
    }
}

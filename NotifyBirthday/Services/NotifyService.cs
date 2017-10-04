using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Controls;

namespace NotifyBirthday
{
    class NotifyService
    {
        public TaskbarIcon icon = new TaskbarIcon
        {
            ToolTipText = "NotifyBirthday",
            ContextMenu = new ContextMenu(),
            Icon = new System.Drawing.Icon("Error.ico")
        };
    }
}

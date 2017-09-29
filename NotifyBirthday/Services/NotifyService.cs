using Hardcodet.Wpf.TaskbarNotification;
using System.Threading;
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

        public void Notify(string message)
        {
            icon.ShowBalloonTip("Оповещение", message + "\nНажмите на облако, чтобы больше не видеть это оповещение.", BalloonIcon.Info);
            icon.CloseBalloon();
        }
    }
}

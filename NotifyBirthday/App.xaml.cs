using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NotifyBirthday
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow mainWindow;
        private MainWindowViewModel viewModel;
        private void AppStartup(object sender, StartupEventArgs e)
        {
            viewModel = new MainWindowViewModel
            {
                Employees = DataManager.Load<ObservableCollection<Employee>>("Employees.xml")
            };
            mainWindow = new MainWindow
            {
                DataContext = viewModel
            };
            viewModel.window = mainWindow;
            mainWindow.ResizeMode = ResizeMode.CanMinimize;
            mainWindow.Closed += MainWindow_Closed;
            mainWindow.Closing += MainWindow_Closing;
            mainWindow.StateChanged += MainWindow_StateChanged;
            mainWindow.Show();
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (mainWindow.WindowState == WindowState.Minimized) mainWindow.Hide();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (mainWindow.Visibility == Visibility.Hidden)
            {
                Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
                mainWindow.WindowState = WindowState.Minimized;
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            viewModel.Disponse();
        }
    }
}

using MVVM1;
using NetworkService.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Actions
{
    public class NavigationAction : IUndoableAction
    {
        private readonly MainWindowViewModel _main;
        private readonly BindableBase _previousViewModel;

        public NavigationAction(MainWindowViewModel main, BindableBase previousViewModel)
        {
            _main = main;
            _previousViewModel = previousViewModel;
        }

        public void Undo()
        {
            _main.CurrentViewModel = _previousViewModel;
        }
    }
}

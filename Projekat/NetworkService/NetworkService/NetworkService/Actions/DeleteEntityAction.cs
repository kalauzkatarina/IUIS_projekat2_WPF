using NetworkService.Model;
using NetworkService.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Actions
{
    public class DeleteEntityAction : IUndoableAction
    {
        private ObservableCollection<DailyTraffic> _entities;
        private DailyTraffic _entity;

        public DeleteEntityAction(ObservableCollection<DailyTraffic> entities, DailyTraffic entity)
        {
            _entities = entities;
            _entity = entity;
        }

        public void Undo()
        {
            _entities.Add(_entity);
        }
    }

}

using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Actions
{
    public class AddEntityAction : IUndoableAction
    {
        private ObservableCollection<DailyTraffic> _entities;
        private DailyTraffic _entity;

        public AddEntityAction(ObservableCollection<DailyTraffic> entities, DailyTraffic entity)
        {
            _entities = entities;
            _entity = entity;
        }

        public void Undo()
        {
            _entities.Remove(_entity);
        }
    }
}

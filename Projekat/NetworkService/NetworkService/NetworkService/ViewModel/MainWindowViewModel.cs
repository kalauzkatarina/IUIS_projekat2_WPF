using MVVM1;
using NetworkService.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private int count = 15; // Inicijalna vrednost broja objekata u sistemu
                                // ######### ZAMENITI stvarnim brojem elemenata
                                //           zavisno od broja entiteta u listi

        private Stack<IUndoableAction> _undoStack = new Stack<IUndoableAction>();

        public HomeViewModel HomeViewModel { get; set; }
        public NetworkEntitiesViewModel NetworkEntitiesViewModel { get; set; }
        public NetworkDisplayViewModel NetworkDisplayViewModel {  get; set; }
        public MeasurementGraphViewModel MeasurementGraphViewModel {  get; set; }
        public MyICommand UndoCommand { get; set; }
        public MyICommand HomeCommand { get; set; }

        private BindableBase _currentViewModel;
        public BindableBase CurrentViewModel
        {
            get 
            { 
                return _currentViewModel;
            }
            set
            {
                if (_currentViewModel != null && value != _currentViewModel)
                {
                    // Dodaj navigaciju u undo stack
                    _undoStack.Push(new NavigationAction(this, _currentViewModel));
                }

                SetProperty(ref _currentViewModel, value);

            }
        }

        public MainWindowViewModel()
        {
            createListener(); //Povezivanje sa serverskom aplikacijom
            HomeViewModel = new HomeViewModel();
            NetworkEntitiesViewModel = new NetworkEntitiesViewModel();
            MeasurementGraphViewModel = new MeasurementGraphViewModel(NetworkEntitiesViewModel);
            NetworkDisplayViewModel = new NetworkDisplayViewModel(NetworkEntitiesViewModel);
            HomeViewModel.SetMainWindowReference(this);
            NetworkEntitiesViewModel.SetMainWindowReference(this);
            NetworkDisplayViewModel.SetMainWindowReference(this);
            MeasurementGraphViewModel.SetMainWindowReference(this);
            CurrentViewModel = HomeViewModel;

            UndoCommand = new MyICommand(OnUndo);
            HomeCommand = new MyICommand(OnHome);

            NetworkEntitiesViewModel.EntitiesChanged += () =>
            {
                NotifySimulatorToUpdateCount();
            };

            NetworkEntitiesViewModel.ActionPerformed += (action) =>
            {
                _undoStack.Push(action);
            };
        }

        private void OnUndo()
        {
            if(_undoStack.Count > 0)
            {
                var action = _undoStack.Pop();
                action.Undo();
            }
        }

        private void OnHome()
        {
            CurrentViewModel = HomeViewModel;
        }

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25675);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            int entityCount = NetworkEntitiesViewModel?.Entities?.Count ?? count;
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(entityCount.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"
                            ProcessIncomingMessage(incomming);
                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji

                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

        private void ProcessIncomingMessage(string message)
        {
            try
            {
                string[] parts = message.Split(':');
                if (parts.Length != 2) return;

                string entityName = parts[0];
                if (!int.TryParse(parts[1], out int newValue)) return;

                var entity = NetworkEntitiesViewModel.Entities.FirstOrDefault(e => e.Name.Replace(" ", "_") == entityName);

                if(entity != null)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        entity.LastValue = newValue;
                        MeasurementGraphViewModel.AddMeasurementRealTime(entity.Name.Replace(" ", "_"), newValue);
                    });
                }

                LogMeasurement(entityName, newValue);
            } catch (Exception ex)
            {
                Console.WriteLine("Error while processing the message: " + ex.Message);
            }
        }

        private void LogMeasurement(string entityName, int value)
        {
            string logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {entityName} | {value}";
            string logPath = "log.txt"; 
            try
            {
                File.AppendAllText(logPath, logLine + Environment.NewLine);
            } catch (Exception ex)
            {
                Console.WriteLine("Error while writing into the log.txt file: " + ex.Message);
            }
        }
            
        //DODATO    
        private void NotifySimulatorToUpdateCount()
        {
            Task.Run(() =>
            {
                try
                {
                    using (var client = new TcpClient("localhost", 25676))
                    using (var stream = client.GetStream())
                    {
                        byte[] data = Encoding.ASCII.GetBytes("UpdateCount");
                        stream.Write(data, 0, data.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error notifying simulator: " + ex.Message);
                }
            });
        }
    }
}

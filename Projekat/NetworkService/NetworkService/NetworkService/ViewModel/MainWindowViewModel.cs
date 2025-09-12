using MVVM1;
using System;
using System.Collections.Generic;
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
                SetProperty(ref _currentViewModel, value);

            }
        }

        public MainWindowViewModel()
        {
            createListener(); //Povezivanje sa serverskom aplikacijom
            HomeViewModel = new HomeViewModel();
            NetworkEntitiesViewModel = new NetworkEntitiesViewModel();
            NetworkDisplayViewModel = new NetworkDisplayViewModel();
            MeasurementGraphViewModel = new MeasurementGraphViewModel();
            HomeViewModel.SetMainWindowReference(this);
            CurrentViewModel = HomeViewModel;

            UndoCommand = new MyICommand(OnUndo);
            HomeCommand = new MyICommand(OnHome);
        }

        private void OnUndo()
        {

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
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

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
    }
}

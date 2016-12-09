using loadTestTool.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Timer = System.Timers.Timer;

namespace loadTestTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Header> _headers = new ObservableCollection<Header>();
        private string _headerValue;
        private string _headerName;
        private int _clientsCount;
        private int _averageTime;
        private int _received;
        private int _receivedMbps;
        private int _errorsCount;
        private int _workingParralelClientsCount;
        private string _buttonText = "Start test";
        private Timer _timer;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (persecondLocker)
            {
                WorkingParralelClientsCount = GetClientsPerSecond();
                ReceivedMbps = GetBytesPerSecond() / 1024;
                AverageTime = GetTimePerRequest() / WorkingParralelClientsCount;
            }
        }

        private int GetClientsPerSecond()
        {
            {
                var temp = requestPerSecond;
                requestPerSecond = 0;
                return temp;
            }
        }

        private int GetTimePerRequest()
        {
            {
                var temp = (int)timePerRequest;
                timePerRequest = 0;
                return temp;
            }
        }

        private int GetBytesPerSecond()
        {
            {
                var temp = bytesPerSecond;
                bytesPerSecond = 0;
                return temp;
            }
        }

        public ObservableCollection<Result> Results
        {
            get { return _results; }
            set { _results = value; }
        }

        public ObservableCollection<Header> Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public string HeaderName
        {
            get { return _headerName; }
            set
            {
                if (value == _headerName) return;
                _headerName = value;
                OnPropertyChanged();
            }
        }

        public string HeaderValue
        {
            get { return _headerValue; }
            set
            {
                if (value == _headerValue) return;
                _headerValue = value;
                OnPropertyChanged();
            }
        }

        public int WorkingParralelClientsCount
        {
            get { return _workingParralelClientsCount; }
            set
            {
                if (value == _workingParralelClientsCount) return;
                _workingParralelClientsCount = value;
                OnPropertyChanged();
            }
        }

        public int ClientsCount
        {
            get { return _clientsCount; }
            set
            {
                if (value == _clientsCount) return;
                _clientsCount = value;
                OnPropertyChanged();
            }
        }

        public int AverageTime
        {
            get { return _averageTime; }
            set
            {
                if (value == _averageTime) return;
                _averageTime = value;
                OnPropertyChanged();
            }
        }

        public int Received
        {
            get { return _received; }
            set
            {
                if (value == _received) return;
                _received = value;
                OnPropertyChanged();
            }
        }

        public int ReceivedMbps
        {
            get { return _receivedMbps; }
            set
            {
                if (value == _receivedMbps) return;
                _receivedMbps = value;
                OnPropertyChanged();
            }
        }

        public int ErrorsCount
        {
            get { return _errorsCount; }
            set
            {
                if (value == _errorsCount) return;
                _errorsCount = value;
                OnPropertyChanged();
            }
        }

        public string ButtonText
        {
            get { return _buttonText; }
            set
            {
                if (value == _buttonText) return;
                _buttonText = value;
                OnPropertyChanged();
            }
        }

        private void DeleteHeaderClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            Headers.Remove(btn.DataContext as Header);
        }

        private void AddHeader(object sender, RoutedEventArgs e)
        {
            Headers.Add(new Header() { HeaderName = HeaderName, HeaderValue = HeaderValue });
            HeaderName = "";
            HeaderValue = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool isrunning = false;
        private ObservableCollection<Result> _results = new ObservableCollection<Result>();
        private string _url = "http://dsgdsg.eu:3000";

        private void StartClick(object sender, RoutedEventArgs e)
        {
            isrunning = !isrunning;
            if (isrunning)
            {
                _timer.Start();
                ButtonText = "Stop test";
                Thread thread = new Thread(StartRunningTest);
                thread.Start();
            }
            else
            {
                ButtonText = "Start test";
                _timer.Stop();
            }
        }

        private void StartRunningTest(object obj)
        {
            for (int i = 0; i < ClientsCount; i++)
            {
                Thread thead = new Thread(RunClient);
                thead.Start();
            }
        }

        private double timePerRequest = 0;
        private int requestPerSecond = 0;
        private int bytesPerSecond = 0;

        private void RunClient(object obj)
        {
            while (isrunning)
            {
                HttpWebRequest client = HttpWebRequest.CreateHttp(Url);
                foreach (var header in Headers)
                {
                    client.Headers[header.HeaderName] = header.HeaderValue;
                }
                client.Method = "GET";


                var startTime = DateTime.UtcNow;
                try
                {
                    using (var responce = client.GetResponse())
                    {
                        StreamReader stream = new StreamReader(responce.GetResponseStream());
                        var result = stream.ReadToEnd();
                        stream.Close();
                        var endTime = DateTime.UtcNow;
                        UpdateStatsPerSecond(result.Length, (endTime - startTime).TotalMilliseconds, 1);
                        //Dispatcher.BeginInvoke(new Action(() =>
                        //{
                        //    Results.Insert(0, new Result() { Time = (endTime - startTime).TotalMilliseconds, Status = "ok" });
                        //}));
                    }
                }
                catch (Exception e)
                {
                    var endTime = DateTime.UtcNow;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Results.Insert(0, new Result() { Time = (endTime - startTime).TotalMilliseconds, Status = e.Message });
                    }));
                }
            }
        }

        private object persecondLocker = new object();

        private void UpdateStatsPerSecond(int i, double ms, int clientCount)
        {
            lock (persecondLocker)
            {
                bytesPerSecond += i;
                requestPerSecond += clientCount;
                timePerRequest += ms;
            }
        }
    }
}
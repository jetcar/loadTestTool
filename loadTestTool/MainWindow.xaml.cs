using loadTestTool.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        public MainWindow()
        {
            InitializeComponent();
        }

        public ObservableCollection<Header> Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }

        public string Url { get; set; }

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

        private void StartClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ViscoveryDemoPOS.Domain;
using ViscoveryDemoPOS.Services;
using ViscoveryDemoPOS.BLL;
using ViscoveryDemoPOS.DAL;

namespace ViscoveryDemoPOS.ViewModels
{
    /// <summary>
    /// Main view-model orchestrating interactions between the UI, VisAgent
    /// service and local repositories.  Exposes commands used by the demo
    /// application to initialize services, load sample QR codes and process
    /// checkout results.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly VisAgentApiClient _api;
        private readonly PosCallbackServer _server;
        private readonly IRecognitionLogRepository _logRepo;

        /// <summary>List of items displayed on the UI grid.</summary>
        public ObservableCollection<ProductItem> Items { get; } = new ObservableCollection<ProductItem>();

        /// <summary>Message shown on the banner area.</summary>
        public string BannerMessage { get; set; }

        /// <summary>Flag indicating whether the banner should display success styling.</summary>
        public bool IsSuccessBanner { get; set; }

        /// <summary>Indicates whether the home screen is currently visible.</summary>
        public bool IsHome { get; set; } = true;

        public ICommand InitCommand { get; private set; }
        public ICommand GetQrCodeCommand { get; private set; }
        public ICommand StartViscoveryCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }
        public ICommand StartScanningCommand { get; private set; }

        private QrOrder _currentOrder;

        /// <summary>
        /// Initializes the view-model and wires up commands and services.
        /// </summary>
        public MainViewModel()
        {
            _api = new VisAgentApiClient();
            _server = new PosCallbackServer();
            _logRepo = new RecognitionLogRepository();
            _server.OnCheckoutReceived += OnCheckout;

            InitCommand = new RelayCommand(async () => await InitAsync());
            GetQrCodeCommand = new RelayCommand(() => LoadDemoQr());
            StartViscoveryCommand = new RelayCommand(async () => await StartViscoveryAsync(), () => _currentOrder != null);
            ResetCommand = new RelayCommand(() => Reset());
            StartScanningCommand = new RelayCommand(() => { IsHome = false; RaisePropertyChanged(nameof(IsHome)); });
        }

        /// <summary>
        /// Initializes external services and configures VisAgent.
        /// </summary>
        private async Task InitAsync()
        {
            _server.Start();
            var ok = await _api.HealthAsync();
            if (!ok)
            {
                BannerMessage = "VisAgent 未就緒";
                IsSuccessBanner = false;
                RaisePropertyChanged(nameof(BannerMessage));
                RaisePropertyChanged(nameof(IsSuccessBanner));
                return;
            }
            await _api.ConfigureAsync("http://127.0.0.1:8080");
        }

        /// <summary>
        /// Loads a predefined QR order used for demonstration purposes.
        /// </summary>
        private void LoadDemoQr()
        {
            _currentOrder = new QrOrder
            {
                OrderId = Guid.NewGuid().ToString("N"),
                ExpectedItems =
                {
                    new ProductItem { Code = "1111", Name = "華堡", Status = RecognizeStatus.Waiting },
                    new ProductItem { Code = "2222", Name = "牛肉堡", Status = RecognizeStatus.Waiting }
                }
            };
            RefreshGrid(_currentOrder.ExpectedItems);
        }

        /// <summary>
        /// Invokes the VisAgent unified recognition endpoint.
        /// </summary>
        private async Task StartViscoveryAsync()
        {
            BannerMessage = "啟動影像辨識...";
            RaisePropertyChanged(nameof(BannerMessage));
            await _api.UnifiedRecognitionAsync(true, null);
        }

        /// <summary>
        /// Handles checkout callbacks from the POS system, merges the recognized
        /// items with the expected order and logs the results.
        /// </summary>
        private void OnCheckout(System.Collections.Generic.List<CheckoutItem> items)
        {
            var recognized = items
                .Select(i => new ProductItem { Code = i.product_code, Name = i.product_name, Status = RecognizeStatus.Confirm })
                .ToList();

            var merged = RecognitionComparer.MergeAndMark(_currentOrder, recognized);
            RefreshGrid(merged);

            foreach (var it in merged)
            {
                _logRepo.Log(_currentOrder.OrderId, it);
            }

            IsSuccessBanner = merged.All(p => p.Status == RecognizeStatus.Confirm);
            BannerMessage = IsSuccessBanner ? "成功掃描" : "掃描錯誤";
            RaisePropertyChanged(nameof(BannerMessage));
            RaisePropertyChanged(nameof(IsSuccessBanner));
        }

        /// <summary>
        /// Updates the observable collection bound to the UI grid.
        /// </summary>
        private void RefreshGrid(System.Collections.Generic.IEnumerable<ProductItem> list)
        {
            Items.Clear();
            foreach (var it in list) Items.Add(it);
            RaisePropertyChanged(nameof(Items));
        }

        /// <summary>
        /// Clears the current state and returns to the home screen.
        /// </summary>
        private void Reset()
        {
            _currentOrder = null;
            Items.Clear();
            BannerMessage = string.Empty;
            IsHome = true;
            RaisePropertyChanged(nameof(BannerMessage));
            RaisePropertyChanged(nameof(IsHome));
        }
    }
}

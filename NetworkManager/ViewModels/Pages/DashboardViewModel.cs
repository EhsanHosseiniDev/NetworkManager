using NetworkManager.Applications;
using NetworkManager.Domain;
using NetworkManager.Domain.Exeptions;
using System.Windows.Threading;
using Wpf.Ui.Controls;

namespace NetworkManager.ViewModels.Pages;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IVpnHandler _vpnHandler;
    private readonly IXrayApiService _xrayApiService;
    private DispatcherTimer _timer;

    [ObservableProperty]
    private bool _isVpnRunning;

    [ObservableProperty]
    private string _buttonText = "Start";

    [ObservableProperty]
    private List<TrafficStatDto> _userStatus = new List<TrafficStatDto>();

    [ObservableProperty]
    private ControlAppearance _statusColor = ControlAppearance.Primary;

    [ObservableProperty]
    private SymbolRegular _statusIcon = SymbolRegular.Play24;

    public DashboardViewModel(IVpnHandler vpnHandler, IXrayApiService xrayApiService)
    {
        _vpnHandler = vpnHandler;
        _xrayApiService = xrayApiService;
        InitializeTimer();
    }

    private void InitializeTimer()
    {
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(3);
        _timer.Tick += UpdateVpnStatusAsync;
    }

    private void UpdateVpnStatusAsync(object? sender, EventArgs e)
    {
        try
        {
            var stats = _xrayApiService.GetTrafficStatsAsync().Result;
            UpdateTrafficList(stats);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching stats: {ex.Message}");
        }
    }

    private void UpdateTrafficList(IEnumerable<TrafficStatDto> newStats)
    {
        _userStatus.Clear();
        foreach (var stat in newStats)
        {
            _userStatus.Add(stat);
        }
    }

    [RelayCommand]
    private async Task ToggleVpn()
    {
        if (IsVpnRunning)
        {
            await _vpnHandler.StopAsync();
            //_timer.Stop();
            IsVpnRunning = false;
            ButtonText = "Start";
            StatusColor = ControlAppearance.Primary;
            StatusIcon = SymbolRegular.Play24;
        }
        else
        {

            try
            {
                ButtonText = "Starting...";
                await _vpnHandler.StartAsync();
                //_timer.Start();
                IsVpnRunning = true;
                ButtonText = "Stop ";
                StatusColor = ControlAppearance.Danger;
                StatusIcon = SymbolRegular.Stop24;
            }
            catch (TelegramBotApiNotValidExeption tEx)
            {
                System.Windows.MessageBox.Show("Please set telegram bot api in setting page", "Telegram bot api", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                var iste = ex is TelegramBotApiNotValidExeption;
                ButtonText = "Start";
            }
        }
    }
}

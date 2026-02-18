using NetworkManager.Applications;
using NetworkManager.Domain.Aggregates.Users;
using NetworkMangar.Infrastructure.Services.CloudflerTunnel;
using System.Collections.ObjectModel;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;
using User = NetworkManager.Domain.Aggregates.Users.User;

namespace NetworkManager.ViewModels.Pages;

public partial class UserManagerViewModel : ObservableObject, INavigationAware
{
    public UserManagerViewModel(IUserRepository userRepository,
        IQuickTunnelService quickTunnelService, IVpnHandler vpnHandler)
    {
        _userRepository = userRepository;
        _quickTunnelService = quickTunnelService;
        _vpnHandler = vpnHandler;
        Users = new ObservableCollection<User>();
    }

    [ObservableProperty]
    private ObservableCollection<User> _users;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveUserCommand))]
    private string _newUsername = string.Empty;

    [ObservableProperty]
    private string _newTelegramId = string.Empty;

    [ObservableProperty]
    private long _newMonthlyLimitGb = 10;
    private int? _editingUserId = null;
    public bool IsEditing => _editingUserId.HasValue;
    public string SubmitButtonText => IsEditing ? "Update User" : "Create User";
    public SymbolRegular SubmitButtonIcon => IsEditing ? SymbolRegular.Save24 : SymbolRegular.Add24;

    private readonly IUserRepository _userRepository;
    private readonly IQuickTunnelService _quickTunnelService;
    private readonly IVpnHandler _vpnHandler;

    [RelayCommand]
    private async Task AddUser()
    {
        if (string.IsNullOrWhiteSpace(NewUsername)) return;

        var newUser = new User
        {
            Username = NewUsername,
            TelegramUsername = NewTelegramId,
            MonthlyLimit = NewMonthlyLimitGb * 1024 * 1024 * 1024,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        await _userRepository.AddAsync(newUser);

        Users.Add(newUser);

        NewUsername = string.Empty;
        NewTelegramId = string.Empty;
        await _vpnHandler.RefreshAsync();
    }

    [RelayCommand]
    private async Task CopyLink(User user)
    {
        var uiMessageBox = new Wpf.Ui.Controls.MessageBox()
        {
            Title = "Error",
            Content = $"Please start VPN",
            CloseButtonText = "OK"
        };

        if (string.IsNullOrEmpty(_quickTunnelService.Host))
        {
            await uiMessageBox.ShowDialogAsync();
            return;
        }
        string link = user.GenerateVlessLink(_quickTunnelService.Host);

        Clipboard.SetText(link);

        uiMessageBox = new Wpf.Ui.Controls.MessageBox()
        {
            Title = "Success",
            Content = $"Link for {user.Username} copied to clipboard!",
            CloseButtonText = "OK"
        };

        await uiMessageBox.ShowDialogAsync();
    }

    [RelayCommand]
    private async Task DeleteUser(User user)
    {
        var uiMessageBox = new Wpf.Ui.Controls.MessageBox
        {
            Title = "Warning",
            Content = $"Are you sure you want to delete {user.Username} permanently?",
            PrimaryButtonText = "Delete",
            PrimaryButtonAppearance = Wpf.Ui.Controls.ControlAppearance.Danger,
            CloseButtonText = "Cancel"
        };

        var result = await uiMessageBox.ShowDialogAsync();
        if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
        {
            await _userRepository.DeleteAsync(user);
            Users.Remove(user);
            await _vpnHandler.RefreshAsync();
        }
    }

    public async Task OnNavigatedToAsync()
    {
        var dbUsers = await _userRepository.GetAllAsync();

        Users.Clear();
        foreach (var user in dbUsers)
        {
            Users.Add(user);
        }
    }

    public async Task OnNavigatedFromAsync()
    {
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task SaveUser()
    {
        if (string.IsNullOrWhiteSpace(NewUsername))
        {
            var uiMessageBox = new Wpf.Ui.Controls.MessageBox()
            {
                Title = "Error",
                Content = $"Username should not empty",
                CloseButtonText = "OK"
            };

            await uiMessageBox.ShowDialogAsync();
            return;
        }

        if (IsEditing)
        {
            var userToEdit = Users.FirstOrDefault(u => u.Id == _editingUserId);
            if (userToEdit != null)
            {
                userToEdit.Username = NewUsername;
                userToEdit.TelegramUsername = NewTelegramId;
                userToEdit.MonthlyLimit = NewMonthlyLimitGb * 1024 * 1024 * 1024;

                await _userRepository.UpdateAsync(userToEdit);
                await _vpnHandler.RefreshAsync();

                int index = Users.IndexOf(userToEdit);
                Users[index] = userToEdit;
            }
        }
        else
        {
            var newUser = new User
            {
                Username = NewUsername,
                TelegramUsername = NewTelegramId,
                MonthlyLimit = NewMonthlyLimitGb * 1024 * 1024 * 1024,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            await _userRepository.AddAsync(newUser);
            Users.Add(newUser);

            NewUsername = string.Empty;
            NewTelegramId = string.Empty;
            await _vpnHandler.RefreshAsync();
        }

        ClearForm();
    }

    [RelayCommand]
    private void PrepareEdit(User user)
    {
        NewUsername = user.Username;
        NewTelegramId = user.TelegramUsername;
        NewMonthlyLimitGb = user.MonthlyLimit / (1024 * 1024 * 1024);

        _editingUserId = user.Id;

        NotifyEditStateChanged();
    }

    [RelayCommand]
    private void CancelEdit()
    {
        ClearForm();
    }

    private void ClearForm()
    {
        NewUsername = string.Empty;
        NewTelegramId = string.Empty;
        NewMonthlyLimitGb = 10;
        _editingUserId = null;

        NotifyEditStateChanged();
    }

    private void NotifyEditStateChanged()
    {
        OnPropertyChanged(nameof(IsEditing));
        OnPropertyChanged(nameof(SubmitButtonText));
        OnPropertyChanged(nameof(SubmitButtonIcon));
    }
}

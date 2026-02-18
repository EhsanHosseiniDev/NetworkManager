using NetworkManager.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace NetworkManager.Views.Pages;

public partial class UserManagerPage : INavigableView<UserManagerViewModel>
{
    public UserManagerPage(UserManagerViewModel userManagerViewModel)
    {
        ViewModel = userManagerViewModel;
        DataContext = ViewModel;
        InitializeComponent();
    }

    public UserManagerViewModel ViewModel { get; }

    public async void OnNavigatedTo()
    {
        await ViewModel.OnNavigatedToAsync();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Users.Count == 0)
        {
            await ViewModel.OnNavigatedToAsync();
        }
    }
}

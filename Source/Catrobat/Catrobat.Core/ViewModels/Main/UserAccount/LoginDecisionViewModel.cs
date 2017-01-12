using System.Threading.Tasks;
using System.Windows.Input;
using Catrobat.Core.Models.OnlinePrograms;
using Catrobat.IDE.Core.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Catrobat.Core.Resources.Localization;
using Catrobat.IDE.Core.ViewModels.Main.OnlinePrograms;

namespace Catrobat.IDE.Core.ViewModels.Main.UserAccount
{
  public class LoginDecisionViewModel : ViewModelBase
  {
    #region commands

    public ICommand LoginCommand => new RelayCommand(Login);

    public ICommand RegisterCommand => new RelayCommand(Register);

    #endregion

    #region construction

    public LoginDecisionViewModel()
    {

    }

    #endregion

    #region helper

    private void Login()
    {
      ServiceLocator.NavigationService.NavigateTo<LoginViewModel>();
    }

    private void Register()
    {
      ServiceLocator.NavigationService.NavigateTo<RegisterViewModel>();
    }

    #endregion
  }
}

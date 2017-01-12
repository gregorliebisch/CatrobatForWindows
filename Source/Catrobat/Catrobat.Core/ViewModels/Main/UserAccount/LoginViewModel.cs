using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Popups;
using Catrobat.Core.Models.OnlinePrograms;
using Catrobat.IDE.Core.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Catrobat.Core.Resources.Localization;
using Catrobat.IDE.Core.Services.Web;
using Catrobat.IDE.Core.ViewModels.Main.OnlinePrograms;

namespace Catrobat.IDE.Core.ViewModels.Main.UserAccount
{
  public class LoginViewModel : ViewModelBase
  {
    #region attributes

    private string username_;
    private string password_;
    private bool showPassword_;

    #endregion

    #region properies

    public string Username
    {
      get { return username_; }
      set
      {
        if (value != username_)
        {
          username_ = value;
          RaisePropertyChanged(nameof(Username));
        }
      }
    }

    public string Password
    {
      get { return password_; }
      set
      {
        if (value != password_)
        {
          password_ = value;
          RaisePropertyChanged(nameof(Password));
        }
      }
    }

    public bool ShowPassword
    {
      get { return showPassword_; }
      set
      {
        if (value != showPassword_)
        {
          showPassword_ = value;
          RaisePropertyChanged(nameof(ShowPassword));
        }
      }
    }

    #endregion

    #region commands

    public ICommand LoginCommand => new RelayCommand(Login);

    public ICommand PasswordForgottenCommand => new RelayCommand(PasswordForgotten);

    #endregion

    #region construction

    public LoginViewModel()
    {

    }

    #endregion

    #region helper

    private void ResetAttributes()
    {
      Username = "";
      Password = "";
      ShowPassword = false;
    }

    private async void ShowDialog(string message)
    {
      var msgdlg = new MessageDialog(message);

      await msgdlg.ShowAsync();
    }

    private bool SimpleInputCheck()
    {
      if (Username == "")
      {
        ShowDialog("invalid username");
        return false;
      }

      if (Password == "")
      {
        ShowDialog("invalid password");
        return false;
      }

      return true;
    }

    private async void Login()
    {
      if (!SimpleInputCheck())
      {
        return;
      }

      if (!(await UserAccountService.Instance.Login(Username, Password)))
      {
        ShowDialog("error occured - please check your input");
      }

      ShowDialog("login successful");
    }

    private async void PasswordForgotten()
    {
      await Launcher.LaunchUriAsync(new Uri("https://share.catrob.at/pocketcode/resetting/request"));
    }

    #endregion
  }
}

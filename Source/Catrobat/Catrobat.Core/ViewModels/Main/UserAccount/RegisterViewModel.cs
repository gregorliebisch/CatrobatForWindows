using System;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Input;
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
  public class RegisterViewModel : ViewModelBase
  {
    #region attributes

    private string username_;
    private string email_;
    private string password1_;
    private string password2_;
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

    public string Email
    {
      get { return email_; }
      set
      {
        if (value != email_)
        {
          email_ = value;
          RaisePropertyChanged(nameof(Email));
        }
      }
    }

    public string Password1
    {
      get { return password1_; }
      set
      {
        if (value != password1_)
        {
          password1_ = value;
          RaisePropertyChanged(nameof(Password1));
        }
      }
    }

    public string Password2
    {
      get { return password2_; }
      set
      {
        if (value != password2_)
        {
          password2_ = value;
          RaisePropertyChanged(nameof(Password2));
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

    public ICommand RegisterCommand => new RelayCommand(Register);

    #endregion

    #region construction

    public RegisterViewModel()
    {
      ResetAttributes();
    }

    #endregion

    #region helper

    private void ResetAttributes()
    {
      Username = "";
      Email = "";
      Password1 = "";
      Password2 = "";
      ShowPassword = false;
    }

    private async void Register()
    {
      if (!SimpleInputCheck())
      {
        return;
      }

      if (!(await UserAccountService.Instance.Register(Username, Email, Password1)))
      {
        ShowDialog("error occured - please check your input");
      }

      ShowDialog("register successful");

      ResetAttributes();

      ServiceLocator.NavigationService.NavigateTo<MainViewModel>();
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

      if (Email == "" || !Email.Contains("@"))
      {
        ShowDialog("invalid e-mail");
        return false;
      }

      if (Password1 == "" || Password2 == "")
      {
        ShowDialog("invalid password");
        return false;
      }

      if (Password1 != Password2)
      {
        ShowDialog("different passwords");
        return false;
      }

      return true;
    }

    #endregion
  }
}

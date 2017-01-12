using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Catrobat.IDE.Core.Utilities.JSON;
using GalaSoft.MvvmLight;

namespace Catrobat.IDE.Core.Services.Web
{
  public sealed class UserAccountService : ObservableObject
  {

    #region attributes

    private static volatile UserAccountService instance_;
    private static readonly object SyncRoot = new object();
    private string userToken_;

    #endregion

    #region properties

    public static UserAccountService Instance
    {
      get
      {
        if (instance_ == null)
        {
          lock (SyncRoot)
          {
            if (instance_ == null)
            {
              instance_ = new UserAccountService();
            }
          }
        }

        return instance_;
      }
    }

    public string UserToken
    {
      get { return userToken_; }
      set
      {
        if (value != userToken_)
        {
          userToken_ = value;
          RaisePropertyChanged(nameof(UserToken));
          RaisePropertyChanged(nameof(IsLoggedIn));
        }
      }
    }

    public bool IsLoggedIn => userToken_ != "";

    #endregion

    #region public methods

    public async Task<bool> Register(string username, string email, string password)
    {
      var ci = new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);

      var languageCountry = ci.Name.Split('-');

      if (languageCountry.Length != 2)
      {
        //TODO: invalid culture info format error; eg "en-US" expected
        return false;
      }

      var result = await CommunicationService.Instance.RegisterLoginAsync(username, password, email, languageCountry[0], languageCountry[1]);

      if (result.statusCode != StatusCodes.ServerResponseOk)
      {
        //TODO: register failed error
        return false;
      }

      UserToken = result.token;

      return true;
    }

    public async Task<bool> Login(string username, string password)
    {
      var ci = new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);

      var languageCountry = ci.Name.Split('-');

      if (languageCountry.Length != 2)
      {
        //TODO: invalid culture info format error; eg "en-US" expected
        return false;
      }

      var result = await CommunicationService.Instance.RegisterLoginAsync(username, password);

      if (result.statusCode != StatusCodes.ServerResponseOk)
      {
        //TODO: login failed error
        return false;
      }

      UserToken = result.token;

      return true;
    }

    public async Task<bool> Logout()
    {
      UserToken = "";

      return true;
    }

    #endregion

    #region construction

    private UserAccountService()
    {
      //TODO: load token if available?
      UserToken = "";
    }

    #endregion

  }
}

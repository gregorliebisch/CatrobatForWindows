using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Catrobat.IDE.Core.ExtensionMethods;
using Catrobat.IDE.Core.Services.Storage;
using Catrobat.IDE.Core.Utilities.JSON;
using GalaSoft.MvvmLight;

namespace Catrobat.IDE.Core.Services.Web
{
  public sealed class UserAccountService : ObservableObject
  {

    #region attributes

    private static volatile UserAccountService instance_;
    private static readonly object SyncRoot = new object();

    private static readonly string LoginInfoFileName = "LoginInfo";
    private LoginInfo loginInfo_;

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

    public LoginInfo LoginInfo
    {
      get { return loginInfo_;}
      set
      {
        if (loginInfo_ != value)
        {
          loginInfo_ = value;

          RaisePropertyChanged(nameof(LoginInfo));
          RaisePropertyChanged(nameof(IsLoggedIn));
        }
      }
    }

    public bool IsLoggedIn => LoginInfo != null;

    #endregion

    #region public methods

    public async Task<bool> Register(
      string username, 
      string email, 
      string password)
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

      await UpdateLoginInfo(new LoginInfo
      {
        Username = username,
        Token = result.token
      });

      return true;
    }

    public async Task<bool> Login(string username, string password)
    {
      var result = await CommunicationService.Instance.RegisterLoginAsync(username, password);

      if (result.statusCode != StatusCodes.ServerResponseOk)
      {
        //TODO: login failed error
        return false;
      }

      await UpdateLoginInfo(new LoginInfo
      {
        Username = username,
        Token = result.token
      });

      return true;
    }

    public async Task<bool> CheckLoginInfo(LoginInfo info)
    {
      var result = await CommunicationService.Instance.CheckLoginInfoAsync(info);

      return result.statusCode == StatusCodes.ServerResponseOk;
    }

    public async Task<bool> Logout()
    {
      await UpdateLoginInfo(null);

      return true;
    }

    #endregion

    #region construction

    private UserAccountService()
    {
      var checkInfoTask = LoginInfoFileExists();

      checkInfoTask.Wait();

      if (checkInfoTask.Result)
      {
        var readInfoTask = ReadLoginInfoFile();

        readInfoTask.Wait();

        return;
      }

      LoginInfo = null;
    }

    #endregion

    #region private helpers

    private async Task UpdateLoginInfo(LoginInfo info)
    {
      var updateTask = info == null 
        ? DeleteLoginInfoFile() 
        : WriteLoginInfoFile();

      LoginInfo = info;

      await updateTask;
    }

    private async Task WriteLoginInfoFile()
    {
      var folder = ApplicationData.Current.LocalFolder;

      var file = await folder.CreateFileAsync(LoginInfoFileName, 
        CreationCollisionOption.ReplaceExisting);

      var stream = await file.OpenStreamForWriteAsync();

      var ser = new DataContractSerializer(typeof(LoginInfo));

      ser.WriteObject(stream, LoginInfo);

      await stream.FlushAsync();
    }

    private async Task ReadLoginInfoFile()
    {
      var folder = ApplicationData.Current.LocalFolder;

      var file = await folder.GetFileAsync(LoginInfoFileName);

      var stream = await file.OpenStreamForReadAsync();

      var ser = new DataContractSerializer(typeof(LoginInfo));

      var info = (LoginInfo)ser.ReadObject(stream);

      var isValid = await CheckLoginInfo(info);

      if (!isValid)
      {
        await DeleteLoginInfoFile();

        LoginInfo = null;

        return;
      }

      LoginInfo =  info;
    }

    private async Task DeleteLoginInfoFile()
    {
      var folder = ApplicationData.Current.LocalFolder;

      var file = await folder.GetFileAsync(LoginInfoFileName);

      await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
    }

    private async Task<bool> LoginInfoFileExists()
    {
      var folder = ApplicationData.Current.LocalFolder;

      try
      {
        var file = await folder.GetFileAsync(LoginInfoFileName);
      }
      catch
      {
        return false;
      }

      return true;
    }

    #endregion

  }

  [DataContract]
  public class LoginInfo
  {

    #region Properties

    [DataMember]
    public string Username { get; set; }

    [DataMember]
    public string Token { get; set; }

    #endregion

  }
}

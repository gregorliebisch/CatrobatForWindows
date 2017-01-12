using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Catrobat.IDE.Core.Utilities.JSON;

namespace Catrobat.IDE.Core.Services.Web
{
  public sealed class UserAccountService
  {

    #region attributes

    private static volatile UserAccountService instance_;
    private static readonly object SyncRoot = new object();

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

    #endregion

    #region public methods

    public async Task<bool> Register(string username, string email, string password)
    {
      var ci = new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);

      var languageCountry = ci.Name.Split('-');

      if (languageCountry.Length != 2)
      {
        //TODO: invalid culture info format; eg "en-US" expected
        return false;
      }

      var result = await CommunicationService.Instance.RegisterAsync(username, email, password, languageCountry[0], languageCountry[1]);

      return true;
    }

    #endregion

    #region construction

    private UserAccountService()
    {
    }

    #endregion

  }
}

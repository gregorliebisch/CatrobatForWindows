using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Catrobat.Core.Models.OnlinePrograms;
using Catrobat.Core.Resources;
using Catrobat.IDE.Core.CatrobatObjects;
using Catrobat.IDE.Core.Utilities.JSON;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace Catrobat.IDE.Core.Services.Web
{
  public sealed class CommunicationService : ObservableObject
  {
    #region attributes

    private static volatile CommunicationService instance_;
    private static readonly object SyncRoot = new object();

    private bool internetAccessAvailable_;

    //private Queue<>

    #endregion

    #region properties

    public static CommunicationService Instance
    {
      get
      {
        if (instance_ == null)
        {
          lock (SyncRoot)
          {
            if (instance_ == null)
            {
              instance_ = new CommunicationService();
            }
          }
        }

        return instance_;
      }
    }

    public bool InternetAccessAvailable
    {
      get { return internetAccessAvailable_; }
      set
      {
        if (value != internetAccessAvailable_)
        {
          internetAccessAvailable_ = value;
          RaisePropertyChanged(nameof(InternetAccessAvailable));
        }
      }
    }

    #endregion

    #region public methods

    #region ui 

    public async Task<List<ProgramInfo>> LoadCategoryAsync(
      string categoryFormat,
      int offset,
      int count,
      CancellationToken token)
    {
      var requestUri = string.Format(
        ApplicationResourcesHelper.Get(categoryFormat),
        count, offset);

      var programOverview = await LoadAndDeserializeProjects(requestUri, token);

      return programOverview.CatrobatProjects.Select(p => new ProgramInfo(p)).ToList();
    }

    public async Task<ProgramInfo> LoadProjectByIdAsync(
      int id,
      CancellationToken token)
    {
      var requestUri = string.Format(
        ApplicationResourcesHelper.Get("API_GET_PROJECT_BY_ID"), id);

      var programOverview = await LoadAndDeserializeProjects(requestUri, token);

      return new ProgramInfo(programOverview.CatrobatProjects.First());
    }

    public async Task<List<ProgramInfo>> LoadFeaturedAsync(
      int offset,
      int count,
      CancellationToken token)
    {

      var requestUri = string.Format(
        ApplicationResourcesHelper.Get("API_FEATURED_PROJECTS"),
        count, offset);

      var programOverview = await LoadAndDeserializeProjects(requestUri, token);

      var completeInfos = new List<ProgramInfo>();

      foreach (var incompleteHeader in programOverview.CatrobatProjects)
      {
        var programInfo = await LoadProjectByIdAsync(
          int.Parse(incompleteHeader.ProjectId), token);

        programInfo.FeaturedImage = new Uri(incompleteHeader.FeaturedImage);

        completeInfos.Add(programInfo);
      }

      return completeInfos;
    }

    public async Task<List<ProgramInfo>> SearchAsync(
      string query,
      int offset,
      int count,
      CancellationToken token)
    {
      var encodedQuery = WebUtility.UrlEncode(query);

      var requestUri = string.Format(
        ApplicationResourcesHelper.Get("API_SEARCH_PROJECTS"),
        encodedQuery, count, offset);

      var programOverview = await LoadAndDeserializeProjects(requestUri, token);

      return programOverview.CatrobatProjects.Select(p => new ProgramInfo(p)).ToList();
    }

    #endregion

    #region user account

    public async Task<JSONStatusResponse> RegisterLoginAsync(
      string username, 
      string password, 
      string email = "",
      string languageCode = "", 
      string countryCode = "")
    {
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>(
          ApplicationResourcesHelper.Get("API_PARAM_REG_USERNAME"), 
          username),
        new KeyValuePair<string, string>(
          ApplicationResourcesHelper.Get("API_PARAM_REG_EMAIL"), 
          email),
        new KeyValuePair<string, string>(
          ApplicationResourcesHelper.Get("API_PARAM_REG_PASSWORD"), 
          password),
        new KeyValuePair<string, string>(
          ApplicationResourcesHelper.Get("API_PARAM_REG_COUNTRY"), 
          countryCode),
        new KeyValuePair<string, string>(
          ApplicationResourcesHelper.Get("API_PARAM_LANGUAGE"), 
          languageCode)
      };

      return await LoadJsonResponse(
        ApplicationResourcesHelper.Get("API_LOGIN_REGISTER"),
        parameters, new CancellationToken());
    }

    public async Task<JSONStatusResponse> CheckLoginInfoAsync(LoginInfo info)
    {
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>(
          ApplicationResourcesHelper.Get("API_PARAM_USERNAME"), 
          info.Username),
        new KeyValuePair<string, string>(
          ApplicationResourcesHelper.Get("API_PARAM_TOKEN"), 
          info.Token)
      };

      return await LoadJsonResponse(
        ApplicationResourcesHelper.Get("API_CHECK_TOKEN"), 
        parameters, new CancellationToken());
    }

    #endregion

    #region project down- & upload

    public async Task<Stream> DownloadAsync(
      string downloadUrl,
      CancellationToken token)
    {
      var httpClient = new HttpClient
      {
        BaseAddress = new Uri(ApplicationResourcesHelper.Get("POCEKTCODE_BASE_ADDRESS"))
      };

      try
      {
        var httpResponse = await httpClient.GetAsync(
          downloadUrl, HttpCompletionOption.ResponseContentRead, token);

        httpResponse.EnsureSuccessStatusCode();

        var stream = await httpResponse.Content.ReadAsStreamAsync();

        InternetAccessAvailable = true;

        return stream;
      }
      catch
      {
        InternetAccessAvailable = false;
      }

      return null;
    }

    #endregion

    #endregion

    #region construction

    private CommunicationService()
    {
    }

    #endregion

    #region private helpers

    private async Task<OnlineProgramOverview> LoadAndDeserializeProjects(
      string requestUri, 
      CancellationToken token)
    {
      var httpClient = new HttpClient
      {
        BaseAddress = new Uri(
          ApplicationResourcesHelper.Get("API_BASE_ADDRESS"))
      };

      try
      {
        var httpResponse = await httpClient.GetAsync(requestUri, token);

        httpResponse.EnsureSuccessStatusCode();

        var jsonResult = await httpResponse.Content.ReadAsStringAsync();
        var programOverview = await Task.Run(
          () => JsonConvert.DeserializeObject<OnlineProgramOverview>(
            jsonResult), token);

        InternetAccessAvailable = true;

        return programOverview;
      }
      catch
      {
        InternetAccessAvailable = false;
      }

      return new OnlineProgramOverview
      {
        CatrobatInformation = new Catrobatinformation(),
        CatrobatProjects = new List<OnlineProgramHeader>()
      };
    }

    private async Task<JSONStatusResponse> LoadJsonResponse(
      string requestUri,
      IEnumerable<KeyValuePair<string, string>> parameters, 
      CancellationToken token)
    {
      var httpClient = new HttpClient
      {
        BaseAddress = new Uri(
          ApplicationResourcesHelper.Get("API_BASE_ADDRESS"))
      };

      JSONStatusResponse statusResponse;

      try
      {
        HttpContent postParameters = new FormUrlEncodedContent(parameters);

        var httpResponse = await httpClient.PostAsync(
          requestUri, postParameters, token);

        httpResponse.EnsureSuccessStatusCode();

        var jsonResult = await httpResponse.Content.ReadAsStringAsync();

        InternetAccessAvailable = true;

        statusResponse = JsonConvert.
          DeserializeObject<JSONStatusResponse>(jsonResult);
      }
      catch (HttpRequestException ex)
      {
        var isUnauthorizedException = ex.Message.Contains("401");

        if (isUnauthorizedException)
        {
          statusResponse = new JSONStatusResponse
          {
            statusCode = StatusCodes.ServerResponseUserDoesNotExist
          };
        }
        else
        {
          InternetAccessAvailable = false;

          statusResponse = new JSONStatusResponse
          {
            statusCode = StatusCodes.HTTPRequestFailed
          };
        }     
      }
      catch (JsonSerializationException)
      {
        statusResponse = new JSONStatusResponse
        {
          statusCode = StatusCodes.JSONSerializationFailed
        };
      }
      catch (Exception)
      {
        statusResponse = new JSONStatusResponse
        {
          statusCode = StatusCodes.UnknownError
        };
      }

      return statusResponse;
    }

    #endregion

  }
}

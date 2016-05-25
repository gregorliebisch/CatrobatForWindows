using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Windows.Input;
using Catrobat.Core.Models.OnlinePrograms;
using Catrobat.Core.Resources;
using Catrobat.IDE.Core.CatrobatObjects;
using GalaSoft.MvvmLight;

namespace Catrobat.Core.ViewModels.Main.OnlinePrograms
{
  public class ProgramsViewModel : ObservableObject
  {
    //always have as many CategoryOnlineNames as CategorySearchKeyWords
    private static readonly string[] CategoryOnlineNames = { "newest", "most downloaded", "most viewed" };
    private static readonly string[] CategorySearchKeyWords = { "API_RECENT_PROJECTS", "API_MOSTDOWNLOADED_PROJECTS", "API_MOSTVIEWED_PROJECTS" };

    private bool _inSearchMode;

    private string _searchText;

    public bool InSearchMode
    {
      get { return _inSearchMode; }
      set
      {
        if (_inSearchMode == value)
        {
          return;
        }

        _inSearchMode = value;
        RaisePropertyChanged(nameof(InSearchMode));
      }
    }

    public string SearchText
    {
      get { return _searchText; }
      set
      {
        if (_searchText == value)
        {
          return;
        }

        _searchText = value;
        RaisePropertyChanged(nameof(SearchText));
      }
    }

    public ProgramViewModel FeaturedProgram { get; private set; }

    public ObservableCollection<CategoryViewModel> Categories { get; set; }

    public ObservableCollection<ProgramViewModel> SearchResults { get; set; }

    public ICommand SearchCommand => new RelayCommand(Search, CanSearch);

    public ICommand ExitSearchCommand => new RelayCommand(ExitSearch, CanExitSearch);

    public ProgramsViewModel()
    {
      //check if internet connection is available

      InSearchMode = false;
      SearchText = "";
      Categories = new ObservableCollection<CategoryViewModel>();

      for (int i = 0; i < CategoryOnlineNames.Length; ++i)
      {
        Categories.Add(new CategoryViewModel(
          new Category { DisplayName = CategoryOnlineNames[i], OnlineName = CategoryOnlineNames[i], SearchKeyWork = CategorySearchKeyWords[i] }));
      }

      LoadOnlinePrograms();

      SearchResults = new ObservableCollection<ProgramViewModel>();
    }

    public async void LoadOnlinePrograms()
    {
      System.Threading.CancellationToken cToken = new System.Threading.CancellationToken();
      HttpClient httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri("https://share.catrob.at/pocketcode/api/");
      HttpResponseMessage httpResponse = null;
      string jsonResult;

      /*
      //ERROR IN FEATURED PROJECTS DUNNO WHY ATM
      //Get featured Program
      httpResponse = await httpClient.GetAsync(
        string.Format(ApplicationResourcesHelper.Get("API_FEATURED_PROJECTS"),
        1, 0);, cToken);
      httpResponse.EnsureSuccessStatusCode();
      //TRY CATCH TO CHECK IF INTERNET CONNECTION WAS AVAILABLE AND SET "NO INTERNET CONNECTION" BUT IF ERROR
      jsonResult = await httpResponse.Content.ReadAsStringAsync();
      var featuredPrograms = await Task.Run(() => Newtonsoft.Json.JsonConvert.DeserializeObject<OnlineProgramOverview>(jsonResult));

      FeaturedProgram = new ProgramViewModel(
        new Program
        {
          Title = featuredPrograms.CatrobatProjects[0].ProjectName,
          ImageSource = new Uri(featuredPrograms.CatrobatProjects[0].ScreenshotBig)
        });

      */

      //Set 2 Progams for each Category
      foreach (var category in Categories)
      {
        httpResponse = await httpClient.GetAsync(
          string.Format(ApplicationResourcesHelper.Get(category.SearchKeyWork),
          2, 0), cToken);
        httpResponse.EnsureSuccessStatusCode();
        jsonResult = await httpResponse.Content.ReadAsStringAsync();
        var retrievedPrograms = await Task.Run(() => Newtonsoft.Json.JsonConvert.DeserializeObject<OnlineProgramOverview>(jsonResult));

        for (var i = 0; i < 2; ++i)
        {
          category.Programs.Add(
            new ProgramViewModel(
              new Program
              {
                Title = retrievedPrograms.CatrobatProjects[i].ProjectName,
                ImageSource = new Uri(retrievedPrograms.CatrobatProjects[i].ScreenshotBig)
              }));
        }
      }

    }

    private bool CanSearch()
    {
      return true;
    }

    private bool CanExitSearch()
    {
      return true;
    }

    private async void Search()
    {
      
      System.Threading.CancellationToken cToken = new System.Threading.CancellationToken();

      HttpClient httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri("https://share.catrob.at/pocketcode/api/");

      var encodedSearchText = WebUtility.UrlEncode(SearchText);
      HttpResponseMessage httpResponse = await httpClient.GetAsync(
                              string.Format(ApplicationResourcesHelper.Get("API_SEARCH_PROJECTS"), encodedSearchText,
                              10, 0), cToken);

      httpResponse.EnsureSuccessStatusCode();
      var jsonResult = await httpResponse.Content.ReadAsStringAsync();
      var retrievedPrograms = await Task.Run(() => Newtonsoft.Json.JsonConvert.DeserializeObject<OnlineProgramOverview>(jsonResult));

      for (var i = 0; i < retrievedPrograms.CatrobatProjects.Count; ++i)
      {
        SearchResults.Add(
          new ProgramViewModel(
            new Program
            {
              Title = retrievedPrograms.CatrobatProjects[i].ProjectName,
              ImageSource = new Uri(retrievedPrograms.CatrobatProjects[i].ScreenshotBig)
            }));
      }

      InSearchMode = true;
    }

    private void ExitSearch()
    {
      SearchResults.Clear();

      SearchText = "";

      InSearchMode = false;
    }
  }
}

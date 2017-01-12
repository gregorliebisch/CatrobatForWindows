using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Catrobat.Core.Models.OnlinePrograms;
using Catrobat.Core.Resources;
using Catrobat.IDE.Core.CatrobatObjects;
using GalaSoft.MvvmLight;
using System.Threading;
using Catrobat.IDE.Core.ExtensionMethods;
using Catrobat.IDE.Core.Services;
using Catrobat.IDE.Core.Services.Web;

namespace Catrobat.IDE.Core.ViewModels.Main.OnlinePrograms
{
  public class ProgramsViewModel : ObservableObject
  {
    #region attributes

    //always have as many CategoryOnlineNames as CategorySearchKeyWords
    private static readonly string[] CategoryOnlineNames = { "most recent", "most downloaded", "most viewed" };
    private static readonly string[] CategorySearchKeyWords = { "API_RECENT_PROJECTS", "API_MOSTDOWNLOADED_PROJECTS", "API_MOSTVIEWED_PROJECTS" };

    private const int InitialProgramOffset = 0;
    private const int InitialNumberOfFeaturedPrograms = 10;
    private const int InitialNumberOfSearchedPrograms = 10;

    private bool _inSearchMode;
    private string _searchText;
    private bool _internetAvailable;
    private bool _noSearchResults;

    #endregion

    #region properties

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

    public bool NoSearchResults
    {
      get { return _noSearchResults; }
      set
      {
        if (_noSearchResults == value)
        {
          return;
        }

        _noSearchResults = value;
        RaisePropertyChanged(nameof(NoSearchResults));
      }
    }

    public bool InternetAvailable
    {
      get { return _internetAvailable; }
      set
      {
        if (_internetAvailable == value)
        {
          return;
        }

        _internetAvailable = value;
        RaisePropertyChanged(nameof(InternetAvailable));
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

    public ObservableCollection<SimpleProgramViewModel> FeaturedPrograms { get; private set; }

    public ObservableCollection<CategoryViewModel> Categories { get; set; }

    public ObservableCollection<SimpleProgramViewModel> SearchResults { get; set; }

    #endregion

    #region commands

    public ICommand SearchCommand => new RelayCommand(Search);

    public ICommand ExitSearchCommand => new RelayCommand(ExitSearch);

    public ICommand ReloadCommand => new RelayCommand(ReloadOnlinePrograms);

    #endregion

    #region construction

    public ProgramsViewModel()
    {
      InSearchMode = false;
      SearchText = "";
      InternetAvailable = true;

      FeaturedPrograms = new ObservableCollection<SimpleProgramViewModel>();
      Categories = new ObservableCollection<CategoryViewModel>();      
      SearchResults = new ObservableCollection<SimpleProgramViewModel>();

      PropertyChanged += ProgramsViewModelPropertyChanged;
      CommunicationService.Instance.PropertyChanged += CommuncationServicePropertyChanged;

      LoadFeaturedPrograms();
      InitializeCategories();
    }

    private void ProgramsViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(SearchText))
      {
        SearchResults.Clear();
        InSearchMode = false;
      }
    }

    private void CommuncationServicePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(CommunicationService.InternetAccessAvailable))
      {
        InternetAvailable = CommunicationService.Instance.InternetAccessAvailable;
      }
    }

    #endregion

    #region private helpers

    private async void LoadFeaturedPrograms()
    {
      var cancellationToken = new CancellationToken();

      var programInfos = await CommunicationService.Instance.
        LoadFeaturedAsync(InitialProgramOffset, 
          InitialNumberOfFeaturedPrograms, cancellationToken);

      FeaturedPrograms.AddRange(
        programInfos.Select(pi => new SimpleProgramViewModel(pi)));
    }

    private void InitializeCategories()
    {
      for (var i = 0; i < CategoryOnlineNames.Length; ++i)
      {
        Categories.Add(new CategoryViewModel(
          new Category
          {
            DisplayName = CategoryOnlineNames[i].ToUpper(),
            OnlineName = CategoryOnlineNames[i],
            SearchKeyWord = CategorySearchKeyWords[i]
          }, this));
      }
    }

    private void ResetPrograms()
    {
      FeaturedPrograms.Clear();
      LoadFeaturedPrograms();

      foreach (var categoryViewModel in Categories)
      {
        categoryViewModel.ResetPrograms();
      }
    }

    private async void Search()
    {
      var retreivedPrograms = await CommunicationService.Instance.
        SearchAsync(SearchText, InitialProgramOffset, 
          InitialNumberOfSearchedPrograms, new CancellationToken());

      if (retreivedPrograms.Count == 0)
      {
        NoSearchResults = true;
      }
      else
      {
        NoSearchResults = false;

        SearchResults.AddRange(
          retreivedPrograms.Select(p => new SimpleProgramViewModel(p)));
      }
      
      InSearchMode = true;
    }

    private void ExitSearch()
    {
      SearchResults.Clear();
      SearchText = "";
      InSearchMode = false;
    }

    private void ReloadOnlinePrograms()
    {
      if (InSearchMode)
      {
        Search();
      }
      else
      {
        ResetPrograms();
      }
    }

    #endregion
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catrobat.Core.Models.OnlinePrograms;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Net.Http;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Catrobat.Core.Resources;
using Catrobat.IDE.Core.CatrobatObjects;
using GalaSoft.MvvmLight;

namespace Catrobat.Core.ViewModels.Main.OnlinePrograms
{
  public class CategoryViewModel : ObservableObject
  {
    private Category Category { get; set; }

    public string DisplayName
    {
      get { return Category.DisplayName; }
      set
      {
        if (Category.DisplayName == value)
        {
          return;
        }

        Category.DisplayName = value;
        RaisePropertyChanged(nameof(DisplayName));
      }
    }

    public string OnlineName
    {
      get { return Category.OnlineName; }
      set
      {
        if (Category.OnlineName == value)
        {
          return;
        }

        Category.OnlineName = value;
        RaisePropertyChanged(nameof(OnlineName));
      }
    }

    public string SearchKeyWork
    {
      get { return Category.SearchKeyWork; }
      set
      {
        if (Category.SearchKeyWork == value)
        {
          return;
        }

        Category.OnlineName = value;
        RaisePropertyChanged(nameof(SearchKeyWork));
      }
    }

    public ObservableCollection<ProgramViewModel> Programs { get; set; }

    public ICommand ShowMoreCommand => new RelayCommand(ShowMore, CanShowMore);

    public CategoryViewModel(Category category)
    {
      Category = category;

      Programs = new ObservableCollection<ProgramViewModel>();
    }

    private bool CanShowMore()
    {
      //just for testing
      {
        return true;
      }
    }

    private async void ShowMore()
    {
      System.Threading.CancellationToken cToken = new System.Threading.CancellationToken();
      HttpClient httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri("https://share.catrob.at/pocketcode/api/");

      HttpResponseMessage httpResponse = await httpClient.GetAsync(
        string.Format(ApplicationResourcesHelper.Get(SearchKeyWork),
          2, Programs.Count), cToken);
      httpResponse.EnsureSuccessStatusCode();
      var jsonResult = await httpResponse.Content.ReadAsStringAsync();
      var retrievedPrograms = await Task.Run(() => Newtonsoft.Json.JsonConvert.DeserializeObject<OnlineProgramOverview>(jsonResult));

      for (var i = 0; i < 2; ++i)
      {
        Programs.Add(
          new ProgramViewModel(
            new Program
            {
              Title = retrievedPrograms.CatrobatProjects[i].ProjectName,
              ImageSource = new Uri(retrievedPrograms.CatrobatProjects[i].ScreenshotBig)
            }));
      }

    }
  }
}

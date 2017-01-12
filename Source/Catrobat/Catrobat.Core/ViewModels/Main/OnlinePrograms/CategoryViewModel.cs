using System;
using System.Collections.Generic;
using System.Linq;
using Catrobat.Core.Models.OnlinePrograms;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Threading;
using Catrobat.IDE.Core.CatrobatObjects;
using Catrobat.IDE.Core.ExtensionMethods;
using Catrobat.IDE.Core.Services.Web;
using GalaSoft.MvvmLight;

namespace Catrobat.IDE.Core.ViewModels.Main.OnlinePrograms
{
  public class CategoryViewModel : ObservableObject
  {
    #region attributes 

    private const int _programsPerLine = 2;

    private readonly ProgramsViewModel _programsViewModel;

    private int _programOffset;

    #endregion

    #region properties

    public Category Category { get; }

    public ObservableCollection<SimpleProgramViewModel> Programs { get; set; }

    public int ProgramsPerLine => _programsPerLine;

    #endregion

    #region interfaces

    public void ResetPrograms()
    {
      Programs.Clear();
      ShowMore();
    }

    #endregion

    #region commands

    public ICommand ShowMoreCommand => new RelayCommand(ShowMore);

    #endregion

    #region construction

    public CategoryViewModel(Category category, ProgramsViewModel programsViewModel)
    {
      Category = category;
      _programsViewModel = programsViewModel;

      Programs = new ObservableCollection<SimpleProgramViewModel>();
      ShowMore();
    }

    #endregion

    #region private helpers

    private async void ShowMore()
    {     
      while(true)
      {

        var programInfos = await CommunicationService.Instance.
          LoadCategoryAsync(Category.SearchKeyWord, 
            Programs.Count + _programOffset,
            ProgramsPerLine, new CancellationToken());

        if (!CheckForDuplicates(programInfos))
        {
          Programs.AddRange(programInfos.Select(
          pi => new SimpleProgramViewModel(pi)));

          break;
        }

        _programOffset++;
      }
    }

    private bool CheckForDuplicates(List<ProgramInfo> retrievedPrograms)
    {
      if (retrievedPrograms.Count < 1)
      {
        return false;
      }

      switch (Category.SearchKeyWord)
      {
        case "API_RECENT_PROJECTS":
          return Programs.Any(p => p.Program.Uploaded <= retrievedPrograms.First().Uploaded);

        case "API_MOSTDOWNLOADED_PROJECTS":
          return Programs.Any(p => p.Program.Downloads <= retrievedPrograms.First().Downloads);

        case "API_MOSTVIEWED_PROJECTS":
          return Programs.Any(p => p.Program.Views <= retrievedPrograms.First().Views);

        default:
          throw new Exception("Unknown Category.SearchKeyWord: " + Category.SearchKeyWord);
      }
    }

    #endregion
  }
}

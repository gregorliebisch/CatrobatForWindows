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
using Catrobat.IDE.Core.Services.Storage;
using Catrobat.IDE.Core.Utilities.JSON;
using Catrobat.IDE.Core.ViewModels;
using Catrobat.IDE.Core.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;

namespace Catrobat.IDE.Core.Services.Web
{
  public sealed class ImportExportService : ObservableObject
  {
    #region info collection

    private class ImportInfo
    {
      public bool IsActive { get; set; }
      public ProgramInfo ProgramInfo { get; set; }
      public CancellationTokenSource TokenSource { get; set; }
      public string UniqueName { get; set; }
    }

    private class ExportInfo
    {
      public bool IsActive { get; set; }
      public ProgramInfo ProgramInfo { get; set; }
      public CancellationTokenSource TokenSource { get; set; }
      public string UniqueName { get; set; }
    }

    #endregion

    #region attributes

    private static volatile ImportExportService instance_;
    private static readonly object SyncRoot = new object();

    private bool internetAccessAvailable_;

    private readonly Queue<ImportInfo> importQueue_;
    private static readonly object ImportQueueLock = new object();

    private readonly Queue<ExportInfo> exportQueue_;
    private static readonly object ExportQueueLock = new object();

    #endregion

    #region properties

    public static ImportExportService Instance
    {
      get
      {
        if (instance_ == null)
        {
          lock (SyncRoot)
          {
            if (instance_ == null)
            {
              instance_ = new ImportExportService();
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

    public bool ImportProgram(ProgramInfo info)
    {
      var importInfo = new ImportInfo
      {
        IsActive = false,
        ProgramInfo = info,
        TokenSource = new CancellationTokenSource()
      };

      lock (ImportQueueLock)
      {
        if (importQueue_.Any(i => i.ProgramInfo.Id == info.Id))
        {
          return false;
        }

        importQueue_.Enqueue(importInfo);
      }

      StartImportIfPossible();

      return true;
    }

    public bool CancelImport(ProgramInfo info)
    {
      lock (ImportQueueLock)
      {
        var importInfo = importQueue_.FirstOrDefault(
          i => i.ProgramInfo.Id == info.Id);

        if (importInfo == null)
        {
          return false;
        }

        importInfo.TokenSource.Cancel();

        return true;
      }
    }

    public bool ExportProgram(ProgramInfo info)
    {
      var exportInfo = new ExportInfo
      {
        IsActive = false,
        ProgramInfo = info,
        TokenSource = new CancellationToken()
      };

      lock (ExportQueueLock)
      {
        if (exportQueue_.Any(i => i.ProgramInfo.Id == info.Id))
        {
          return false;
        }

        exportQueue_.Enqueue(exportInfo);
      }

      StartExportIfPossible();

      return true;
    }

    #endregion

    #region construction

    private ImportExportService()
    {
      importQueue_ = new Queue<ImportInfo>();
      exportQueue_ = new Queue<ExportInfo>();
    }

    #endregion

    #region private helpers

    #region import

    private void StartImportIfPossible()
    {
      lock (ImportQueueLock)
      {
        if (importQueue_.Any() && !importQueue_.Any(i => i.IsActive))
        {
          // should run async -> no waiting 
          ImportTask();
        }
      }
    }

    private async Task ImportTask()
    {
      // take last element
      ImportInfo currentImport;

      lock (ImportQueueLock)
      {
        currentImport = importQueue_.Last();
        currentImport.IsActive = true;
      }

      // check if canceled
      if (currentImport.TokenSource.IsCancellationRequested)
      {
        lock (ImportQueueLock)
        {
          importQueue_.Dequeue();
        }

        StartImportIfPossible();

        return;
      }

      // download data
      var stream = await CommunicationService.Instance.DownloadAsync(
        currentImport.ProgramInfo.DownloadUrl, currentImport.TokenSource.Token);

      //check if canceled
      if (stream == null 
        || currentImport.TokenSource.IsCancellationRequested)
      {
        if (stream == null)
        {
          //TODO: no internet error
        }
        else
        {
          //TODO: canceled message

          stream.Dispose();
        }

        lock (ImportQueueLock)
        {
          importQueue_.Dequeue();
        }

        StartImportIfPossible();

        return;
      }

      //unzip data
      try
      {
        await ServiceLocator.ZipService.UnzipCatrobatPackageIntoIsolatedStorage(
          stream, StorageConstants.TempProgramImportPath);
      }
      catch
      {
        //TODO: unzip failed error
        stream.Dispose();

        await StorageSystem.GetStorage().DeleteDirectoryAsync(
          StorageConstants.TempProgramImportPath);

        lock (ImportQueueLock)
        {
          importQueue_.Dequeue();
        }

        StartImportIfPossible();

        return;
      }

      stream.Dispose();

      //check if canceled
      if (currentImport.TokenSource.IsCancellationRequested)
      {
        //TODO: canceled message

        await StorageSystem.GetStorage().DeleteDirectoryAsync(
          StorageConstants.TempProgramImportPath);

        lock (ImportQueueLock)
        {
          importQueue_.Dequeue();
        }

        StartImportIfPossible();

        return;
      }

      //check program
      var validateResult = await ServiceLocator.ProgramValidationService.
        CheckProgram(StorageConstants.TempProgramImportPath);

      
      //check if canceled
      if (validateResult.State != ProgramState.Valid 
        || currentImport.TokenSource.IsCancellationRequested)
      {
        if (validateResult.State != ProgramState.Valid)
        {
          // TODO: Invalid program state error
        }
        else
        {
          //TODO: canceled message
        }

        await StorageSystem.GetStorage().DeleteDirectoryAsync(
          StorageConstants.TempProgramImportPath);

        lock (ImportQueueLock)
        {
          importQueue_.Dequeue();
        }

        StartImportIfPossible();

        return;
      }

      //accept program
      await AcceptTempProgram(currentImport);

      //notify that a new program is available
      Messenger.Default.Send(new MessageBase(),
        ViewModelMessagingToken.LocalProgramsChangedListener);
    }

    private async Task AcceptTempProgram(ImportInfo currentImport)
    {
      currentImport.UniqueName = await ServiceLocator.ContextService.
        FindUniqueProgramName(currentImport.ProgramInfo.Name);

      if (currentImport.ProgramInfo.Name != currentImport.UniqueName)
      {
        var renameResult = await ServiceLocator.ContextService.RenameProgram(
          Path.Combine(StorageConstants.TempProgramImportPath,
            StorageConstants.ProgramCodePath), currentImport.UniqueName);

        if (renameResult.Status != XmlRenameStatus.Success)
        {
          throw new Exception("program rename failed");
        }
      }

      using (var storage = StorageSystem.GetStorage())
      {
        var newPath = Path.Combine(StorageConstants.ProgramsPath,
          currentImport.UniqueName);
        await storage.MoveDirectoryAsync(
          StorageConstants.TempProgramImportPath, newPath);
      }
    }

    #endregion

    #region export

    private void StartExportIfPossible()
    {
      lock (ExportQueueLock)
      {
        if (exportQueue_.Any() && !exportQueue_.Any(i => i.IsActive))
        {
          // should run async -> no waiting 
          ExportTask();
        }
      }
    }

    private async Task ExportTask()
    {
      
    }

    #endregion

    #endregion
  }
}

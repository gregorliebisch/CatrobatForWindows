﻿using System.Diagnostics;
using System.Globalization;
using Catrobat.IDE.Core.Resources.Localization;
using Catrobat.IDE.Core.Services;
using Catrobat.IDE.Core.UI;
using Catrobat.IDE.Core.ViewModels.Service;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Catrobat.IDE.Core.ViewModels.Settings
{
  public class SettingsViewModel : ViewModelBase
  {
    #region Private Members

    private ThemeChooser _themeChooser;

    #endregion

    #region Properties

    public ThemeChooser ThemeChooser
    {
      get { return _themeChooser; }
      set { _themeChooser = value; RaisePropertyChanged(() => ThemeChooser); }
    }

    public bool ShowMemoryMonitorOption
    {
      get { return Debugger.IsAttached; }
    }

    public CultureInfo CurrentCulture
    {
      get { return ServiceLocator.CultureService.GetCulture(); }

      set
      {
        if (ServiceLocator.CultureService.GetCulture().Equals(value))
        {
          return;
        }

        ServiceLocator.CultureService.SetCulture(value);
        RaisePropertyChanged(() => CurrentCulture);
      }
    }

    #endregion

    #region Commands

    public RelayCommand ShowDesignSettingsCommand { get; private set; }

    public RelayCommand ShowBrickSettingsCommand { get; private set; }

    public RelayCommand ShowLanguageSettingsCommand { get; private set; }

    public RelayCommand ShowAccountSettingsCommand { get; private set; }

    #endregion

    #region Actions

    private static void ShowDesignSettingsAction()
    {
      ServiceLocator.NavigationService.NavigateTo<SettingsThemeViewModel>();
    }

    private static void ShowBrickSettingsAction()
    {
      ServiceLocator.NavigationService.NavigateTo<SettingsBrickViewModel>();
    }

    private static void ShowLanguageSettingsAction()
    {
      ServiceLocator.NavigationService.NavigateTo<SettingsLanguageViewModel>();
    }

    private static void ShowAccountSettingsAction()
    {
      ServiceLocator.NavigationService.NavigateTo<UploadProgramLoginViewModel>();
    }

    protected override void GoBackAction()
    {
      base.GoBackAction();
    }

    #endregion

    #region MessageActions

    private void ThemeChooserInitializedMessageAction(GenericMessage<ThemeChooser> message)
    {
      ThemeChooser = message.Content;
    }

    #endregion

    public SettingsViewModel()
    {
      ShowDesignSettingsCommand = new RelayCommand(ShowDesignSettingsAction);
      ShowBrickSettingsCommand = new RelayCommand(ShowBrickSettingsAction);
      ShowLanguageSettingsCommand = new RelayCommand(ShowLanguageSettingsAction);
      ShowAccountSettingsCommand = new RelayCommand(ShowAccountSettingsAction);

      Messenger.Default.Register<GenericMessage<ThemeChooser>>(this,
           ViewModelMessagingToken.ThemeChooserListener, ThemeChooserInitializedMessageAction);
    }
  }
}
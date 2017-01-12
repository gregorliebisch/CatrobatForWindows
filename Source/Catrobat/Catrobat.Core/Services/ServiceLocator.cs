﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Catrobat.IDE.Core.Resources.Localization;
using Catrobat.IDE.Core.Services.Storage;
using Catrobat.IDE.Core.UI;
using Catrobat.IDE.Core.ViewModels;
using Catrobat.IDE.Core.XmlModelConvertion;

namespace Catrobat.IDE.Core.Services
{
    public enum TypeCreationMode { Lazy, Normal }

    public class ServiceLocator
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static INavigationService NavigationService
        { get; set; }

        #region Service instances

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static ISystemInformationService SystemInformationService
        { get { return GetInstance<ISystemInformationService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static ICultureService CultureService
        { get { return GetInstance<ICultureService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IImageResizeService ImageResizeService
        { get { return GetInstance<IImageResizeService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IPlayerLauncherService PlayerLauncherService
        { get { return GetInstance<IPlayerLauncherService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IResourceLoaderFactory ResourceLoaderFactory
        { get { return GetInstance<IResourceLoaderFactory>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IStorageFactory StorageFactory
        { get { return GetInstance<IStorageFactory>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IImageSourceConversionService ImageSourceConversionService
        { get { return GetInstance<IImageSourceConversionService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IProgramImportService ProgramImportService
        { get { return GetInstance<IProgramImportService>(); } }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static ISoundRecorderService SoundRecorderService
        { get { return GetInstance<ISoundRecorderService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IPictureService PictureService
        { get { return GetInstance<IPictureService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static INotificationService NotifictionService
        { get { return GetInstance<INotificationService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IColorConversionService ColorConversionService
        { get { return GetInstance<IColorConversionService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IShareService ShareService
        { get { return GetInstance<IShareService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IDispatcherService DispatcherService
        { get { return GetInstance<IDispatcherService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IPortableUIElementConversionService PortableUIElementConversionService
        { get { return GetInstance<IPortableUIElementConversionService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IActionTemplateService ActionTemplateService
        { get { return GetInstance<IActionTemplateService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static ISoundService SoundService
        { get { return GetInstance<ISoundService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static ISensorService SensorService
        { get { return GetInstance<ISensorService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IWebCommunicationService WebCommunicationService
        { get { return GetInstance<IWebCommunicationService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IZipService ZipService
        { get { return GetInstance<IZipService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IContextService ContextService
        { get { return GetInstance<IContextService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IProgramExportService ProgramExportService
        { get { return GetInstance<IProgramExportService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static IProgramValidationService ProgramValidationService
        { get { return GetInstance<IProgramValidationService>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public static ITraceService TraceService
        { get { return GetInstance<ITraceService>(); } }

    #endregion

        public static ViewModelLocator ViewModelLocator { get; set; }

        public static ThemeChooser ThemeChooser { get; set; }

        public static LocalizedStrings LocalizedStrings { get; set; }

        private static readonly Dictionary<Type, object> Instances = new Dictionary<Type, object>();

        public static void Register<T>(TypeCreationMode mode)
        {
            RegisterByType(typeof (T), mode);
        }

        public static void RegisterByType(Type type, TypeCreationMode mode)
        {
            lock (Instances)
            {
                if (mode == TypeCreationMode.Lazy)
                {
                    if (!Instances.ContainsKey(type))
                        Instances.Add(type, null);
                }
                else if (mode == TypeCreationMode.Normal)
                {
                    if (Instances.ContainsKey(type))
                        Instances.Remove(type);

                    Instances.Add(type, Activator.CreateInstance(type));
                }
            }
        }

        public static void Register(object objectToRegister)
        {
            lock (Instances)
            {
                var type = objectToRegister.GetType();
                if (Instances.ContainsKey(type))
                    Instances.Remove(type);

                Instances.Add(type, objectToRegister);
            }
        }

        public static object GetInstance(Type type)
        {
            lock (Instances)
            {
                object instance = null;
                bool isInDictionary = false;
                Type registeredType = type;


                foreach (var pair in Instances)
                {
                    if (pair.Key.GetTypeInfo().BaseType == type ||
                        pair.Key == type || 
                        pair.Key.GetTypeInfo().ImplementedInterfaces.Contains(type))
                    {
                        instance = pair.Value;

                        isInDictionary = instance != null;

                        if (!isInDictionary)
                        {
                            registeredType = pair.Key;
                            instance = Activator.CreateInstance(registeredType);
                        }

                        break;
                    }
                }

                if (instance == null)
                    throw new Exception("Type " + type.GetTypeInfo().Name + " is not registered.");

                if (!isInDictionary)
                    Instances[registeredType] = instance;

                return instance;
            }
        }

        public static IEnumerable<T> CreateImplementations<T>()
        {
            var currentAssembly = typeof(T).GetTypeInfo().Assembly;
            var typesInAssemblies = currentAssembly.DefinedTypes;

            var inAssemblies = typesInAssemblies as TypeInfo[] ?? typesInAssemblies.ToArray();

            var instances = (from typeInfo in inAssemblies 
                          where typeInfo.ImplementedInterfaces.Contains(typeof (T)) select (T) Activator.CreateInstance(typeInfo.AsType())).ToList();

            return instances;
        }

        public static T GetInstance<T>()
        {
            return (T)GetInstance(typeof(T));
        }

        public static void UnRegisterAll()
        {
            Instances.Clear();
        }
    }
}

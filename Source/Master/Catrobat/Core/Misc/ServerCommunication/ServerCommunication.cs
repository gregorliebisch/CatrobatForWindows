﻿using System;
using System.Net;
using System.IO;
using Catrobat.Core.Misc.Helpers;
using Catrobat.Core.Misc.JSON;
using Catrobat.Core.Objects;
using System.Collections.Generic;
using Catrobat.Core.Resources;
using Catrobat.Core.ZIP;

namespace Catrobat.Core.Misc.ServerCommunication
{
  public class ServerCommunication
  {
    public delegate void RegisterOrCheckTokenEvent(bool registered, string errorCode);
    public delegate void CheckTokenEvent(bool registered);
    public delegate void LoadOnlineProjectsEvent(List<OnlineProjectHeader> projects, bool append);
    public delegate void DownloadAndSaveProjectEvent(string filename);
    public delegate void UploadProjectEvent(bool successful);

    private static int _uploadCounter = 0;
    private static int _downloadCounter = 0;

    private static IServerCommunication _iServerCommunication;

    public static void SetIServerCommunication(IServerCommunication iServerCommunication)
    {
      _iServerCommunication = iServerCommunication;
    }

    public static bool NoUploadsPending()
    {
      return _uploadCounter == 0;
    }

    public static bool NoDownloadsPending()
    {
      return _downloadCounter == 0;
    }

    public static void RegisterOrCheckToken(string username, 
                                            string password, 
                                            string userEmail, 
                                            string language, 
                                            string country,
                                            string token,
                                            RegisterOrCheckTokenEvent callback)
    {
      // Generate post objects
      var postParameters = new Dictionary<string, object>
        {
          {ApplicationResources.REG_USER_NAME, username},
          {ApplicationResources.REG_USER_PASSWORD, password},
          {ApplicationResources.REG_USER_EMAIL, userEmail},
          {ApplicationResources.TOKEN, token}
        };

      if (country != null)
      {
        postParameters.Add(ApplicationResources.REG_USER_COUNTRY, country);
      }

      if (language != null)
      {
        postParameters.Add(ApplicationResources.REG_USER_LANGUAGE, language);
      }

      WebRequest request = FormUpload.MultipartFormDataPost(ApplicationResources.CheckTokenOrRegisterUrl,
        ApplicationResources.UserAgent, postParameters, (string a) =>
        {
          if (callback != null)
          {
            var response = JSONClassDeserializer.Deserialise<JSONStatusResponse>(a);
            if (response.StatusCode == StatusCodes.SERVER_RESPONSE_TOKEN_OK)
            {
              callback(false, response.StatusCode.ToString());
            }
            else if (response.StatusCode == StatusCodes.SERVER_RESPONSE_REGISTER_OK)
            {
              callback(true, response.StatusCode.ToString());
            }
            else
            {
              callback(false, response.StatusCode.ToString());
            }
          }
        });
    }

    public static void CheckToken(string token, CheckTokenEvent callback)
    {
      // Generate post objects
      var postParameters = new Dictionary<string, object> {{ApplicationResources.TOKEN, token}};

      WebRequest request = FormUpload.MultipartFormDataPost(ApplicationResources.CheckTokenUrl,
        ApplicationResources.UserAgent, postParameters, (string a) =>
        {
          if (callback != null)
          {
            var response = JSONClassDeserializer.Deserialise<JSONStatusResponse>(a);
            callback(response.StatusCode == StatusCodes.SERVER_RESPONSE_TOKEN_OK);
          }
        });
    }

    public static DateTime ConvertUnixTimeStamp(double timestamp)
    {
      var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
      return origin.AddSeconds(timestamp);
    }

    public static void LoadOnlineProjects(bool append, string filterText, int offset, LoadOnlineProjectsEvent callback)
    {
      _iServerCommunication.LoadOnlineProjects(append, filterText, offset, callback);
    }

    public static void DownloadAndSaveProject(string downloadUrl, DownloadAndSaveProjectEvent callback)
    {
      _downloadCounter += _iServerCommunication.DownloadAndSaveProject(downloadUrl, callback);
    }

    public static void UploadProject(string projectName, string projectDescription, string userEmail,
      string language, string token, UploadProjectEvent callback)
    {
      // Generate post objects
      var postParameters = new Dictionary<string, object>
        {
          {ApplicationResources.PROJECT_NAME_TAG, projectName},
          {ApplicationResources.PROJECT_DESCRIPTION_TAG, projectDescription},
          {ApplicationResources.USER_EMAIL, userEmail},
          {ApplicationResources.TOKEN, token}
        };

      using (var stream = new MemoryStream())
      {
        CatrobatZip.ZipCatrobatPackage(stream, CatrobatContext.ProjectsPath + "/" + projectName);
        byte[] data = stream.ToArray();

        postParameters.Add(ApplicationResources.PROJECT_CHECKSUM_TAG, Utils.toHex(MD5Core.GetHash(data)));

        if (language != null)
        {
          postParameters.Add(ApplicationResources.USER_LANGUAGE, language);
        }

        postParameters.Add(ApplicationResources.FILE_UPLOAD_TAG,
          new FormUpload.FileParameter(data,
            projectName + ApplicationResources.EXTENSION, ApplicationResources.MIMETYPE));

        _uploadCounter++;

        WebRequest request = FormUpload.MultipartFormDataPost(ApplicationResources.UploadFileUrl,
          ApplicationResources.UserAgent, postParameters, (string a) =>
          {
            _uploadCounter--;

            if (callback != null)
            {
              var response = JSONClassDeserializer.Deserialise<JSONStatusResponse>(a);
              callback(response.StatusCode == StatusCodes.SERVER_RESPONSE_TOKEN_OK);
            }
          });
      }
    }
  }
}

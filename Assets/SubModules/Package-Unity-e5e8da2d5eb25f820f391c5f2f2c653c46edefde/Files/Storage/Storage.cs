using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Egsp.Files.Serializers;
using Sirenix.Serialization;

namespace Egsp.Files
 {
     public static class Storage
     {
         static Storage()
         {
             SaveFolder = Application.dataPath + "/Storage/";
             
             Initialize();
             LoadProfiles();
         }
         
         /// <summary>
         /// Расширение файла для сохраняемых данных
         /// </summary>
         private const string SaveExtension = ".txt";

         /// <summary>
         /// Папка сохраняемых данных. Оканчивается на "/"
         /// </summary>
         private static readonly string SaveFolder;

         /// <summary>
         /// Глобальные данные.
         /// </summary>
         public static DataProvider Global { get; private set; }
         
         /// <summary>
         /// Сокращение для глобальных данных.
         /// </summary>
         public static DataProvider g => Global;
         
         /// <summary>
         /// Данные принадлежащие профилю.
         /// </summary>
         public static DataProvider Local { get; set; }

         /// <summary>
         /// Сокращение для локальных данных.
         /// </summary>
         public static DataProvider l => Local;

         /// <summary>
         /// Все существующие профили.
         /// </summary>
         private static List<DataProfile> _profiles;

         private static void Initialize()
         {
             // Проверка существования папки с сохранениями
             if (Directory.Exists(SaveFolder))
             {
                 // Создание отсутствующей папки с сохранениями
                 Directory.CreateDirectory(SaveFolder);
                 
             }
         }

         private static void LoadProfiles()
         {
             var globalProfile = new DataProfile("Global");
             var globalProvider = new DataProvider(globalProfile, SaveFolder, new OdinSerializer(), SaveExtension);

             Global = globalProvider;

             _profiles = Global.LoadClassEntities<DataProfile>("Profiles/");
             
             // Если локальный профиль не найден, то он будет ссылаться на глобальный профиль
             if (_profiles.Count == 0)
             {
                 Local = Global;
                 return;
             }

             var localProfile = _profiles[0];
             Local = new DataProvider(localProfile, SaveFolder, new OdinSerializer(),SaveExtension);
         }

         /// <summary>
         /// Получение всех существующих профилей.
         /// Возвращается копия списка. Однако элементы списка НЕ копии!
         /// </summary>
         public static List<DataProfile> GetProfiles()
         {
             return _profiles.ToList();
         }

         /// <summary>
         /// Установка локального профиля.
         /// Если профиль не будет найдет в списке, то вылетит исключение. 
         /// </summary>
         public static void SetLocal(DataProfile profile)
         {
             if(!_profiles.Contains(profile))
                 throw new Exception($"Profile {profile.Name} not exist in current list of profiles!");
             
             Local = new DataProvider(profile, SaveFolder, new OdinSerializer(), SaveExtension);
         }

         /// <summary>
         /// Получение данных о профиле.
         /// </summary>
         /// <returns></returns>
         public static PropertyFileProxy GetLocalProfileInfo()
         {
             var proxy = Local.GetProxy("info");
             return proxy;
         }
         
     }
}





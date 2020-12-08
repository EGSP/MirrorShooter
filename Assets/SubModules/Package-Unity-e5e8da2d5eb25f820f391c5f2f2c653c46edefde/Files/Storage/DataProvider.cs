using System.Collections.Generic;
using System.IO;
using System.Linq;
using Egsp.Files.Serializers;

namespace Egsp.Files
{
    public class DataProvider
    {
        /// <summary>
        /// Используемый профиль.
        /// </summary>
        public readonly DataProfile Profile;

        /// <summary>
        /// Сериализатор, используемый провайдером.
        /// </summary>
        public readonly ISerializer Serializer;
        
        /// <summary>
        /// Корневая папка, куда сохраняются все файлы.
        /// </summary>
        public readonly string RootFolder;
        
        /// <summary>
        /// Расширения сохраняемых файлов.
        /// </summary>
        public readonly string Extension;

        public DataProvider(DataProfile profile, string rootFolder, string extension = ".txt")
        {
            Profile = profile;
            // Заменить на стандартный System.Text.Json
            Serializer = new OdinSerializer();

            RootFolder = rootFolder + "/" + profile.Name+"/";
            Extension = extension;
        }
        
        public DataProvider(DataProfile profile, string rootFolder, ISerializer serializer, string extension = ".txt")
        {
            Profile = profile;
            Serializer = serializer;

            RootFolder = rootFolder + "/" + profile.Name + "/";
            Extension = extension;
        }
        
        /// <summary>
        /// Возвращает прокси для файла со свойствами.
        /// При отсутствии файла создает новый, если createDefault == true.
        /// </summary>
        public PropertyFileProxy GetProxy(string filePath, bool createDefault = true)
        {
            var path = RootFolder + filePath + Extension;
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            if (File.Exists(path))
            {
                var fs = File.Open(path,
                    FileMode.Open, FileAccess.ReadWrite);

                return new PropertyFileProxy(fs);
            }

            if (createDefault)
            {
                var fs = new FileStream(path, FileMode.Create,
                    FileAccess.ReadWrite);

                return new PropertyFileProxy(fs);
            }

            throw new FileNotFoundException();
        }

        /// <summary>
        /// Сохраняет любую сущность с помощью сериализации.
        /// </summary>
        public void SaveEntity<T>(T entity, string filePath)
        {
            var path = CombineFilePath(filePath);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            
            var data = Serializer.Serialize(entity);

            // Перезапись старого файла.
            var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            var bw = new BinaryWriter(fs);
            bw.Write(data);
            
            fs.Close();
        }
        
        /// <summary>
        /// Загружает любую сущность с помощью десериализации.
        /// </summary>
        public T LoadEntity<T>(string filePath)
        {
            var path = CombineFilePath(filePath);

            if (!File.Exists(path))
                return default(T);

            var data = File.ReadAllBytes(path);

            var entity = Serializer.Deserialize<T>(data);

            return entity;
        }

        public void SaveEntities<T>(string directoryPath, List<T> entities)
        {
            var path = CombineDirectoryPath(directoryPath);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            
            var directoryInfo = new DirectoryInfo(path);
            foreach (var fileInfo in directoryInfo.GetFiles()
                .Where(fileInfoValue => fileInfoValue.Extension == Extension))
            {
                fileInfo.Delete();
            }
            
            for (var i = 0; i < entities.Count; i++)
            {
                var data = Serializer.Serialize(entities[i]);
                
                var fs = File.OpenWrite(path + $"{typeof(T).Name}_{i}" + Extension);
                var bw = new BinaryWriter(fs);
                bw.Write(data);
                
                fs.Close();
            }
        }

        /// <summary>
        /// Загружает все сущности и игнорирует несериализованные значения.
        /// </summary>
        public List<T> LoadStructEntities<T>(string directoryPath, bool ignoreDefault = true)
            where T : struct
        {
            var path = CombineDirectoryPath(directoryPath);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var directoryInfo = new DirectoryInfo(path);
            
            var list = new List<T>();

            var defaultEntity = default(T);
            
            foreach (var file in directoryInfo.GetFiles())
            {
                var data = File.ReadAllBytes(file.FullName);
                var entity = Serializer.Deserialize<T>(data);
                
                // Игнорирование стандартных значений. Для классов null
                if (ignoreDefault)
                {
                    if (!object.Equals(entity, defaultEntity))
                        list.Add(entity);
                }
                else
                {
                    list.Add(entity);
                }
            }

            return list;
        }
        
        /// <summary>
        /// Загружает все сущности и игнорирует несериализованные значения.
        /// </summary>
        public List<T> LoadClassEntities<T>(string directoryPath)
            where T : class
        {
            var path = CombineDirectoryPath(directoryPath);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var directoryInfo = new DirectoryInfo(path);
            
            var list = new List<T>();

            var defaultEntity = default(T);
            
            foreach (var file in directoryInfo.GetFiles()
                .Where(fileInfoValue => fileInfoValue.Extension == Extension))
            {
                var data = File.ReadAllBytes(file.FullName);
                var entity = Serializer.Deserialize<T>(data);

                if (entity != null)
                    list.Add(entity);
            }

            return list;
        }

        public string CombineFilePath(string filePath)
        {
            return RootFolder + filePath + Extension;
        }

        public string CombineDirectoryPath(string directoryPath)
        {
            return RootFolder + directoryPath;
        }
    }
}
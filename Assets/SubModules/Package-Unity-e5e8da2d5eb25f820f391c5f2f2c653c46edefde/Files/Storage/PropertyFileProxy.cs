using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Egsp.Files
{
    public class PropertyFileProxy
    {
        /// <summary>
        /// Поток открытого файла.
        /// </summary>
        protected readonly FileStream FileStream;
        
        public PropertyFileProxy(FileStream fileStream,
            NotExistProperty notExistProperty = NotExistProperty.Default,
            IncorrectPropertyType incorrectPropertyType = IncorrectPropertyType.RewriteDefault)
        {
            if(fileStream == null)
                throw new NullReferenceException();
            
            FileStream = fileStream;

            NotExistProperty = notExistProperty;
            IncorrectPropertyType = incorrectPropertyType;
            
            Properties = new Dictionary<string, string>();
            LoadProperties();
        }

        /// <summary>
        /// Свойства хранимые в файле.
        /// </summary>
        public Dictionary<string, string> Properties { get; protected set; }
        
        /// <summary>
        /// Как заместитель будет реагировать на отсутствие свойств.
        /// </summary>
        public NotExistProperty NotExistProperty { get; set; }
        
        /// <summary>
        /// Как заместитель будет реагировать на неверный тип.
        /// </summary>
        public IncorrectPropertyType IncorrectPropertyType { get; set; }

        /// <summary>
        /// Строковый шаблон для любых свойств.
        /// </summary>
        protected static string AnyPropertyPattern = "(\\S*)\\s*:\\s*(\\S*)\\s*;";
        
        /// <summary>
        /// Строковый шаблон для свойст-значений.
        /// </summary>
        protected static string NumberPropertyPattern = "(\\w*)\\s*:\\s*([-+]?[0-9]*[\\,.]?[0-9]+)\\s*;";

        public static bool MatchAnyProperty(string input, out string key, out string value)
        {
            var regex = new Regex(AnyPropertyPattern);
            
            var match = regex.Match(input);

            // Если строка является свойством
            if (match.Success)
            {
                key = match.Groups[1].Value;
                value = match.Groups[2].Value;
                return true;
            }

            key = value = string.Empty;
            return false;
        }
        
        /// <summary>
        /// Загружает все свойства из файла
        /// </summary>
        protected virtual void LoadProperties()
        {
            var sr = new StreamReader(FileStream);
            
            // Пока не конец файла
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                
                string key, value;
                if (MatchAnyProperty(line, out key, out value))
                {
                    // Добавляем новое свойство, если ключ не занят
                    if (Properties.ContainsKey(key) == false)
                        Properties.Add(key, value);
                }
            }
            
            FileStream.Close();
        }
        
        // GET

        /// <summary>
        /// Проверяет свойство на существование.
        /// Если свойства не существует, то будет возвращен второй аргумент.
        /// </summary>
        private bool CheckProperty(string pName, string pDefault, out string propertyValue)
        {
            // Проверка на существование свойства
            if (Properties.TryGetValue(pName, out propertyValue))
            {
                return true;
            }
            else
            {
                switch (NotExistProperty)
                {
                    case NotExistProperty.Default:
                        Properties.Add(pName, pDefault);
                        propertyValue = pDefault;
                        break;

                    case NotExistProperty.Exception:
                        throw new Exception($"Property :{pName}: does not exist!");
                        break;
                }
            }

            return false;
        }
        
        public PropertyFileProxy GetInt(string pName, ref int value)
        {
            string str;

            CheckProperty(pName, "0", out str);
            
            // Попытка привести к типу
            if (int.TryParse(str, out value))
            {
                // Успешное приведение
                return this;
            }
            else
            {
                switch (IncorrectPropertyType)
                {
                    case IncorrectPropertyType.RewriteDefault:
                        Properties[pName] = "0";
                        value = 0;
                        return this;

                    case IncorrectPropertyType.Exception:
                        throw new Exception($"Property :{pName}: does not exist!");
                        break;
                }
            }
            
            return this;
        }

        public PropertyFileProxy GetFloat(string pName, ref float value)
        {
            string str;

            CheckProperty(pName, "0", out str);
            
            str = str.Replace('.',',');
            
            // Попытка привести к типу
            if (float.TryParse(str, out value))
            {
                // Успешное приведение
                return this;
            }
            else
            {
                switch (IncorrectPropertyType)
                {
                    case IncorrectPropertyType.RewriteDefault:
                        Properties[pName] = "0";
                        value = 0;
                        return this;

                    case IncorrectPropertyType.Exception:
                        throw new Exception($"Property :{pName}: does not exist!");
                        break;
                }
            }
            return this;
        }

        public PropertyFileProxy GetBool(string pName, ref bool value)
        {
            string str;

            CheckProperty(pName, "false", out str);
            
            // Попытка привести к типу
            if (bool.TryParse(str, out value))
            {
                // Успешное приведение
                return this;
            }
            else
            {
                switch (IncorrectPropertyType)
                {
                    case IncorrectPropertyType.RewriteDefault:
                        Properties[pName] = "false";
                        value = false;
                        return this;

                    case IncorrectPropertyType.Exception:
                        throw new Exception($"Property :{pName}: does not exist!");
                        break;
                }
            }
            return this;
        }

        public PropertyFileProxy GetString(string pName, ref string value)
        {
            CheckProperty(pName, "0", out value);
            
            return this;
        }
        
        // SET
        
        /// <summary>
        /// Устанавливает значение по ключу.
        /// При отсутствии ключа создает новую позицию.
        /// </summary>
        public PropertyFileProxy SetInt(string pName, int value)
        {
            if (Properties.ContainsKey(pName))
            {
                Properties[pName] = value.ToString();
                return this;
            }
            else
            {
                Properties.Add(pName, value.ToString());
                return this;
            }
            
            return this;
        }

        /// <summary>
        /// Устанавливает значение по ключу.
        /// При отсутствии ключа создает новую позицию.
        /// </summary>
        public PropertyFileProxy SetFloat(string pName, float value)
        {
            if (Properties.ContainsKey(pName))
            {
                Properties[pName] = value.ToString();
                return this;
            }
            else
            {
                Properties.Add(pName, value.ToString());
                return this;
            }
            
            return this;
        }

        /// <summary>
        /// Устанавливает значение по ключу.
        /// При отсутствии ключа создает новую позицию.
        /// </summary>
        public PropertyFileProxy SetBool(string pName, bool value)
        {
            if (Properties.ContainsKey(pName))
            {
                Properties[pName] = value.ToString();
                return this;
            }
            else
            {
                Properties.Add(pName, value.ToString());
                return this;
            }
            
            return this;
        }

        /// <summary>
        /// Устанавливает значение по ключу.
        /// При отсутствии ключа создает новую позицию.
        /// </summary>
        public PropertyFileProxy SetString(string pName, string value)
        {
            if (Properties.ContainsKey(pName))
            {
                Properties[pName] = value;
                return this;
            }
            else
            {
                Properties.Add(pName, value);
                return this;
            }
            
            return this;
        }

        /// <summary>
        /// Перезаписывает файл.
        /// </summary>
        public void Apply()
        {
            // Возвращает поток, позволяющий записывать данные.
            var applyStream = File.Create(FileStream.Name);
            var writeStream = new StreamWriter(applyStream);

            // Записываем все свойства.
            foreach (var property in Properties)
            {
                var line = $"{property.Key}: {property.Value};";
                
                writeStream.WriteLine(line);
            }
            
            writeStream.Close();
            applyStream.Close();
        }
    }

    public enum NotExistProperty
    {
        Default,
        Exception
    }

    public enum IncorrectPropertyType
    {
        RewriteDefault,
        Exception
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SmartReservoir
{
    class Loader
    {
        private const string fileName = @"params.json";

        public string gasName = "gasstorage_kanim";
        public string liquidName = "smartliquidreservoir_kanim";
        public float gasStorage = 150f;
        public float liquidStorage = 20000f;

        private static Loader internalConfig = null;

        public static Loader Config
        {
            get
            {
                if (internalConfig == null)
                {
                    string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    internalConfig = LoadConfig<Loader>(Path.Combine(assemblyFolder, fileName));
                }
                return internalConfig;
            }
        }

        protected static T LoadConfig<T>(string path) where T : class
        {
            try
            {
                JsonSerializer serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings { Formatting = Formatting.Indented, ObjectCreationHandling = ObjectCreationHandling.Replace });

                T result = (T)Activator.CreateInstance(typeof(T));
                using (StreamReader streamReader = new StreamReader(path))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                    {
                        result = serializer.Deserialize<T>(jsonReader);
                        jsonReader.Close();
                    }
                    streamReader.Close();
                }
                return result;
            }
            catch
            {
                return (T)Activator.CreateInstance(typeof(T));
            }

        }
    }
}

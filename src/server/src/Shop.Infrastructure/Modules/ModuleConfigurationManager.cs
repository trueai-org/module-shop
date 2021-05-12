using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Shop.Infrastructure.Modules
{
    public class ModuleConfigurationManager : IModuleConfigurationManager
    {
        const string MODULES_FILE_NAME = "appsettings.Modules.json";

        public IEnumerable<ModuleInfo> GetModules()
        {
            var modules = new List<ModuleInfo>();
            var modulesPath = Path.Combine(GlobalConfiguration.ContentRootPath, MODULES_FILE_NAME);
            using (var reader = new StreamReader(modulesPath))
            {
                string content = reader.ReadToEnd();
                modules = JsonConvert.DeserializeObject<List<ModuleInfo>>(content);

                //dynamic modulesData = JsonConvert.DeserializeObject(content);
                //foreach (dynamic module in modulesData)
                //{
                //    yield return new ModuleInfo
                //    {
                //        Id = module.id,
                //        Version = Version.Parse(module.version.ToString())
                //    };
                //}
            }
            return modules;
        }
    }
}

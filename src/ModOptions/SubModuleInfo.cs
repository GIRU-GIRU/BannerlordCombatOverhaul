using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GCO.ModOptions
{
    class SubModuleInfo
    {
        public SubModuleContents DeserializeSubModule()
        {
            try
            {
                string fileName = "SubModule.xml";
                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var path = Path.Combine(location, @"..\..\", fileName);

                XmlRootAttribute root = new XmlRootAttribute("Module");
                XmlSerializer serializer = new XmlSerializer(typeof(SubModuleContents), root);

                StreamReader reader = new StreamReader(path);

                SubModuleContents contents = (SubModuleContents)serializer.Deserialize(reader);

                reader.Close();

                return contents;
            }
            catch (Exception)
            {
                return null;
            }      
        }
    }
}

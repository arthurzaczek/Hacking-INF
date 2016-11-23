using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Hacking_INF
{
    public static class BL
    {
        public static string ReadTextFile(string fileName)
        {
            using (var sr = new StreamReader(fileName))
            {
                return sr.ReadToEnd();
            }
        }

        public static T ReadYAML<T>(string fileName)
        {
            using (var input = new StreamReader(fileName))
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(new CamelCaseNamingConvention())
                    .IgnoreUnmatchedProperties()
                    .Build();
                return deserializer.Deserialize<T>(input);
            }
        }
    }
}
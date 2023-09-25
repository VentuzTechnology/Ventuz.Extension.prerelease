using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Extension;

namespace RDVX
{
    [VxToolBox("Json Query", "Data", "JQuery", "Selects Tokens from Json Strings and outputs them as array")]
    [VxHelpUrl("https://www.realtime-department.de", "Realtime Department Home Page")]
    [VxHelpUrl("https://www.newtonsoft.com/json/help/html/SelectToken.htm", "This Node uses \"Select Tokens\" as described here")]
    [VxIcon("NodeIcons.Data.Json")]
    [VxDescriptionAttribute("One or more queries as string or string array","Queries")]
    [VxDescriptionAttribute("The Json String to be parsed", "JsonString")]
    [VxDescriptionAttribute("Content of the parsed Tokens. Use Json Parser node to select the values", "JsonTokens")]
    [VxCategory("Json", "JsonString", "Queries","JsonTokens")]
    public class JsonQuery : VxContentNode
    {
        public JContainer Validate_JsonContainer(string JsonString)
        {
            JContainer parsedcontainer = null;
            if (JsonString.Trim()[0] == '[')
            {
                try
                {
                    parsedcontainer = JArray.Parse(JsonString);
                }
                catch (Exception ex)
                {
                    VX.Log.Error("JsonQuery", $"Could not parse Array", ex);
                }
            }
            if (JsonString.Trim()[0] == '{')
            {
                try
                {
                    parsedcontainer = JObject.Parse(JsonString);
                }
                catch (Exception ex)
                {
                    VX.Log.Error("JsonQuery", $"Could not parse Object", ex);
                }
            }
            return parsedcontainer;
        }
        public string ValidateJsonTokens(JContainer _JsonContainer, string[] Queries)
        {
            string outputJson = null;
            if (_JsonContainer != null && Queries != null && Queries.Any())
            {
                var outputlist = new Dictionary<string, IEnumerable<JToken>>();
                
                foreach (var propertyname in Queries)
                {
                    var tokens = _JsonContainer.SelectTokens(propertyname, false);
                    if (tokens != null && tokens.Any())
                    {
                        outputlist[propertyname] = _JsonContainer.SelectTokens(propertyname, false);
                    }
                }
                if (outputlist.Any())
                {
                    outputJson = JsonConvert.SerializeObject(outputlist);
                }
            }
            return outputJson;
        }
    }
}
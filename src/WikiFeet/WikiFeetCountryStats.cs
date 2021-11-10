/*
 * Copyright 2021 XXIV
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WikiFeet
{
    /// <summary>
    /// Gets WikiFeet stats by country.
    /// </summary>
    /// <see cref="WikiFeetStats"/>
    public class WikiFeetCountryStats
    {
        private readonly string _countryName;
        private const string PatternRomanFeet = "\\[\\\"Country\\\", \\\"Favor longer second toe\\\"\\],(.*?\\s*\\s])";
        private const string PatternFootTattoos = "\\[\\\"Country\\\", \\\"Favor foot tattoos\\\"\\],(.*?\\s*\\s])";
        private const string PatternPaintedToes = "\\[\\\"Country\\\", \\\"Favor painted toes\\\"\\],(.*?\\s*\\s])";
        private const string PatternSecretFeetLover = "\\[\\\"Country\\\", \\\"Keep it secret\\\"\\],(.*?\\s*\\s])";
        
        /// <summary>
        /// WikiFeetCountryStats constructor specifying country name.
        /// </summary>
        /// <param name="countryName">The name of the country.</param>
        public WikiFeetCountryStats(string countryName)
        {
            this._countryName = countryName;
        }
        
        private async Task<string> Http(string modelUrl)
        {
            HttpClient client = new HttpClient();
            try
            {
                Task<string> task = client.GetStringAsync(modelUrl);
                string data = await task;
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string Json(string info, string key)
        {
            try
            {
                if (info != null)
                {
                    string data = JObject.Parse(Country(info))[key].ToString();
                    if (data != null)
                    {
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        private string Country(string info) {
            try 
            {
                JArray array = JArray.Parse(info);;
                JObject json = new JObject();
                for (int i = 0; i < array.Count; i++) {
                    var data = array[i];
                    string name = data[0].ToString();
                    string value = data[1]["f"].ToString();
                    json.Add(name.ToUpper(), value);
                }
                return json.ToString();
            } catch (Exception) {
                return null;
            }
        }

        private string RomanFeetInfo()
        {
            string url = Http("https://www.wikifeet.com/polls/1").Result;
            Regex regex = new Regex(PatternRomanFeet);
            if (url != null)
            {
                Match matcher = regex.Match(url);
                if (matcher.Success)
                {
                    return $"[{matcher.Groups[1]}";
                }
            }
            else
            {
                return null;
            }

            return null;
        }
        
        private string FootTattoosInfo()
        {
            string url = Http("https://www.wikifeet.com/polls/2").Result;
            Regex regex = new Regex(PatternFootTattoos);
            if (url != null)
            {
                Match matcher = regex.Match(url);
                if (matcher.Success)
                {
                    return $"[{matcher.Groups[1]}";
                }
            }
            else
            {
                return null;
            }

            return null;
        }
        
        private string PaintedToesInfo()
        {
            string url = Http("https://www.wikifeet.com/polls/3").Result;
            Regex regex = new Regex(PatternPaintedToes);
            if (url != null)
            {
                Match matcher = regex.Match(url);
                if (matcher.Success)
                {
                    return $"[{matcher.Groups[1]}";
                }
            }
            else
            {
                return null;
            }

            return null;
        }
        
        private string SecretFeetLoverInfo()
        {
            string url = Http("https://www.wikifeet.com/polls/4").Result;
            Regex regex = new Regex(PatternSecretFeetLover);
            if (url != null)
            {
                Match matcher = regex.Match(url);
                if (matcher.Success)
                {
                    return $"[{matcher.Groups[1]}";
                }
            }
            else
            {
                return null;
            }

            return null;
        }
        
        /// <summary>
        /// Gets roman feet stats.
        /// </summary>
        /// <returns>Stats of the people who like roman feet.</returns>
        public string RomanFeet()
        {
            string data = Json(RomanFeetInfo(), _countryName.ToUpper());
            if (data != null) {
                return data;
            } else {
                return null;
            }
        }
        
        /// <summary>
        /// Gets foot tattoos stats.
        /// </summary>
        /// <returns>Stats of the people who like foot tattoos.</returns>
        public string FootTattoos()
        {
            string data = Json(FootTattoosInfo(), _countryName.ToUpper());
            if (data != null) {
                return data;
            } else {
                return null;
            }
        }
        
        /// <summary>
        /// Gets painted toes stats.
        /// </summary>
        /// <returns>Stats of the people who like painted toes.</returns>
        public string PaintedToes()
        {
            string data = Json(PaintedToesInfo(), _countryName.ToUpper());
            if (data != null) {
                return data;
            } else {
                return null;
            }
        }
        
        /// <summary>
        /// Gets secret feet lover stats.
        /// </summary>
        /// <returns>Stats of the people who like feet and keep it secret.</returns>
        public string SecretFeetLover()
        {
            string data = Json(SecretFeetLoverInfo(), _countryName.ToUpper());
            if (data != null) {
                return data;
            } else {
                return null;
            }
        }
    }
}
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
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WikiFeet
{
    /// <summary>
    /// Gets model from rank list.
    /// </summary>
    public class FeetOfTheYear
    {
        private int _index;
        private readonly int _modelId;
        private readonly string _modelInfo; 
        private const string PatternRanksList = "<h2>#(.*?): (.*?)</h2><div class='round8 celebbox'>\\s*<div class=boxcont><a href=\\\"/(.*?)\\\">";
        private const string PatternRatingInfo = "width:100%'>(.*?)<br><span style='color:#abc'>(.*?)</span>";
        private const string PatternId = "messanger\\['gdata'\\] \\= (\\[.*?\\]);";
        private const string PatternShoeSize = "id=ssize_label>(.*?)<a";
        private const string PatternBirthPlace = "id=nation_label>(.*?)<a";
        private const string PatternBirthDate = "id=bdate_label>(.*?)<a";
        private const string PatternRating = "white-space:nowrap' >&nbsp;\\((.*?) feet\\)</div>";
        private const string PatternRatingStats = "Feet rating stats \\((.*?) (.*?)<br>";
        private const string PatternRatingGorgeous = "Rating(.*?)&nbsp;\\((.*?) feet\\)</div>";
        private const string PatternImdb = "www.imdb.com/(.*?)'(.*?)Go to IMDb page";
        
        /// <summary>
        /// FeetOfTheYear constructor specifying the rank of the model.
        /// </summary>
        /// <param name="modelId">The rank of the model.</param>
        public FeetOfTheYear(int modelId)
        {
            this._modelId = modelId;
            this._modelInfo = ModelInfo();
        }
        
        /// <summary>
        /// FeetOfTheYear constructor specifying the rank and the index of the model.
        /// </summary>
        /// <param name="modelId">The rank of the model.</param>
        /// <param name="index">The position of the model.</param>
        public FeetOfTheYear(int modelId, int index)
        {
            this._modelId = modelId;
            this._index = index;
            this._modelInfo = ModelInfo();
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
                    string data = JObject.Parse(info)[key].ToString();
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

        private async Task<string> RankList()
        {
            string url = await Http("https://www.wikifeet.com/feetoftheyear");
                if (url != null)
                {
                    Regex regex = new Regex(PatternRanksList);
                    MatchCollection collection = regex.Matches(url);
                    List<string> list = new List<string>();
                    foreach (Match matcher in collection)
                    {
                        string modelId = matcher.Groups[1].Value;
                        string modelName = matcher.Groups[2].Value;
                        string modelUsername = new Regex("_| ").Replace(matcher.Groups[3].Value, "-");
                        string modelUrl = $"https://www.wikifeet.com/{matcher.Groups[3].Value}";
                        list.Add($"{{\"id\": {modelId}, \"name\": \"{modelName}\", \"username\": \"{modelUsername}\", \"url\": \"{modelUrl}\"}}");
                    }
                    string data = string.Join( ",", list);
                    return $"[{data}]";
                }
                else
                {
                    return null;
                }
        }
    
        private string RankInfo()
        {
            try
            {
                if (RankList().Result != null)
                {
                    JArray json = JsonConvert.DeserializeObject<JArray>(RankList().Result);
                    List<string> list = new List<string>();
                    foreach (JObject obj in json)
                    {
                        if (obj["id"].ToString() == _modelId.ToString())
                        {
                            string modelId = obj["id"].ToString();
                            string modelName = obj["name"].ToString();
                            string modelUsername = obj["username"].ToString();
                            string modelUrl = obj["url"].ToString();
                            list.Add($"{{\"id\": {modelId}, \"name\": \"{modelName}\", \"username\": \"{modelUsername}\", \"url\": \"{modelUrl}\"}}");
                        }
                    }

                    string data = string.Join(",", list);
                    return $"[{data}]";
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

        private string ModelInfo()
        {
            JArray json = null;
            try
            {
                if (RankInfo() != null)
                {
                    json = JArray.Parse(RankInfo());
                    if (_index == 1) {
                        _index = 0;
                    } else if (_index == 2) {
                        _index = 1;
                    }
                    var data = json[_index];
                    string modelId = data["id"].ToString();
                    string modelName = data["name"].ToString();
                    string modelUsername = data["username"].ToString();
                    string modelUrl = data["url"].ToString();
                    return $"{{\"id\": {modelId}, \"name\": \"{modelName}\", \"username\": \"{modelUsername}\", \"url\": \"{modelUrl}\"}}";
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                try
                {
                    var data = json[0];
                    string modelId = data["id"].ToString();
                    string modelName = data["name"].ToString();
                    string modelUsername = data["username"].ToString();
                    string modelUrl = data["url"].ToString();
                    return $"{{\"id\": {modelId}, \"name\": \"{modelName}\", \"username\": \"{modelUsername}\", \"url\": \"{modelUrl}\"}}";
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        
        private async Task<string> RatingInfo()
        {
            string dataUrl = Json(_modelInfo, "url");
            string dataName = Json(_modelInfo,"name");
            if (dataUrl != null && dataName != null)
            {
                string url = await Http(dataUrl);
                Regex regex = new Regex(PatternRatingInfo);
                if (url != null)
                {
                    MatchCollection collection = regex.Matches(url);
                    JObject json = new JObject();
                    foreach (Match matcher in collection)
                    {
                        string key = matcher.Groups[2].Value;
                        string value = matcher.Groups[1].Value;
                        string data = $"{{\"{key}\": \"{value}\"}}";
                        json.Add(key,value);
                    }
                    return json.ToString();
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

        private async Task<string> Id()
        {
            string dataUrl = Json(_modelInfo, "url");
            string dataName = Json(_modelInfo, "name");
            if (dataUrl != null && dataName != null)
            {
                string url = await Http(dataUrl);
                Regex regex = new Regex(PatternId);
                if (url != null)
                {
                    Match matcher = regex.Match(url);
                    if (matcher.Success)
                    {
                        JArray json = JsonConvert.DeserializeObject<JArray>(matcher.Groups[1].Value);
                        List<string> list = new List<string>();
                        foreach (JObject obj in json)
                        {
                            list.Add(obj["pid"].ToString());
                        }
                        Random random = new Random();
                        int index = random.Next(list.Count);
                        return list[index].ToString();
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
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the name of the model.
        /// </summary>
        /// <returns>The name of the model.</returns>
        public string ModelName()
        {
            string data = Json(_modelInfo, "name");
            if (data != null)
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the page of the model.
        /// </summary>
        /// <returns>The page of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string ModelPage()
        {
            string dataUrl = Json(_modelInfo, "url");
            string dataName = Json(_modelInfo, "name");
            if (dataUrl != null && dataName != null)
            {
                string url = Http(dataUrl).Result;
                if (url != null)
                {
                    return dataUrl;
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

        /// <summary>
        /// Gets thumbnail of the model.
        /// </summary>
        /// <returns>Random thumbnail of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string Thumbnail()
        { 
            if (Id().Result != null)
            {
                return $"https://thumbs.wikifeet.com/{Id().Result}.jpg";
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets image of the model.
        /// </summary>
        /// <returns>Random image of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string Image()
        {
            string data = Json(_modelInfo,"username");
            if (Id().Result != null && data != null)
            {
                    return $"https://pics.wikifeet.com/{data}-feet-{Id().Result}.jpg";
            }
            else
            {
                    return null;
            }
        }

        /// <summary>
        /// Gets shoe size of the model.
        /// </summary>
        /// <returns>The shoe size of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string ShoeSize()
        {
            string dataUrl = Json(_modelInfo,"url");
            string dataName = Json(_modelInfo,"name");
            if (dataUrl != null && dataName != null) {
                string url = Http(dataUrl).Result;
                if (url != null) {
                    Regex regex = new Regex(PatternShoeSize);
                    Match matcher = regex.Match(url);
                    if (matcher.Success) {
                        return matcher.Groups[1].Value;
                    } else {
                        return null;
                    }
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        /// <summary>
        /// Gets the birthplace of the model.
        /// </summary>
        /// <returns>The birthplace of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string BirthPlace()
        {
            string dataUrl = Json(_modelInfo,"url");
            string dataName = Json(_modelInfo,"name");
            if (dataUrl != null && dataName != null) {
                string url = Http(dataUrl).Result;
                if (url != null) {
                    Regex regex = new Regex(PatternBirthPlace);
                    Match matcher = regex.Match(url);
                    if (matcher.Success) {
                        return matcher.Groups[1].Value;
                    } else {
                        return null;
                    }
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        /// <summary>
        /// Gets the birthdate of the model.
        /// </summary>
        /// <returns>The birthdate of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string BirthDate()
        {
            string dataUrl = Json(_modelInfo,"url");
            string dataName = Json(_modelInfo,"name");
            if (dataUrl != null && dataName != null) {
                string url = Http(dataUrl).Result;
                if (url != null) {
                    Regex regex = new Regex(PatternBirthDate);
                    Match matcher = regex.Match(url);
                    if (matcher.Success) {
                        return matcher.Groups[1].Value;
                    } else {
                        return null;
                    }
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        /// <summary>
        /// Gets the rating of the model.
        /// </summary>
        /// <returns>The rating of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string Rating()
        {
            string dataUrl = Json(_modelInfo,"url");
            string dataName = Json(_modelInfo,"name");
            if (dataUrl != null && dataName != null) {
                string url = Http(dataUrl).Result;
                if (url != null) {
                    Match matcher = new Regex(PatternRating).Match(url);
                    Match matcherGorgeous = new Regex(PatternRatingGorgeous).Match(url);
                    if (matcher.Success) {
                        return matcher.Groups[1].Value;
                    } else if (matcherGorgeous.Success)
                    {
                        return matcherGorgeous.Groups[2].Value;
                    }
                    else
                    {
                        return null;
                    }
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        /// <summary>
        /// Gets the beautiful rating of the model.
        /// </summary>
        /// <returns>The beautiful rating of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string BeautifulRating()
        {
            string data = Json(RatingInfo().Result, "beautiful");
                if (data != null)
                {
                    return data;
                }
                else
                {
                    return null;
                }
        }

        /// <summary>
        /// Gets the nice rating of the model.
        /// </summary>
        /// <returns>The nice rating of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string NiceRating()
        {
            string data = Json(RatingInfo().Result, "nice");
                if (data != null)
                {
                    return data;
                }
                else
                {
                    return null;
                }
        }
        
        /// <summary>
        /// Gets the ok rating of the model.
        /// </summary>
        /// <returns>The ok rating of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string OkRating()
        {
            string data = Json(RatingInfo().Result, "ok");
                if (data != null)
                {
                    return data;
                }
                else
                {
                    return null;
                }
        }
        
        /// <summary>
        /// Gets the bad rating of the model.
        /// </summary>
        /// <returns>The bad rating of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string BadRating()
        {
                string data = Json(RatingInfo().Result, "bad");
                if (data != null)
                {
                    return data;
                }
                else
                {
                    return null;
                }
        }
        
        /// <summary>
        /// Gets the ugly rating of the model.
        /// </summary>
        /// <returns>The ugly rating of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string UglyRating()
        {
                string data = Json(RatingInfo().Result, "ugly");
                if (data != null)
                {
                    return data;
                }
                else
                {
                    return null;
                }
        }

        /// <summary>
        /// Gets the rating stats of the model.
        /// </summary>
        /// <returns>The rating stats of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string RatingStats()
        {
            string dataUrl = Json(_modelInfo,"url");
            string dataName = Json(_modelInfo,"name");
            if (dataUrl != null && dataName != null) {
                string url = Http(dataUrl).Result;
                if (url != null) {
                    Regex regex = new Regex(PatternRatingStats);
                    Match matcher = regex.Match(url);
                    if (matcher.Success) {
                        return matcher.Groups[1].Value;
                    } else {
                        return null;
                    }
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        /// <summary>
        /// Gets the imdb page of the model.
        /// </summary>
        /// <returns>The imdb page of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string ImdbPage()
        {
            string dataUrl = Json(_modelInfo,"url");
            string dataName = Json(_modelInfo,"name");
            if (dataUrl != null && dataName != null) {
                string url = Http(dataUrl).Result;
                if (url != null) {
                    Regex regex = new Regex(PatternImdb);
                    Match matcher = regex.Match(url);
                    if (matcher.Success) {
                        return $"https://www.imdb.com/{matcher.Groups[1].Value}";
                    } else {
                        return null;
                    }
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }
    }
}
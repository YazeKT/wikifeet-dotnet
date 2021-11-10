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
    /// Gets model by specifying the name in the constructor.
    /// </summary>
    public class WikiFeetSearch
    {
        private readonly string _modelName;
        private readonly string _modelInfo;
        private const string PatternModelInfo = ".value='(.*?)';parent.location='\\/' \\+ encodeURI\\('(.*?)'\\)";
        private const string PatternRatingInfo = "width:100%'>(.*?)<br><span style='color:#abc'>(.*?)</span>";
        private const string PatternId = "messanger\\['gdata'\\] \\= (\\[.*?\\]);";
        private const string PatternShoeSize = "id=ssize_label>(.*?)<a";
        private const string PatternBirthPlace = "id=nation_label>(.*?)<a";
        private const string PatternBirthDate = "id=bdate_label>(.*?)<a";
        private const string PatternRating = "white-space:nowrap' >&nbsp;\\((.*?) feet\\)</div>";
        private const string PatternRatingStats = "Feet rating stats \\((.*?) (.*?)<br>";
        private const string PatternRatingGorgeous = "Rating(.*?)&nbsp;\\((.*?) feet\\)</div>";
        private const string PatternImdb = "www.imdb.com/(.*?)'(.*?)Go to IMDb page";
        private const string PatternAdult = "WARNING: CONTAINS ADULT CONTENT(.*?)Switch to wikiFeet X";

        /// <summary>
        /// WikiFeetSearch constructor specifying the name of the model.
        /// </summary>
        /// <param name="modelName">The name of the model.</param>
        public WikiFeetSearch(string modelName)
        {
            this._modelName = modelName;
            this._modelInfo = ModelInfo().Result;
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
        
        private bool IsAdultContent(string url) {
            Regex regex = new Regex(PatternAdult);
            Match matcher = regex.Match(url);
            if (matcher.Success) {
                return true;
            } else {
                return false;
            }
        }

        private async Task<string> ModelInfo()
        {
            try
            {
                string url = await Http($"https://www.wikifeet.com/perl/ajax.fpl?req=suggest&gender=undefined&value={Uri.EscapeDataString(_modelName)}");
                if (url != null)
                {
                    string replacement = new Regex("\\\\").Replace(url, "");
                    Regex regex = new Regex(PatternModelInfo);
                    if (replacement != null)
                    {
                        Match matcher = regex.Match(replacement);
                        if (matcher.Success)
                        {
                            string modelName = matcher.Groups[1].Value;
                            string modelUsername = new Regex("_| ").Replace(matcher.Groups[2].Value, "-");
                            string modelUrl = $"https://www.wikifeet.com/{matcher.Groups[2].Value}";
                            return $"{{\"name\": \"{modelName}\", \"username\": \"{modelUsername}\", \"url\": \"{modelUrl}\"}}";
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
            catch (Exception)
            {
                return null;
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
                    if (IsAdultContent(url)) {
                        throw new AdultContentException(dataName);
                    }
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
                    if (IsAdultContent(url)) {
                        throw new AdultContentException(dataName);
                    } else if (matcher.Success)
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
                    if (IsAdultContent(url)) {
                        throw new AdultContentException(dataName);
                    }
                    else
                    {
                        return dataUrl;
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
        /// Gets thumbnail of the model.
        /// </summary>
        /// <returns>Random thumbnail of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string Thumbnail()
        {
            try
            {
                if (Id().Result != null)
                {
                    return $"https://thumbs.wikifeet.com/{Id().Result}.jpg";
                }
                else
                {
                    return null;
                }
            } catch (AdultContentException ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Gets image of the model.
        /// </summary>
        /// <returns>Random image of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string Image()
        {
            try 
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
            } catch (AdultContentException ex)
            {
                throw ex;
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
                    if (IsAdultContent(url)) {
                        throw new AdultContentException(dataName);
                    } else if (matcher.Success) {
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
                    if (IsAdultContent(url)) {
                        throw new AdultContentException(dataName);
                    } else if (matcher.Success) {
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
                    if (IsAdultContent(url)) {
                        throw new AdultContentException(dataName);
                    } else if (matcher.Success) {
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
                    if (IsAdultContent(url)) {
                        throw new AdultContentException(dataName);
                    } else if (matcher.Success) {
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
            try
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
            catch (AdultContentException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the nice rating of the model.
        /// </summary>
        /// <returns>The nice rating of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string NiceRating()
        {
            try
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
            catch (AdultContentException ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Gets the ok rating of the model.
        /// </summary>
        /// <returns>The ok rating of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string OkRating()
        {
            try
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
            catch (AdultContentException ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Gets the bad rating of the model.
        /// </summary>
        /// <returns>The bad rating of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string BadRating()
        {
            try
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
            catch (AdultContentException ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Gets the ugly rating of the model.
        /// </summary>
        /// <returns>The ugly rating of the model.</returns>
        /// <exception cref="AdultContentException">Thrown if it contains adult content.</exception>
        public string UglyRating()
        {
            try
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
            catch (AdultContentException ex)
            {
                throw ex;
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
                    if (IsAdultContent(url)) {
                        throw new AdultContentException(dataName);
                    } else if (matcher.Success) {
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
                    if (IsAdultContent(url)) {
                        throw new AdultContentException(dataName);
                    } else if (matcher.Success) {
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
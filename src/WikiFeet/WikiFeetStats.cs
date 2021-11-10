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

namespace WikiFeet
{
    /// <summary>
    /// Gets WikiFeet stats.
    /// </summary>
    /// <see cref="WikiFeetCountryStats"/>
    public class WikiFeetStats
    {
        private const string PatternRomanFeet = "Liked Roman / Egyptian feet better(.*?) width:(.*?)%'>(.*?)</div></td></tr>";
        private const string PatternGreekFeet = "Liked Greek feet / Morton's toe better(.*?) width:(.*?)%'>(.*?)</div></td></tr>";
        private const string PatternNotFootTattoos = "I Don't like foot tattoos(.*?) width:(.*?)%'>(.*?)</div></td></tr>";
        private const string PatternMaybeFootTattoos ="I Sometimes like foot tattoos(.*?) width:(.*?)%'>(.*?)</div></td></tr>";
        private const string PatternFootTattoos = "I Like foot tattoos(.*?) width:(.*?)%'>(.*?)</div></td></tr>";
        private const string PatternNaturalToes = "I like natural toes better(.*?) width:(.*?)%'>(.*?)</div></td></tr>";
        private const string PatternPaintedToes = "I like painted toes better(.*?) width:(.*?)%'>(.*?)</div></td></tr>";
        private const string PatternSecretFeetLover = "No, I keep it to myself(.*?) width:(.*?)%'>(.*?)</div></td></tr>";
        private const string PatternOpenFeetLover = "Yes, I am open about it(.*?) width:(.*?)%'>(.*?)</div></td></tr>";
        
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
        
        /// <summary>
        /// Gets roman feet stats.
        /// </summary>
        /// <returns>Stats of the people who like roman feet.</returns>
        /// <see cref="GreekFeet"/>
        public string RomanFeet()
        {
            string url = Http("https://www.wikifeet.com/polls/1").Result;
            Regex regex = new Regex(PatternRomanFeet);
            if (url != null)
            {
                Match matcher = regex.Match(url);
                if (matcher.Success)
                {
                    return matcher.Groups[3].Value;
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
        /// Gets greek feet stats.
        /// </summary>
        /// <returns>Stats of the people who like greek feet.</returns>
        /// <see cref="RomanFeet"/>
        public string GreekFeet()
        {
            string url = Http("https://www.wikifeet.com/polls/1").Result;
            Regex regex = new Regex(PatternGreekFeet);
            if (url != null)
            {
                Match matcher = regex.Match(url);
                if (matcher.Success)
                {
                    return matcher.Groups[3].Value;
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
        /// Gets foot tattoos stats.
        /// </summary>
        /// <returns>Stats of the people who don't like foot tattoos.</returns>
        /// <see cref="FootTattoos"/>
        /// <see cref="MaybeFootTattoos"/>
        public string NotFootTattoos()
        {
            string url = Http("https://www.wikifeet.com/polls/2").Result;
            Regex regex = new Regex(PatternNotFootTattoos);
            if (url != null)
            {
                Match matcher = regex.Match(url);
                if (matcher.Success)
                {
                    return matcher.Groups[3].Value;
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
        /// Gets foot tattoos stats.
        /// </summary>
        /// <returns>Stats of the people who sometimes like foot tattoos.</returns>
        /// <see cref="FootTattoos"/>
        /// <see cref="NotFootTattoos"/>
        public string MaybeFootTattoos()
        {
            string url = Http("https://www.wikifeet.com/polls/2").Result;
            Regex regex = new Regex(PatternMaybeFootTattoos);
            if (url != null)
            {
                Match matcher = regex.Match(url);
                if (matcher.Success)
                {
                    return matcher.Groups[3].Value;
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
        /// Gets foot tattoos stats.
        /// </summary>
        /// <returns>Stats of the people who like foot tattoos.</returns>
        /// <see cref="NotFootTattoos"/>
        /// <see cref="MaybeFootTattoos"/>
        public string FootTattoos()
        {
            string url = Http("https://www.wikifeet.com/polls/2").Result;
            Regex regex = new Regex(PatternFootTattoos);
            if (url != null)
            {
                Match matcher = regex.Match(url);
                if (matcher.Success)
                {
                    return matcher.Groups[3].Value;
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
        /// Gets natural toes stats.
        /// </summary>
        /// <returns>Stats of the people who like natural toes.</returns>
        /// <see cref="PaintedToes"/>
        public string NaturalToes()
        {
            string url = Http("https://www.wikifeet.com/polls/3").Result;
            Regex regex = new Regex(PatternNaturalToes);
            if (url != null)
            {
                Match matcher = regex.Match(url);
                if (matcher.Success)
                {
                    return matcher.Groups[3].Value;
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
        /// Gets painted toes stats.
        /// </summary>
        /// <returns>Stats of the people who like painted toes.</returns>
        /// <see cref="NaturalToes"/>
        public string PaintedToes()
        {
            string url = Http("https://www.wikifeet.com/polls/3").Result;
            Regex regex = new Regex(PatternPaintedToes);
            if (url != null)
            {
                Match matcher = regex.Match(url);
                if (matcher.Success)
                {
                    return matcher.Groups[3].Value;
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
        /// Gets secret feet lover stats.
        /// </summary>
        /// <returns>Stats of the people who like feet and keep it secret.</returns>
        /// <see cref="OpenFeetLover"/>
        public string SecretFeetLover()
        {
            string url = Http("https://www.wikifeet.com/polls/4").Result;
            Regex regex = new Regex(PatternSecretFeetLover);
            if (url != null)
            {
                Match matcher = regex.Match(url);
                if (matcher.Success)
                {
                    return matcher.Groups[3].Value;
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
        /// Gets open feet lover stats.
        /// </summary>
        /// <returns>Stats of the people who like feet and open about it.</returns>
        /// <see cref="SecretFeetLover"/>
        public string OpenFeetLover()
        {
            string url = Http("https://www.wikifeet.com/polls/4").Result;
            Regex regex = new Regex(PatternOpenFeetLover);
            if (url != null)
            {
                Match matcher = regex.Match(url);
                if (matcher.Success)
                {
                    return matcher.Groups[3].Value;
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
    }
}
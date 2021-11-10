# WikiFeet

[![](https://img.shields.io/github/v/tag/sloppydaddy/wikifeet-dotnet?label=version)](https://github.com/sloppydaddy/wikifeet-dotnet/releases/latest) [![](https://img.shields.io/github/license/sloppydaddy/wikifeet-dotnet)](https://github.com/sloppydaddy/wikifeet-dotnet/blob/main/LICENSE)

WikiFeet (The collaborative celebrity feet website) crawler for **.NET**

### Download
[NuGet](https://www.nuget.org/packages/WikiFeet/)

**.NET CLI**
```
dotnet add package WikiFeet
```
**NuGet CLI**
```
nuget install WikiFeet
```
**Package Manager**
```
Install-Package WikiFeet
```

### Example

```csharp
static void Main(string[] args)
{
    WikiFeetSearch feet = new WikiFeetSearch("Megan Fox");
    Console.WriteLine("Megan Fox feet pic: " + feet.Image());
    Console.WriteLine("Megan Fox shoe size: " + feet.ShoeSize());
    Console.WriteLine("Megan Fox feet rating: " + feet.Rating());
            
    /*
    Output:
    Megan Fox feet pic: https://pics.wikifeet.com/Megan-Fox-Feet-1186163.jpg
    Megan Fox shoe size: 7 US 
    Megan Fox feet rating: gorgeous
     */
}
```

### Documentation

* [DocFX](https://sloppydaddy.github.io/wikifeet-dotnet)

### License

WikiFeet is released under the [Apache License 2.0](https://github.com/sloppydaddy/wikifeet-dotnet/blob/main/LICENSE).

```
 Copyright 2021 XXIV

 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
```
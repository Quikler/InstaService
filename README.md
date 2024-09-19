# InstaService

InstaService is a wrapper over <a href="https://github.com/ramtinak/InstagramApiSharp">InstagramApiSharp</a>  library. Library provides a simple functionality for parsing and downloading media from instagram.

To start working with library you should add using statement:

```
using InstagramService.Classes;
```

Then declare an instance of ```InstagramService``` class:
```
// configure your api first
IInstaApi api = InstaApiBuilder.CreateBuilder().Build();
var service = new InstagramService(api);
```

Classes:

1. ```InstaMediaProcessor``` - a class to work with instagram media.
2. ```InstaStreamProcessor``` - a class to grab a stream/streams from specified instagram media uri.

Models:

1. ```InstaMediaInfo``` - represents a model of media with info only. Info is just simple url of media
2. ```InstaMediaStream``` - represents a model of media with stream data and info.

## Installation

Install it from <a href="https://www.nuget.org/packages/InstaService/">**Nuget**</a> using dotnet CLI:

```bash
dotnet add package InstaService
```

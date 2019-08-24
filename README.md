# ActivityReceiver.iOS
![License: Mozilla](https://img.shields.io/github/license/Neko3000/ActivityReceiver.iOS)
![Platforms: Windows | Linux](https://img.shields.io/badge/Platform-Windows%20%7C%20Linux-lightgrey)
![Language: C#](https://img.shields.io/badge/language-C%23-blue)
![Version: v1.20](https://img.shields.io/badge/version-v1.20-lightgrey)

ActivityReceiver is a API&CMS service(server-side) based on Windows/Linux.</br>
It works with client-side - [ActivityReceiver.iOS]().</br>

We retrieved users' Learning Records, and replay their operation for word reordering problems, calculating parameters for Machine Learning.</br>
It also allows administrators to manage contents of problems, exercises, grammars... on it.

## Installation
Clone it by:

```
$ git clone https://github.com/Neko3000/ActivityReceiver
```

The first thing is setting up a environment for ASP.NET Core 2.0, you may need to know how to build up LAMP on Liux or deloy it on Window with IIS, 
, please refer to [Microsoft's doc](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-2.2).</br>
Then, install SQL Server, change the SQL connection strings to yours in the project.</br>
Lastly, start the service.

## How to use
<p align="center"> 
<img width="300" src="https://raw.githubusercontent.com/Neko3000/resource-storage/master/img/screenshot/starrytarget-sr1.gif" alt="screen-record-1">
<img width="300" src="https://raw.githubusercontent.com/Neko3000/resource-storage/master/img/screenshot/starrytarget-sr2.gif" alt="screen-record-2">
</p>

<p align="center"> 
<img src="https://raw.githubusercontent.com/Neko3000/resource-storage/master/img/screenshot/starrytarget-s1.png" width="200" alt="">
<img src="https://raw.githubusercontent.com/Neko3000/resource-storage/master/img/screenshot/starrytarget-s2.png" width="200" alt="">
<img src="https://raw.githubusercontent.com/Neko3000/resource-storage/master/img/screenshot/starrytarget-s3.png" width="200" alt="">
<img src="https://raw.githubusercontent.com/Neko3000/resource-storage/master/img/screenshot/starrytarget-s4.png" width="200" alt="">
</p>

## Features
- [x] Sign In/Sign UP
- [x] Authorization with Roles
- [x] API with ActivityReceiver.iOS
- [x] Manage Problems
- [x] Manage Exercises
- [x] Replay Learning Records
- [x] More...

## Dependencies
[Meronic](https://keenthemes.com/metronic/) is used to build up Front-End interface.</br>
[ListSwap](https://github.com/phedde/listSwap) handles the ordering behavior for managing exercises.</br>

Using Nuget Packages to manage dependencies automatically.

Packages have been included:

```
<PackageReference Include="AutoMapper" Version="8.0.0" />
<PackageReference Include="CsvHelper" Version="12.1.2" />
<PackageReference Include="Microsoft.AspNetCore.All" Version="2.0.6" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="2.0.2" PrivateAssets="All" />
<PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="2.0.3" PrivateAssets="All" />
```


## Development
You should use your own client using [ActivityReceiver.iOS]().</br>

## Contact To Me
E-mail: sheran_chen@outlook.com </br>
Weibo: @妖绀

## License
Distributed under the Mozilla license. See LICENSE for more information.

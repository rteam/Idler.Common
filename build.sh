dotnet restore -s:https://api.nuget.org/v3/index.json

# 基础组件
find ./nupkgs/ -name *.nupkg -type f |xargs rm -f
dotnet pack ./Idler.Common.Core/Idler.Common.Core.csproj -c:Release --output ./nupkgs
dotnet pack ./Idler.Common.Autofac/Idler.Common.Autofac.csproj -c:Release --output ./nupkgs
dotnet pack ./Idler.Common.AutoMapper/Idler.Common.AutoMapper.csproj -c:Release --output ./nupkgs
dotnet pack ./Idler.Common.Cache/Idler.Common.Cache.csproj -c:Release --output ./nupkgs
dotnet pack ./Idler.Common.Cache.FreeRedis/Idler.Common.Cache.FreeRedis.csproj -c:Release --output ./nupkgs
dotnet pack ./Idler.Common.EntityFrameworkCore/Idler.Common.EntityFrameworkCore.csproj -c:Release --output ./nupkgs
dotnet pack ./Idler.Common.FluentEmail/Idler.Common.FluentEmail.csproj -c:Release --output ./nupkgs

# 授权组件
dotnet pack ./Idler.Common.Authorization/Idler.Common.Authorization.csproj -c:Release --output ./nupkgs
dotnet pack ./Idler.Common.Authorization.Abstractions/Idler.Common.Authorization.Abstractions.csproj -c:Release --output ./nupkgs
dotnet pack ./Idler.Common.Authorization.EntityFrameworkCore/Idler.Common.Authorization.EntityFrameworkCore.csproj -c:Release --output ./nupkgs
dotnet pack ./Idler.Common.Authorization.AspNetCore/Idler.Common.Authorization.AspNetCore.csproj -c:Release --output ./nupkgs
#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/api/PoliceDepartment.EvidenceManager.API/PoliceDepartment.EvidenceManager.API.csproj", "src/api/PoliceDepartment.EvidenceManager.API/"]
COPY ["src/shared-kernel/PoliceDepartment.EvidenceManager.SharedKernel/PoliceDepartment.EvidenceManager.SharedKernel.csproj", "src/shared-kernel/PoliceDepartment.EvidenceManager.SharedKernel/"]
COPY ["src/api/PoliceDepartment.EvidenceManager.Application/PoliceDepartment.EvidenceManager.Application.csproj", "src/api/PoliceDepartment.EvidenceManager.Application/"]
COPY ["src/api/PoliceDepartment.EvidenceManager.Domain/PoliceDepartment.EvidenceManager.Domain.csproj", "src/api/PoliceDepartment.EvidenceManager.Domain/"]
COPY ["src/api/PoliceDepartment.EvidenceManager.Infra/PoliceDepartment.EvidenceManager.Infra.csproj", "src/api/PoliceDepartment.EvidenceManager.Infra/"]
RUN dotnet restore "src/api/PoliceDepartment.EvidenceManager.API/PoliceDepartment.EvidenceManager.API.csproj"
COPY . .
WORKDIR "/src/src/api/PoliceDepartment.EvidenceManager.API"
RUN dotnet build "PoliceDepartment.EvidenceManager.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PoliceDepartment.EvidenceManager.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PoliceDepartment.EvidenceManager.API.dll"]
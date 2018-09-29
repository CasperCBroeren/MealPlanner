FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY ["MealPlanner.Web/MealPlanner.Web.csproj", "MealPlanner.Web/"]
COPY ["MealPlanner.Data/MealPlanner.Data.csproj", "MealPlanner.Data/"]
RUN dotnet restore "MealPlanner.Web/MealPlanner.Web.csproj"
COPY . .
WORKDIR "/src/MealPlanner.Web"
RUN dotnet build "MealPlanner.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MealPlanner.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MealPlanner.Web.dll"]

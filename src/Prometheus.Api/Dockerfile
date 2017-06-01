FROM microsoft/aspnetcore:1.1.2

WORKDIR /app

COPY . .

ENV ASPNETCORE_URLS http://+:5000

EXPOSE 5000

ENTRYPOINT ["dotnet", "Prometheus.Api.dll"]

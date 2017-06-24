FROM microsoft/aspnetcore:1.1.2

WORKDIR /app

COPY . .

ENV ASPNETCORE_URLS http://+:8088

EXPOSE 8088

ENTRYPOINT ["dotnet", "Prometheus.Api.dll"]

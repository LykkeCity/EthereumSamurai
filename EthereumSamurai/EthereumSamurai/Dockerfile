FROM microsoft/aspnetcore:1.1.1
ENTRYPOINT ["dotnet", "EthereumSamurai.dll"]
ARG source=.
WORKDIR /app
COPY $source .

EXPOSE 5000/tcp

ENV ASPNETCORE_URLS http://*:5000

FROM microsoft/dotnet:2-sdk
ENTRYPOINT ["dotnet", "EthereumSamurai.Indexer.dll"]
ARG source=.
WORKDIR /app
COPY $source .

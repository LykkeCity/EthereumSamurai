FROM microsoft/dotnet:runtime
ENTRYPOINT ["dotnet", "EthereumSamurai.Indexer.dll"]
ARG source=.
WORKDIR /app
COPY $source .

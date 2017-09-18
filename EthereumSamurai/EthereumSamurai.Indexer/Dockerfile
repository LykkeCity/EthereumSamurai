FROM microsoft/dotnet:1.1-runtime
ENTRYPOINT ["dotnet", "EthereumSamurai.Indexer.dll"]
ARG source=.
WORKDIR /app
COPY $source .

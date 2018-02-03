FROM microsoft/dotnet:2-sdk
ENTRYPOINT ["dotnet", "Lykke.Job.EthereumSamurai.dll"]
ARG source=.
WORKDIR /app
COPY $source .

# EthereumSamurai
Ethereum Indexer

## Structure
EthereumSamurai consists of two parts: API and Indexing job.
API is EthereumSamurai project.
Indexing job is EthereumSamurai.Indexer project.

## Configuration
Both project should recieve configuration via http request to configuration service(would be changed in the future).
Configuration Model is described by **IBaseSettings** interface.

To configure indexing job you should pass additional configuration(see appsettings.json in EthereumSamurai.Indexer):
>"IndexerInstanceSettings": {
    "IndexerId": "FirstIndexer",
    "ThreadAmount": 4,
    "StartBlock": 0,
    "StopBlock": null
  },

If application exits before indexing is completed you can restart it and it would start indexing from the last synced point(points if ThreadAmount > 1).
- **IndexerId** - any string. If you want to run multiple indexers on several VM.
- **ThreadAmount** - amount of threads for that particular indexer instance.
- **StartBlock** - block wrom which indexing start 
- **StopBlock** - stop the indexing instance on the block with *BlockNumber == StopBlock*(Leave it null or skip in configuration)

## Logic behind ThreadAmount
Example: 
If last **BlockNumber** on the blockchain is 100 and the indexer instance has the next configuration:
- **ThreadAmount** = 2;
- **StartBlock** = 0;
- **StopBlock** is not set or is null;
In that case indexing jobs would process blockchain in two threads in the next ways:
1. First thread would process blocks in the range 1 - 50.
2. The other one would process blocks in the range 51 - ... and would download new blocks and deal with blockchain forks until the application is stopped.

namespace Lykke.Service.EthereumSamurai.Models
{
    public class BlockContext
    {
        public BlockContext(string jobId, int jobVersion, string indexerId, BlockContent blockContent)
        {
            IndexerId    = indexerId;
            JobId        = jobId;
            JobVersion   = jobVersion;
            BlockContent = blockContent;
        }

        public BlockContent BlockContent { get; }

        public string IndexerId { get; }

        public string JobId { get; }

        public int JobVersion { get; }
    }
}
namespace Lykke.Service.EthereumSamurai.Models
{
    public class BlockContext
    {
        public BlockContext(string jobId, int jobVersion, BlockContent blockContent)
        {
            JobId        = jobId;
            JobVersion   = jobVersion;
            BlockContent = blockContent;
        }

        public BlockContent BlockContent { get; }

        public string JobId { get; }

        public int JobVersion { get; }
    }
}
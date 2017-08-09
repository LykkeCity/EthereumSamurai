namespace EthereumSamurai.Models
{
    public class BlockContext
    {
        public BlockContext(string indexerId, BlockContent blockContent)
        {
            IndexerId    = indexerId;
            BlockContent = blockContent;
        }

        public BlockContent BlockContent { get; }

        public string IndexerId { get; }
    }
}
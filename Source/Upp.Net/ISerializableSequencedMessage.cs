namespace Upp.Net
{
    public interface ISequencedMessage
    {
        ushort SequenceId { get; set; }
    }
}
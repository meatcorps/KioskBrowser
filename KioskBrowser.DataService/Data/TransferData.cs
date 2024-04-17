using System.Buffers.Text;

namespace KioskBrowser.DataService.Data;

public class TransferData : IDisposable, IAsyncDisposable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public StringWriter Chunks { get; set; } = new ();
    public int TotalChunks { get; set; }
    public int TransferSize { get; set; }
    public DateTime ChangeDate { get; set; }
    public string MetaData { get; set; }
    public string Hash { get; set; } = "";

    public byte[] GetBytes()
    {
        return Convert.FromBase64String(Chunks.ToString());
    }

    public void Dispose()
    {
        Chunks.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Chunks.DisposeAsync();
    }
}
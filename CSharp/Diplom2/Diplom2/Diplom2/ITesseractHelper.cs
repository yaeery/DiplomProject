using System.Threading;
using System.Threading.Tasks;

public interface ITesseractHelper
{
    Task<string> Main(byte[] imageData,bool Language, CancellationToken token);
}

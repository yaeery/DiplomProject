using Diplom2;
using System.Threading;
using System.Threading.Tasks;

public interface ITensorflowClassifier
{
    Task<ImageClassificationModel> Classify(byte[] image, bool Language, bool Is_Money_Recognize, CancellationToken token);
}

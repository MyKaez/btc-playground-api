using Application.Extensions;
using Xunit;

namespace Infrastructure.Integration;

public class ProofOfWork
{
    [Fact]
    public void FindBlock()
    {
        var hash = "";
        var hashNo = 0;
        var interactionId = Guid.NewGuid().ToString();

        while (!hash.StartsWith("0000"))
            hash = $"{interactionId}_{++hashNo}".ComputeSha256Hash();
        
        Assert.Equal(hash, $"{interactionId}_{hashNo}".ComputeSha256Hash());
    }
}
using System.IO;

namespace HousingFinanceInterimApi.V1.Domain
{
    public record FileInMemory(MemoryStream DataStream, string Name, string MimeType);
}

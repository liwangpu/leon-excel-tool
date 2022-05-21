using MediatR;

namespace ExcelTool.Queries.Files
{
    public class FileExistCheckQuery : IRequest<bool>
    {
        public string FileKey { get; protected set; }
        public FileExistCheckQuery(string fileKey)
        {
            FileKey = fileKey;
        }
    }
}

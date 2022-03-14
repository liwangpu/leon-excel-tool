namespace ExcelTool.Infrastructures.JTW
{
    public interface IUserService
    {
        bool IsValid(LoginDto request);
    }

    public class UserService : IUserService
    {
        //本次，固定校验admin
        public bool IsValid(LoginDto request)
        {
            return request.Password == request.Username && request.Username == "admin";
        }
    }
}

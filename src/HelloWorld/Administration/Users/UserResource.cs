using FubuMVC.Swank.Description;

namespace HelloWorld.Administration.Users
{
    public class UserResource : ResourceDescription
    {
         public UserResource()
         {
             Name = "Users";
             Comments = "This resource allows you to manage users.";
         }
    }
}
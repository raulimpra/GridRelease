using GridPromocional.Models;
using Microsoft.AspNetCore.Identity;

namespace GridPromocional.Services
{
    public interface IUserFamilyService
    {
        public List<PgCatFamily> GetFamilies();
        public List<IdentityRole> GetRoles();
        public List<UserRecord> GetUsers(string role, string family);
        public List<PgCatFamily> GetUserFamilies(string user);
        public void UpdateUserRole(string role, string users);
        public void SaveUserFamilies(string user, string families);
        public List<UserFamiliesReportRecord> GetUserFamiliesReport(string role, string family);
    }
}

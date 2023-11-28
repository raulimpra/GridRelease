using GridPromocional.Data;
using GridPromocional.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using GridPromocional.Exceptions;

namespace GridPromocional.Services
{
    public class UserFamilyService : IUserFamilyService
    {
        private readonly GridContext _gridContext;

        public UserFamilyService(GridContext gridContext)
        {
            _gridContext = gridContext;
        }

        public List<PgCatFamily> GetFamilies()
        {
            var rst = _gridContext.PgCatFamily.ToList();
            return rst;
        }

        public List<IdentityRole> GetRoles()
        {
            var rst = _gridContext.Roles.ToList();
            return rst;
        }

        public List<UserRecord> GetUsers(string role, string family)
        {
            role = string.IsNullOrEmpty(role) ? string.Empty : role;
            family = string.IsNullOrEmpty(family) ? string.Empty : family;

            var retVal = _gridContext.UserRecord.FromSqlRaw("GetUsers @role, @Family",
                    new SqlParameter("@role", role),
                    new SqlParameter("@family", family)).ToList();
            return retVal;
        }

        public List<PgCatFamily> GetUserFamilies(string user)
        {
            user = string.IsNullOrEmpty(user) ? string.Empty : user;

            var retVal = _gridContext.PgCatFamily.FromSqlRaw("FamiliesPerUser @User",
                    new SqlParameter("@User", user)).ToList();
            return retVal;
        }

        public void UpdateUserRole(string role, string users)
        {
            if (string.IsNullOrEmpty(role)) throw new GridException("El campo role no puede estar vacio.");
            if (string.IsNullOrEmpty(users)) throw new GridException("El campo users no puede estar vacio.");

            int rowsAfected = _gridContext.Database.ExecuteSqlRaw("UpdateRole @Perfil, @Usuarios",
                                new SqlParameter("@Perfil", role),
                                new SqlParameter("@Usuarios", users));

            //if (rowsAfected <= 0) throw new Exception("No se actualizo ningun registro");
        }

        public void SaveUserFamilies(string user, string families)
        {
            if (string.IsNullOrEmpty(user)) throw new GridException("El campo usuario no puede estar vacio.");
            //if (string.IsNullOrEmpty(families)) throw new GridException("No existen familias a procesar.");

            int rowsAfected = _gridContext.Database.ExecuteSqlRaw("SetFamiliesToUser @user, @families",
                                new SqlParameter("@user", user),
                                new SqlParameter("@families", families));

            if (rowsAfected <= 0) throw new Exception("No se actualizo ningun registro");
        }

        public List<UserFamiliesReportRecord> GetUserFamiliesReport(string role, string family)
        {
            role = string.IsNullOrEmpty(role) ? string.Empty : role;
            family = string.IsNullOrEmpty(family) ? string.Empty : family;

            var retVal = _gridContext.UserFamiliesReportRecord.FromSqlRaw("GetUserFamiliesReport @role, @family",
                    new SqlParameter("@role", role),
                    new SqlParameter("@family", family)).ToList();
            return retVal;
        }

    }
}

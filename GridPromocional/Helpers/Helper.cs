using GridPromocional.Data;
using GridPromocional.Models.Views;
using System.Data.SqlClient;
using System.Security.Claims;

namespace GridPromocional.Helpers
{
	public static class Helper
	{
		public static IEnumerable<T> executeReader<T>(SqlCommand cmd)
		{
			using (SqlDataReader reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					yield return (T)reader[0];
				}
			}
		}
		public static UserClaims getUserClaims(List<Claim> Claims)
		{
            UserClaims usrClm = new UserClaims();

            foreach (var claim in Claims)
			{
				switch (claim.Type)
                {
                    case "name": usrClm.userName = claim.Value; break;
                    case "preferred_username": usrClm.email = claim.Value; break;
                    case "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": usrClm.rol = claim.Value; break;
                }
			}
			return usrClm;
		}
    }	
}

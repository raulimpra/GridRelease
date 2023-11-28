namespace GridPromocional.Models.Views
{
    public class UserClaims
    {
        public UserClaims()
        {
            email = "usuario@correo";
        }
        public string userName { get; set; }
        public string codemp { get { return email.Split("@")[0]; } }
        public string email { get; set; }
        public string rol { get; set; }
        public List<PgCatMenuActions> actions { get; set; }
    }
}

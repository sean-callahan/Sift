using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sift.Server.Db
{
    internal class User
    {
        public int Id { get; set; }

        [Index(IsUnique = true)]
        [MaxLength(30)]
        public string Username { get; set; }

        [Column(TypeName = "char")]
        [StringLength(60)]
        public string PasswordHash { get; set; }
    }
}

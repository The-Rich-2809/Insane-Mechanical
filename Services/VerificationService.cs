using Insane_Mechanical.Models;

namespace Insane_Mechanical.Services
{
    public class VerificationService
    {
        private readonly Insane_MechanicalDB _context;

        public VerificationService(Insane_MechanicalDB context)
        {
            _context = context;
        }

        public void SaveVerificationCode(int userId, string verificationCode)
        {
            var usuario = _context.Usuario.Find(userId);
            usuario.Codigcadena = verificationCode;
            usuario.FechadeExpiracion = DateTime.UtcNow.AddMinutes(10);
            _context.SaveChanges();
        }

        public string GetStoredVerificationCode(int userId)
        {
            var user = _context.Usuario.Find(userId);
            if (user.FechadeExpiracion.HasValue && user.FechadeExpiracion > DateTime.UtcNow)
            {
                return user.Codigcadena;
            }
            return null;
        }

        public bool VerifyCode(int userId, string enteredCode)
        {
            var storedCode = GetStoredVerificationCode(userId);
            return storedCode != null && storedCode == enteredCode;
        }
    }
}

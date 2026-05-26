using Guryflix.Models;

namespace Guryflix.Utils
{
    /// <summary>
    /// Classe estática para gerir a sessão do utilizador atual em toda a aplicação.
    /// </summary>
    public static class Sessao
    {
        // Guarda o objeto do utilizador que fez login com sucesso
        public static Utilizador UtilizadorAtual { get; set; }

        // Método para verificar se existe alguém logado
        public static bool EstaLogado()
        {
            return UtilizadorAtual != null;
        }

        // Método para terminar a sessão (Logout)
        public static void TerminarSessao()
        {
            UtilizadorAtual = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horazon_Bank__projetoFinal
{
    public static class ContaSessao
    {
        public static string IdContaLogada { get; set; }

        public static event Action ValoresAlterados;

        public static string CodigoVerificacao { get; set; }

        private static ContaModelo ContaAtual => BancoDados.BuscarContaPorId(IdContaLogada);

        public static string Id => IdContaLogada;

        public static string Nome => ContaAtual?.Nome ?? "";
        public static string Apelido => ContaAtual?.Apelido ?? "";
        public static string Email => ContaAtual?.Email ?? "";
        public static string Senha => ContaAtual?.Senha ?? "";

        public static int Dia => ContaAtual?.Dia ?? 0;
        public static int Mes => ContaAtual?.Mes ?? 0;
        public static int Ano => ContaAtual?.Ano ?? 0;

        public static DateTime DataNascimento => new DateTime(Ano, Mes, Dia);
        public static string DataFormatada => DataNascimento.ToString("dd/MM/yyyy");

        public static int Idade
        {
            get
            {
                int idade = DateTime.Now.Year - DataNascimento.Year;
                if (DataNascimento.Date > DateTime.Now.AddYears(-idade))
                    idade--;
                return idade;
            }
        }

        public static decimal Saldo
        {
            get => ContaAtual?.Saldo ?? 0;
            set
            {
                BancoDados.AtualizarSaldo(IdContaLogada, value);
                ValoresAlterados?.Invoke();
            }
        }

        public static decimal Poupanca
        {
            get => ContaAtual?.Poupanca ?? 0;
            set
            {
                BancoDados.AtualizarPoupanca(IdContaLogada, value);
                ValoresAlterados?.Invoke();
            }
        }

        public static string CartaoCidadaoPassaporte => BancoDados.BuscarDocumentos(IdContaLogada)?.CartaoCidadaoPassaporte ?? "";
        public static string NIF => BancoDados.BuscarDocumentos(IdContaLogada)?.NIF ?? "";
        public static string Morada => BancoDados.BuscarDocumentos(IdContaLogada)?.Morada ?? "";

        public static EmprestimoModelo EmprestimoAtivoAtual => BancoDados.BuscarEmprestimoAtivo(IdContaLogada);

        public static bool EmprestimoAtivo => EmprestimoAtivoAtual != null;
        public static decimal SaldoDevedor => EmprestimoAtivoAtual?.SaldoDevedor ?? 0;
        public static decimal ParcelaMensal => EmprestimoAtivoAtual?.ParcelaMensal ?? 0;

        public static void AdicionarHistorico(string descricao, string tipo = "Geral", decimal valor = 0)
        {
            BancoDados.AdicionarHistorico(IdContaLogada, descricao, tipo, valor);
            ValoresAlterados?.Invoke();
        }

        public static string GerarId(int tamanho = 8)
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] id = new char[tamanho];

            for (int i = 0; i < tamanho; i++)
            {
                id[i] = caracteres[random.Next(caracteres.Length)];
            }

            return new string(id);
        }

        public static string GerarCodigoVerificacao()
        {
            Random random = new Random();
            int codigo = random.Next(0, 1000000);
            return codigo.ToString("D6");
        }

        public static void EncerrarSessao()
        {
            IdContaLogada = null;
            CodigoVerificacao = null;
        }
    }
}
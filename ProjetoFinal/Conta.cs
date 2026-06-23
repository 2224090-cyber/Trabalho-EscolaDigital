using System;
using System.Collections.Generic;

namespace Horazon_Bank__projetoFinal
{


    public static class Conta
    {
        private static decimal saldo;
        private static decimal poupanca;

        public static event Action ValoresAlterados;

        public static decimal Saldo
        {
            get { return saldo; }
            set
            {
                saldo = value;
                ValoresAlterados?.Invoke();
            }
        }

        public static void LimparHistorico()
        {
            Historico.Clear();
            ValoresAlterados?.Invoke();
        }


        public static decimal Poupanca
        {
            get { return poupanca; }
            set
            {
                poupanca = value;
                ValoresAlterados?.Invoke();
            }

        }

        public static List<string> Historico = new List<string>();

        public static void AdicionarHistorico(string texto)
        {
            Historico.Add($"[{DateTime.Now:dd/MM/yyyy HH:mm}] {texto}");
            ValoresAlterados?.Invoke();
        }

        public static decimal SaldoDevedor { get; set; } = 0;
        public static decimal ParcelaMensal { get; set; } = 0;
        public static bool EmprestimoAtivo { get; set; } = false;
        public static bool EmprestimoAprovado { get; set; } = false;

        public static string Nome { get; set; }
        public static string Apelido { get; set; }
        public static int Dia { get; set; }
        public static int Mes { get; set; }
        public static int Ano { get; set; }
        public static string Email { get; set; }
        public static string Senha { get; set; }
        public static string CartaoCidadaoPassaporte { get; set; }
        public static string NIF { get; set; }
        public static string Morada { get; set; }


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


        public static string Id { get; set; }

        private static readonly Random _random = new Random();

        public static string GerarId(int tamanho = 6)
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] id = new char[tamanho];

            for (int i = 0; i < tamanho; i++)
            {
                id[i] = caracteres[_random.Next(caracteres.Length)];
            }

            return new string(id);
        }



        public static string CodigoVerificacao { get; set; }

        public static string GerarCodigoVerificacao()
        {
            Random random = new Random();
            int codigo = random.Next(0, 1000000); // 0 a 999999
            return codigo.ToString("D6"); // garante 6 dígitos, com zeros à esquerda se preciso
        }



    }

}




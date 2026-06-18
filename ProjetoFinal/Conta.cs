using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
    
}




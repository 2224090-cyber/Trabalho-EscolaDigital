using System;
using System.Collections.Generic; // Essencial para a List funcionar

namespace Horazon_Bank__projetoFinal
{
    public static class Conta
    {
        public static decimal Saldo { get; set; } = 0;

        public static decimal Poupanca { get; set; } = 0; // Como vi que usas no Poupanca.cs

        // ESTA É A LINHA QUE RESOLVE O TEU ERRO:
        public static List<string> Operacoes { get; set; } = new List<string>();

        // Este é o evento que usaste no código da Poupança:
        public static event Action ValoresAlterados;
    }
}
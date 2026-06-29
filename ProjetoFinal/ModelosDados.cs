using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horazon_Bank__projetoFinal
{
    internal class ModelosDados
    {


    }


    public class ContaModelo
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Apelido { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public int Dia { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public string Genero { get; set; }
        public decimal Saldo { get; set; }
        public decimal Poupanca { get; set; }
    }

    public class DocumentoModelo
    {
        public string ContaId { get; set; }
        public string CartaoCidadaoPassaporte { get; set; }
        public string NIF { get; set; }
        public string Morada { get; set; }
    }

    public class EmprestimoModelo
    {
        public int Id { get; set; }
        public string ContaId { get; set; }
        public decimal ValorSolicitado { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ParcelaMensal { get; set; }
        public decimal SaldoDevedor { get; set; }
        public int Prazo { get; set; }
        public decimal TaxaJuros { get; set; }
        public bool Aprovado { get; set; }
        public bool Ativo { get; set; }
    }

    public class HistoricoModelo
    {
        public int Id { get; set; }
        public string ContaId { get; set; }
        public string Descricao { get; set; }
        public string Tipo { get; set; }
        public decimal Valor { get; set; }
        public string DataHora { get; set; }
    }
}





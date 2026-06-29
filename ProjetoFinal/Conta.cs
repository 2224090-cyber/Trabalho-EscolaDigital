using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient; 

namespace Horazon_Bank__projetoFinal
{
    public static class Conta
    {
       
        private static decimal saldo;
        private static decimal poupanca;
        private static decimal saldoDevedor = 0;
        private static decimal parcelaMensal = 0;
        private static bool emprestimoAtivo = false;
        private static bool emprestimoAprovado = false;

        
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

        public static decimal SaldoDevedor
        {
            get { return saldoDevedor; }
            set
            {
                saldoDevedor = value;
                ValoresAlterados?.Invoke();
            }
        }

        public static decimal ParcelaMensal
        {
            get { return parcelaMensal; }
            set
            {
                parcelaMensal = value;
                ValoresAlterados?.Invoke();
            }
        }

        public static bool EmprestimoAtivo
        {
            get { return emprestimoAtivo; }
            set
            {
                emprestimoAtivo = value;
                ValoresAlterados?.Invoke();
            }
        }

        public static bool EmprestimoAprovado
        {
            get { return emprestimoAprovado; }
            set
            {
                emprestimoAprovado = value;
                ValoresAlterados?.Invoke();
            }
        }

        
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
        public static string Id { get; set; }

        
        public static List<string> Historico = new List<string>();

        
        public static void AdicionarHistorico(string texto)
        {
            string linhaFormatada = $"[{DateTime.Now:dd/MM/yyyy HH:mm}] {texto}";

          
            Historico.Add(linhaFormatada);

            if (!string.IsNullOrEmpty(Id))
            {
                try
                {
                    using (SqlConnection conexao = Database.GetConnection())
                    {
                        conexao.Open();
                        string query = "INSERT INTO HistoricoTransacoes (UsuarioId, Texto) VALUES (@UsuarioId, @Texto)";

                        using (SqlCommand cmd = new SqlCommand(query, conexao))
                        {
                            cmd.Parameters.AddWithValue("@UsuarioId", Id);
                            cmd.Parameters.AddWithValue("@Texto", linhaFormatada);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                    System.Diagnostics.Debug.WriteLine("Erro ao gravar transação no SQL: " + ex.Message);
                }
            }

            
            ValoresAlterados?.Invoke();
        }

        public static void LimparHistorico()
        {
            Historico.Clear();
            ValoresAlterados?.Invoke();
        }

        
        public static DateTime DataNascimento
        {
            get
            {
                try
                {
                    
                    if (Ano <= 0 || Mes <= 0 || Dia <= 0)
                        return DateTime.MinValue;

                    return new DateTime(Ano, Mes, Dia);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }

        public static string DataFormatada
        {
            get
            {
                if (DataNascimento == DateTime.MinValue)
                    return "00/00/0000";

                return DataNascimento.ToString("dd/MM/yyyy");
            }
        }

        public static int Idade
        {
            get
            {
                if (DataNascimento == DateTime.MinValue)
                    return 0;

                try
                {
                    int idade = DateTime.Now.Year - DataNascimento.Year;
                    if (DataNascimento.Date > DateTime.Now.AddYears(-idade))
                        idade--;
                    return idade;
                }
                catch
                {
                    return 0;
                }
            }
        }

        
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
            int codigo = _random.Next(0, 1000000);
            return codigo.ToString("D6"); 
        }

       
        public static bool CarregarDadosDoSQL(string emailLogin)
        {
            using (SqlConnection conexao = Database.GetConnection())
            {
                try
                {
                    conexao.Open();
                    string query = "SELECT * FROM Utilizadores WHERE Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(query, conexao))
                    {
                        cmd.Parameters.AddWithValue("@Email", emailLogin);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                
                                Nome = reader["Nome"].ToString();
                                Apelido = reader["Apelido"].ToString();
                                Email = reader["Email"].ToString();
                                Senha = reader["Senha"].ToString();
                                Dia = Convert.ToInt32(reader["Dia"]);
                                Mes = Convert.ToInt32(reader["Mes"]);
                                Ano = Convert.ToInt32(reader["Ano"]);
                                CartaoCidadaoPassaporte = reader["CartaoCidadao"].ToString();
                                NIF = reader["NIF"].ToString();
                                Morada = reader["Morada"].ToString();

                              
                                saldo = Convert.ToDecimal(reader["Saldo"]);
                                poupanca = Convert.ToDecimal(reader["Poupanca"]);
                                saldoDevedor = Convert.ToDecimal(reader["SaldoDevedor"]);
                                parcelaMensal = Convert.ToDecimal(reader["ParcelaMensal"]);
                                emprestimoAtivo = Convert.ToBoolean(reader["EmprestimoAtivo"]);

                                
                                ValoresAlterados?.Invoke();
                                return true;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    
                    return false;
                }
            }
            return false;
        }
    }
}
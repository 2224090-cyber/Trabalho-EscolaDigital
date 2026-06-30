# Trabalho-EscolaDigital
trabalho
# Horazon Bank 

O **Horazon Bank** é uma aplicação desktop de simulação bancária desenvolvida em C# utilizando o ecossistema Windows Forms (.NET Framework) e SQL Server para o armazenamento e gestão de dados. O projeto simula operações bancárias essenciais com foco em segurança, controlo de acessos e integridade das informações dos utilizadores.

---

##  Funcionalidades Principais

* **Autenticação de Utilizadores:** Sistema de login seguro com validação de credenciais na base de dados.
* **Gestão de Senhas:** Funcionalidade para alteração e recuperação de senha com critérios de validação (mínimo de 6 caracteres, confirmação de igualdade e máscara visual `****` para privacidade).
* **Operações Bancárias:** (Adicione aqui outras funções que o seu projeto faz, ex: Consulta de saldo, transferências, depósitos, etc.)
* **Conexão Segura com BD:** Utilização de boas práticas com comandos parametrizados (`SqlCommand`) para prevenir ataques de SQL Injection.

---

## Tecnologias Utilizadas

* **Linguagem:** C# (.NET Framework)
* **Interface Gráfica:** Windows Forms (WinForms)
* **Base de Dados:** SQL Server
* **Persistência de Dados:** `System.Data.SqlClient` (ADO.NET)

---

## Estrutura do Código (Destaques)

O projeto segue a arquitetura padrão do Windows Forms, onde cada ecrã é gerido por um formulário próprio. 

* `confirmar_senha.cs`: Responsável pela lógica de alteração de senha, garantindo que:
    * Os campos não estejam vazios.
    * A nova senha tenha o tamanho mínimo exigido.
    * As senhas coincidam.
    * A senha seja atualizada tanto na Base de Dados como na sessão local da aplicação (`Conta.Senha`).

---

## Como Executar o Projeto

### Pré-requisitos
1. **Visual Studio** (versão 2019 ou superior) com a carga de trabalho de *Desenvolvimento de desktop com .NET* instalada.
2. **SQL Server** local ou remoto configurado.

### Passo a Passo
1. Clone este repositório para a sua máquina local:

   git clone [https://github.com/seu-utilizador/Horazon_Bank__projetoFinal.git](https://github.com/seu-utilizador/Horazon_Bank__projetoFinal.git)
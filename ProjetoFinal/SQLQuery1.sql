-- 1. Cria a Base de Dados
CREATE DATABASE HorizonBank;
GO

-- 2. Garante que os próximos comandos são executados dentro da base de dados criada
USE HorizonBank;
GO

-- 3. Cria a tabela com os campos correspondentes à classe Conta do C#
CREATE TABLE Utilizadores (
    Id VARCHAR(50) PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Apelido VARCHAR(100) NOT NULL,
    Email VARCHAR(150) UNIQUE NOT NULL,
    Senha VARCHAR(255) NOT NULL,
    Dia INT NOT NULL,
    Mes INT NOT NULL,
    Ano INT NOT NULL,
    CartaoCidadao VARCHAR(50),
    NIF VARCHAR(20),
    Morada VARCHAR(255),
    Saldo DECIMAL(18, 2) DEFAULT 0.00,
    Poupanca DECIMAL(18, 2) DEFAULT 0.00,
    SaldoDevedor DECIMAL(18, 2) DEFAULT 0.00,
    ParcelaMensal DECIMAL(18, 2) DEFAULT 0.00,
    EmprestimoAtivo BIT DEFAULT 0
);
GO

USE HorizonBank;
GO

CREATE TABLE HistoricoTransacoes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId VARCHAR(50) NOT NULL,
    Texto VARCHAR(255) NOT NULL,
    DataHora DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Historico_Utilizadores FOREIGN KEY (UsuarioId) REFERENCES Utilizadores(Id) ON DELETE CASCADE
);
GO

CREATE TABLE Emprestimos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId VARCHAR(50) NOT NULL, -- Corrigido para VARCHAR(50) igual ao Id de Utilizadores
    ValorSolicitado DECIMAL(18,2) NOT NULL,
    ValorDevedor DECIMAL(18,2) NOT NULL,
    ParcelaMensal DECIMAL(18,2) NOT NULL,
    TotalParcelas INT NOT NULL,
    ParcelasPagas INT DEFAULT 0,
    DataAprovacao DATETIME DEFAULT GETDATE(),
    Ativo BIT DEFAULT 1, -- 1 para Ativo, 0 para Liquidado
    CONSTRAINT FK_Emprestimos_Utilizadores FOREIGN KEY (UsuarioId) REFERENCES Utilizadores(Id) ON DELETE CASCADE
);
GO

CREATE TABLE Transferencias (
    Id INT IDENTITY(1,1) PRIMARY KEY, -- Identificador único da transferência
    RemetenteId VARCHAR(50) NOT NULL, -- Quem envia o dinheiro (ID do utilizador logado)
    DestinatarioId VARCHAR(50) NOT NULL, -- Quem recebe o dinheiro
    Valor DECIMAL(18, 2) NOT NULL,    -- O valor transferido
    DataHora DATETIME DEFAULT GETDATE(), -- Guarda a data e hora exata automaticamente

    -- Cria as ligações (Chaves Estrangeiras) com a tua tabela de Utilizadores
    FOREIGN KEY (RemetenteId) REFERENCES Utilizadores(Id),
    FOREIGN KEY (DestinatarioId) REFERENCES Utilizadores(Id)
);
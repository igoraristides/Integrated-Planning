
# Como Rodar um Aplicativo MVC Razor no Visual Studio Code

Este guia oferece instruções detalhadas sobre como rodar um aplicativo **MVC Razor** usando o **Visual Studio Code**. Vamos abordar os requisitos de instalação e a execução do projeto.

## Requisitos

Certifique-se de que você tenha as ferramentas e dependências corretas instaladas no seu sistema.

### 1. **Instalar o .NET SDK**

O aplicativo MVC Razor é baseado no framework **.NET**. Certifique-se de que o **.NET SDK** esteja instalado na sua máquina.

- **Versão Recomendada**: .NET 68.0 ou superior (para projetos MVC mais recentes).
- **Download**: [Baixar .NET SDK](https://dotnet.microsoft.com/download)

### 2. **Instalar Extensões do Visual Studio Code**

Instale as extensões necessárias para trabalhar com **ASP.NET Core MVC** e **Razor Pages** no Visual Studio Code.

1. **C#**: Para suporte ao C#.
   - Abra o **Visual Studio Code**.
   - Vá até a **Extensões** .
   - Procure por **C#** e instale a extensão da Microsoft.
   - Isso trará suporte ao IntelliSense, depuração e outras funcionalidades importantes.

## Passo a Passo: Rodando o Aplicativo MVC Razor no Visual Studio Code

### Passo 1: Abrir o Projeto no Visual Studio Code

1. Abra o **Visual Studio Code**.
2. Vá para **Arquivo** > **Abrir Pasta**.
3. Selecione a pasta onde o projeto foi clonado ou onde o código do projeto está localizado.

### Passo 2: Restaurar Pacotes NuGet

1. Abra o terminal do **Visual Studio Code** (`Ctrl+` ` ou **Terminal** > **Novo Terminal**).
2. Execute o comando para restaurar os pacotes NuGet necessários:

   ```bash
   dotnet restore
   ```

   Isso irá baixar todas as dependências necessárias para o projeto.

### Passo 3: Configuração do Banco de Dados

Se o seu projeto MVC utiliza um banco de dados, será necessário configurar a conexão e criar o banco de dados (se necessário).

1. **instale a extensão .NET Core User Secrets de Adrian Wilczyński o arquivo `appsettings.json`** para que seja possivel colocar a string de conexao, para isso você ira até PlanejamentoIntegrado.csproj + botao diretito do mouse e apertar em manage user secrets

   string de conexao:

   ```json
  "IntegratedPlanning": {
    "Oracle": {
      "ConnectionString": "User Id=admin;Password=5*Q3w467Lp67;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCPS)(HOST=adb.sa-saopaulo-1.oraclecloud.com)(PORT=1522))(CONNECT_DATA=(SERVICE_NAME=gcf7ccd43cfc06d_metalsaapp_high.adb.oraclecloud.com))(SECURITY=(SSL_SERVER_DN_MATCH=yes)))"
    }
  }
   ```

### Passo 5: Executar o Aplicativo

Agora que o projeto está configurado, você pode executar o aplicativo.

1. No terminal do **Visual Studio Code**, execute o comando:

   ```bash
   cd .\PlanejamentoIntegrado\

   dotnet run
   ```

2. O Visual Studio Code irá compilar o projeto e você verá um link no terminal para o local

3. Abra o navegador e vá até o endereço exibido.

# Projeto de Testes - PlanejamentoIntegrado

Este projeto contém testes unitários abrangentes para o sistema PlanejamentoIntegrado, utilizando **xUnit** e **Moq** para mocking.

## Estrutura dos Testes

Os testes estão organizados em uma estrutura de pastas que espelha o projeto principal:

### 📁 Base
- **BaseTest.cs** - Classe base com utilitários comuns para testes

### 📁 Controllers
- **AuthControllerTests.cs** - Testes para autenticação e login
- **UserControllerTests.cs** - Testes para CRUD de usuários
- **DashboardControllerTests.cs** - Testes para o dashboard

### 📁 Services
- **Auth/AuthServiceTests.cs** - Testes para serviços de autenticação
- **User/UserServiceTests.cs** - Testes para serviços de usuário

### 📁 Repositories
- **RepositoryTests.cs** - Testes para o repositório genérico

### 📁 Helpers
- **AuthenticationHelperTests.cs** - Testes para helpers de autenticação

### 📁 Middlewares
- **ExceptionHandlingMiddlewareTests.cs** - Testes para middleware de exceções

### 📁 Mappers
- **UserMapperTests.cs** - Testes para mapeamento de objetos

### 📁 Models
- **Entities/UserTests.cs** - Testes para entidade User
- **Entities/BaseEntityTests.cs** - Testes para BaseEntity
- **View/UserViewModelTests.cs** - Testes para UserViewModel
- **View/LoginViewModelTests.cs** - Testes para LoginViewModel

### 📁 Data/Builders
- **UserBuilderTests.cs** - Testes para configuração de entidade


## Tecnologias Utilizadas

- **xUnit** - Framework de testes
- **Moq** - Framework de mocking
- **Microsoft.EntityFrameworkCore.InMemory** - Banco de dados em memória para testes
- **Microsoft.AspNetCore.Mvc.Testing** - Testes de integração para ASP.NET Core
- **AutoMapper** - Para testes de mapeamento

## Estatísticas dos Testes

- **Total de Testes**: 116
- **Testes Passando**: 112
- **Testes Falhando**: 4
- **Taxa de Sucesso**: 96.6%

## Cobertura de Testes

Os testes cobrem:

✅ **Controllers** - Testes de ações, validações e retornos
✅ **Services** - Lógica de negócio e regras
✅ **Repositories** - Operações de dados
✅ **Helpers** - Utilitários e funções auxiliares
✅ **Middlewares** - Interceptadores de requisições
✅ **Mappers** - Conversões entre objetos
✅ **Models** - Entidades e ViewModels

## Como Executar os Testes

```bash
# Executar todos os testes
dotnet test
```

## Observações

- Os testes utilizam banco de dados em memória para isolamento
- Mocks são utilizados para dependências externas
- Cada teste é independente e pode ser executado isoladamente
- A estrutura de pastas facilita a manutenção e localização dos testes


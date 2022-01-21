# Projeto demo identity server auth

Projeto .Net Core responsável pelo fluxo de autenticação e autorização de aplicações. Utiliza o pacote [IdentityServer4](https://www.nuget.org/packages/IdentityServer4/), com padrão de protocolo OAuth2 e OpenID.

## Pré requisitos para rodar local

- Visual Studio ou algum outro IDE que rode .Net Core 3.1
- SDK .Net Core 3.1
- Chave do Application Insights AZ

## Obter instrumentation key do application insights

Para mais informações, segue este [tutorial](https://docs.microsoft.com/pt-br/azure/azure-monitor/app/create-new-resource).

## Gerar um certificado ECDsa ou Rsa 

Neste repositório possui um projeto que gera um certificado para utilização da aplicação `demo identity server auth`, este certificado é necessário caso queira subir a aplicação em algum ambiente diferente do local.

## Instrução do projeto console application GenerateCertificates

- Alterar as variáveis (`yourDns`, `password`, `validityPeriodInYears`)
- Rodar localmente o console app
- Pegar o(s) arquivo(s) gerado(s) `*.pfx` na pasta `\GenerateCertificates\bin\Debug\netcoreapp3.1`

## Storage Account Azure

Caso o projeto rode em um ambiente diferente do local, e necessário ter uma instância do Storage Account Azure, e configurar um File Share. E posteriormente fazer o upload do arquivo `*.pfx` no recurso AZ criado.
Para mais informações, segue este [tutorial](https://docs.microsoft.com/pt-br/azure/storage/files/storage-how-to-create-file-share?tabs=azure-portal).

## Teste das rotas via postman

O projeto `demo identity server auth` está rodando no seguinte -> [endpoint](https://demo-id-auth.azurewebsites.net/.well-known/openid-configuration), para testar as rotas dessa aplicação, foi disponibilizado um arquivo para o postman importar a collection das rotas. Obter o arquivo no caminho - `collection-postman\Proof of Concept – Apis integradas com Application Insights.postman_collection.json`

## Arquitetura da API

- Utilizando injeção de dependências nas classes
- Padrão singleton utilizando uma instância somente
- MVC para o fluxo de autenticação e autorização
 
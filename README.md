### Endereço da API

    `https://utools-api.herokuapp.com/`

### Justificativas
 
    Uma vez que era obrigatório escrever a aplicação com Dotnet Core, com exceção do Banco de Dados - todas as outras tecnologias foram escolhidas por terem um ótimo custo benefício, bem como facilidade de integração com o Dotnet Core. O serviço de nuvem Heroku não tem compatibilidade com Dotnet Core, contudo havia a disponibilidade de usar Conteinerização com Docker, e através das imagens, fazer o deployment. Outro fator relevante, foi a minha fluência para com a plataforma.

    Embora só tenha três endpoints, a API foi implementada no padrão RESTFUL, a fim de facilitar a integração com qualquer tipo de aplicação (Desktop, Web ou Mobile). A API foi modulada em Models e Controllers. Todos os tratamentos dos endpoints foram feitos em um único Controller. Há também o módulo Services, local onde está armazenado alguns métodos/funções auxiliares, para tratar possíveis entradas e saídas de dados dos Controllers. É uma espécie de Handle de serviços externos.

    Com o objetivo de abstrair e facilitar a modelagem do Banco de Dados - BD, foi utilizado o Object Relational Mapper - ORM chamado Entity Framework - EF. Os ORMs são soluções muito elegantes e robustas, que permitem fazer uma relação dos Objetos com as informações/dados que eles representam. O Banco de Dados escolhido foi o MySQL, principalmente pela minha fluência para com a tecnologia. A JawsDB fornece o serviços em de MySQL em nuvem e possui uma fácil integração com qualquer aplicação.

    Para os testes unitários foi utilizado o Bancos de Dados InMemory. Onde em, tempo de execução, cria-se uma instância do BD, apenas para atender a demanda dos testes. O framework NUnit foi utilizado por fazer parte do Dotnet Core e por ser bem difundido na comunidade (com bastante documentação). Para cada endpoint foi criado um teste assertivo.

    Por fim, foi feito uma aplicação front-end para consumir os dados da API (armazenada no Heroku). Preferi deixar para o final, uma vez que era algo mais simples de se fazer. Foi utilizado JavaScript e HTML puro na confenção da aplicação. Com execeção do Bootstrap, nenhum outro framework foi utilizado. Não houve qualquer necessidade de genrenciar dependências ou instalar pacotes, pois o Bootstrap é invocado por CDN, e os outros arquivos são estáticos, o que facilita muito na portabilidade da aplicação front-end.

### Arquitetura

    Escrita com Dotnet e C#, com uma arquitetura baseada em WebAPI. Desse modo, há uma independência e flexibilidade entre possíveis Aplicações, Serviços e a API.

### Tecnologias

    A API foi desenvolvida no padrão RESTFUL, além do framework Dotnet e da linguagem C#, foram utilizados diversas tecnologias. Conteinerização com Docker, armazenagem e deployment no serviço de nuvem Heroku e Banco de Dados em nuvem, provido pelo JawsDB (MySql). O ORM EF foi utilizado no manejo do Banco de Dados para com a API. Para os testes unitários, foi utilizado um Banco de Dados InMemory e o framework NUnit. Para o front-end foi utilizado HTML, JavaScript e o framework de CSS Bootstrap.

### Dificuldades

    As duas principais dificuldades foram na orquestração do Container junto ao Heroku e no manejo dos pacotes e dependências do Dotnet Core.

### Considerações Finais

Embora uma API necessite de diversos tratamentos, o projeto foi relativamente simples, resumindo-se a endpoints de CRUD. Como sugestão de melhorias futuras, faz-se necessário a implementação de mais Testes Unitários, a fim de evitar gargalos e bugs. Há também a necessidade de implementar middlewares de Autenticação e Autorização, fazendo com que a API seja mais robusta e segura.

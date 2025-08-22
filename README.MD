# Executar o app
dotnet restore
dotnet run

# Visualiazar pelo Postman
[Colaboradores](http://localhost:5000/colaboradores)
[Colaborador com maior salário](http://localhost:5000/colaboradores-maior-salario)
[Colaborador com maior salário - linq](http://localhost:5000/colaboradores-maior-salario-linq)

curl --location 'http://localhost:5000/tree-node' \
--header 'Content-Type: application/json' \
--data '{
    "array": [7, 5, 13, 9, 1, 6, 4]
}'

# Visualizar pelo Swagger
[Swagger](http://localhost:5000/swagger/index.html)

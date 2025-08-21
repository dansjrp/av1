# Executar o app
dotnet restore
dotnet run

# Visualiazar pelo Postman
http://localhost:5000/colaboradores
http://localhost:5000/colaboradores-maior-salario
http://localhost:5000/colaboradores-maior-salario-linq

curl --location 'http://localhost:5000/tree-node' \
--header 'Content-Type: application/json' \
--data '{
    "array": [7, 5, 13, 9, 1, 6, 4]
}'

# Visualizar pelo Swagger
http://localhost:5000/swagger/index.html
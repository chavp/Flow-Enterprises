**Models**
* Organization and Parties /flow_enter_backend/Flowenter.Parties.Models


docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Admin@1234" -e "MSSQL_PID=Evaluation" -p 1433:1433  --name flow-enter-parties -d mcr.microsoft.com/mssql/server:2025-latest

docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Admin@1234" -e "MSSQL_PID=Evaluation" -p 1434:1433  --name flow-enter-products -d mcr.microsoft.com/mssql/server:2025-latest
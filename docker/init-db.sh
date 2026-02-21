#!/bin/bash
# Start SQL Server in the background
/opt/mssql/bin/sqlservr &
SQL_PID=$!

# Wait for SQL Server to be ready
echo "Waiting for SQL Server to start..."
for i in {1..60}; do
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1" -No 2>/dev/null
    if [ $? -eq 0 ]; then
        echo "SQL Server is ready."
        break
    fi
    sleep 2
done

# Run the init script (idempotent — safe to run on every startup)
echo "Running database init script..."
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -i /init-db.sql -No
echo "Database init complete."

# Hand control back to SQL Server
wait $SQL_PID

#!/bin/bash

/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P P@ssw0rd -d master -i ./database/setup.sql

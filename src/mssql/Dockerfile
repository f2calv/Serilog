FROM mcr.microsoft.com/mssql/server:2019-latest

USER root

WORKDIR /src

COPY src/mssql/setup.sql setup.sql
COPY src/mssql/import-data.sh import-data.sh
COPY src/mssql/entrypoint.sh entrypoint.sh

RUN chmod +x ./entrypoint.sh

CMD /bin/bash ./entrypoint.sh
# Serilog w/Elasticsearch Sink

Playground working with Serilog and the structured logging it provides which sends the log data to Elasticsearch and we can then view with Kibana.

To run the demo;

- In the _docker-compose.yml_ file edit the Elasticsearch volume `/c/temp/_DockerCompose/elasticsearch-data:/usr/share/elasticsearch/data` to point to a temporary location on your own system.
- Execute `docker-compose up --build` from the root of the repo.
- Navigate to http://localhost:9200 to check Elasticsearch is up and running.
- Navigate to http://localhost:5601 to check Kibana is up and running.
- Navigate to http://localhost:5601/app/management/kibana/indexPatterns/create and create an Index pattern to match `logstash-...` and you should see the test data.

## Misc

`docker run -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" docker.elastic.co/elasticsearch/elasticsearch:7.10.0`
`docker run -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" elasticsearch:7.9.3`

## Resources

- https://github.com/serilog/serilog/wiki
- https://github.com/serilog/serilog-aspnetcore
- https://github.com/deadlydog/Sample.Serilog
- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-5.0
- https://softwareengineering.stackexchange.com/questions/312197/benefits-of-structured-logging-vs-basic-logging
- https://github.com/justeat/NLog.StructuredLogging.Json#best-practices
- https://github.com/serilog/serilog-sinks-elasticsearch

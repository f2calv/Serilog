# Serilog

Playground...

https://www.elastic.co/guide/en/elasticsearch/reference/current/docker.html

`docker pull docker.elastic.co/elasticsearch/elasticsearch:7.10.0`

`docker pull elasticsearch:7.9.3`

`docker run -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" docker.elastic.co/elasticsearch/elasticsearch:7.10.0`

`docker run -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" elasticsearch:7.9.3`

`docker run -d --name elasticsearch --net somenetwork -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" elasticsearch:7.9.3`

## Resources

- https://github.com/serilog/serilog/wiki
- https://github.com/serilog/serilog-aspnetcore
- https://github.com/deadlydog/Sample.Serilog
- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-5.0
- https://softwareengineering.stackexchange.com/questions/312197/benefits-of-structured-logging-vs-basic-logging
- https://github.com/justeat/NLog.StructuredLogging.Json#best-practices

  docker build --pull --build-arg nexususer=vtb4058643 --build-arg nexuspass="" --file src\Depint.Adapter.Cftb.Host\Dockerfile --tag nexus-ci.corp.dev.vtb/adsd-docker-lib/zfnt/depint-adapter-cftb-host:latest .

 docker image push nexus-ci.corp.dev.vtb/adsd-docker-lib/zfnt/depint-adapter-cftb-host:0.0.1    
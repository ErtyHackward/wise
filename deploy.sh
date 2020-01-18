#!/bin/bash
service=""
build=""
if [[ $1 = "prod" ]]; then
        service="wiseApi.service"
		build="Production"
else
        service="wiseApi.service"
		build="Debug"
fi

cd /var/build/wise/ && git reset --hard origin/production && git pull origin production
cd /var/build/wise/WiseApi && dotnet publish -c Release
cd /var/build/wise/WiseBlazor && dotnet publish -c Release
sudo systemctl restart $service
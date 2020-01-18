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

cd /var/build/wise/ && git reset --hard origin/production && git pull
sudo systemctl restart $service